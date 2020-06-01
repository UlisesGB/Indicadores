using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using Proyecto_Indicadores.Models;
using Proyecto_Indicadores.Models.BaseDatos;
using System.Data.Entity;

namespace IndicadoresBancarios.Controllers
{
    public class IndicadorController : Controller
    {
        private readonly BD_Indicadores contexto = new BD_Indicadores();

        public ActionResult CrearIndicador()
        {
            CargarDropDown objDatosDropDownList = new CargarDropDown()
            {
                listaCatalogos = new SelectList(contexto.TB_Catalogo.Select(x => x), "IdCatalogo", "Prefijo"),
                listaEstados = new SelectList(contexto.TB_Estado.Select(x => x), "IdEstado", "Sigla"),
                listaUnidades = new SelectList(contexto.TB_UnidadMedida.Select(x => x), "IdUnidad", "Siglas"),
                listaPeriodos = new SelectList(contexto.TB_Periodo.Select(x => x), "IdPeriodo", "Siglas")
            };
            return View(objDatosDropDownList);
        }

        public ActionResult CrearIndicador_Posvalores()
        {
            var resultado = (from c in contexto.TB_Posvalor
                             join a in contexto.TB_Metadata
                             on c.IdMetadata equals a.IdMetadata
                             select new Posvalor
                             {
                                 IdPosvalor = c.IdPosvalor,
                                 Valor = a.Prefijo + c.Valor,
                                 Descripcion = a.Nombre + ": " + c.Descripcion
                             }).ToList();
            return View(resultado);
        }

        public ActionResult EditarIndicador(int idIndicador)
        {
            var indicador = contexto.TB_Indicador.Where(x => x.IdIndicador == idIndicador).FirstOrDefault();
            CargarDropDown objDatosDropDownList = new CargarDropDown()
            {
                listaCatalogos = new SelectList(contexto.TB_Catalogo.Select(x => x), "IdCatalogo", "Prefijo", indicador.IdCatalogo),
                listaEstados = new SelectList(contexto.TB_Estado.Select(x => x), "IdEstado", "Sigla", indicador.IdEstado),
                listaPeriodos = new SelectList(contexto.TB_Periodo.Select(x => x), "IdPeriodo", "Siglas", indicador.IdPeriodo),
                listaUnidades = new SelectList(contexto.TB_UnidadMedida.Select(x => x), "IdUnidad", "Siglas", indicador.IdUnidad)

            };
            return View("CrearIndicador", objDatosDropDownList);
        }

        [HttpPost]
        public ActionResult GetIds(string[] arrValores)
        {
            TB_Indicador objIndicador = new TB_Indicador();

            string Prefijo = arrValores[0];
            string Estado = arrValores[1];
            string Periodo = arrValores[2];
            string UnidadMedida = arrValores[3];

            var catalogo = contexto.TB_Catalogo.Where(x => x.Prefijo == Prefijo).FirstOrDefault();
            var estado = contexto.TB_Estado.Where(x => x.Sigla == Estado).FirstOrDefault();
            var periodo = contexto.TB_Periodo.Where(x => x.Siglas == Periodo).FirstOrDefault();
            var unidad = contexto.TB_UnidadMedida.Where(x => x.Siglas == UnidadMedida).FirstOrDefault();

            objIndicador.IdCatalogo = catalogo.IdCatalogo;
            objIndicador.IdEstado = estado.IdEstado;
            objIndicador.IdPeriodo = periodo.IdPeriodo;
            objIndicador.IdUnidad = unidad.IdUnidad;

            GuardarCambiosIndicador(objIndicador);
            GuardarIndicadorPosvalor(objIndicador.IdIndicador, arrValores);

            return View(arrValores);
        }

        public ActionResult ListarIndicadores()
        {
            List<TablaIndicador> listaindicadores = Listar();
            return View(listaindicadores);
        }

        private List<TablaIndicador> Listar()
        {
            List<TablaIndicador> listaIndicadores = new List<TablaIndicador>();

            var consultaIndicadores = contexto.V_SelectIndicadores.Where(x => x.Id == x.Id);
            var consultaMetadata = contexto.V_SelectMetadata.Where(x => x.Indicador == x.Indicador);

            DataTable TMetadatas = new DataTable();
            TMetadatas.Columns.Add("Id");
            TMetadatas.Columns.Add("Metadata");
            TMetadatas.Columns.Add("PosValor");

            foreach (var m in consultaMetadata)
            {
                DataRow row = TMetadatas.NewRow();
                row["Id"] = m.Indicador;
                row["Metadata"] = m.Metadata;
                row["PosValor"] = m.Posvalor;

                TMetadatas.Rows.Add(row);
            }

            foreach (var t in consultaIndicadores)
            {
                TablaIndicador objtablaIndicador = new TablaIndicador
                {
                    IdIndicador = t.Id,
                    Catalogo = t.Prefijo,
                    Estado = t.Estado,
                    Perido = t.Periodo,
                    Moneda = t.Moneda
                };

                foreach (DataRow row in TMetadatas.Rows)
                {
                    if (t.Id == Convert.ToInt32(row["Id"]))
                    {
                        objtablaIndicador.Metadata += row["Metadata"].ToString() + row["PosValor"].ToString() + ".";
                    }
                }
                objtablaIndicador.Metadata = objtablaIndicador.Metadata.Substring(0, (objtablaIndicador.Metadata.Length - 1));

                listaIndicadores.Add(objtablaIndicador);
                objtablaIndicador = null;
            }

            return listaIndicadores;
        }

        private void GuardarIndicadorPosvalor(int idIndicador, string[] arrValores)
        {
            for (int i = 4; i < arrValores.Length; i++)
            {
                string valor = arrValores[i].Remove(0, 3);
                var posvalor = contexto.TB_Posvalor.Where(x => x.Valor == valor).FirstOrDefault();

                TB_IndicadorPosvalor objIndiPosvalor = new TB_IndicadorPosvalor()
                {
                    IdIndicador = idIndicador,
                    IdPosvalor = posvalor.IdPosvalor,
                    Orden = i - 3
                };
                contexto.TB_IndicadorPosvalor.Add(objIndiPosvalor);
                contexto.SaveChanges();
            }
        }

        public ActionResult EliminarIndicador(int idIndicador)
        {
            try
            {
                foreach (var x in contexto.TB_IndicadorPosvalor)
                {
                    if (x.IdIndicador == idIndicador)
                    {
                        contexto.TB_IndicadorPosvalor.Remove(x);
                    }
                }
                contexto.SaveChanges();

                var indicador = contexto.TB_Indicador.Where(x => x.IdIndicador == idIndicador).FirstOrDefault();
                contexto.TB_Indicador.Remove(indicador);
                contexto.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.Msg = string.Format("Ocurrio un error al eliminar el archivo, compruebe el listado de nuevo") + e;
            }
            var list = Listar();
            return View("ListarIndicadores", (object)list);
        }

        public ActionResult ExportarExcel()
        {
            var stream = new MemoryStream();
            var serialicer = new XmlSerializer(typeof(List<TablaIndicador>));

            List<TablaIndicador> datos = Listar();

            serialicer.Serialize(stream, datos);
            stream.Position = 0;
            return File(stream, "application/vnd.ms-excel", "Unidades de Medida.xls");
        }

        private void GuardarCambiosIndicador(TB_Indicador objIndicador)
        {
            contexto.TB_Indicador.Add(objIndicador);
            contexto.SaveChanges();
        }
    }
}

