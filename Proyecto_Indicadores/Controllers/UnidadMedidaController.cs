using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_Indicadores.Models.BaseDatos;
using Proyecto_Indicadores.Models;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;


namespace Proyecto_Indicadores.Controllers
{
    public class UnidadMedidaController : Controller
    {

        public List<UnidadMedida> ListUnidadMedida()
        {
            BD_Indicadores consulta = new BD_Indicadores();
            var resultado = (from c in consulta.TB_UnidadMedida
                             select new UnidadMedida
                             {
                                 IdUnidad = c.IdUnidad,
                                 Siglas = c.Siglas,
                                 Descripcion = c.Descripcion
                             }).ToList();

            return (resultado);
        }

        public ActionResult GetDataPreEdit(int idUnidad)
        {
            BD_Indicadores consulta = new BD_Indicadores();
            var consultaUnidad = (from u in consulta.TB_UnidadMedida
                                  where u.IdUnidad == idUnidad
                                  select new UnidadMedida
                                  {
                                      IdUnidad = idUnidad,
                                      Siglas = u.Siglas,
                                      Descripcion = u.Descripcion
                                  }).FirstOrDefault();


            return View("CrearUnidadMedida", consultaUnidad);
        }

        public void EditarUnidadMedida(UnidadMedida objUnidadMedida)
        {
            Regex val = new Regex(@"^[a-zA-Z]*$");
            if (val.IsMatch(objUnidadMedida.Siglas))
            {
                using (BD_Indicadores update = new BD_Indicadores())
                {
                    TB_UnidadMedida unidad = update.TB_UnidadMedida.Single(u => u.IdUnidad == objUnidadMedida.IdUnidad);
                    unidad.Siglas = objUnidadMedida.Siglas;
                    unidad.Descripcion = objUnidadMedida.Descripcion;
                    update.SaveChanges();
                    update.Dispose();
                }
            }
            else
            {
                ViewBag.Message = string.Format("Solo se admiten letras");
            }
        }

        [HttpPost]
        public JsonResult GuardarUnidadMedida(UnidadMedida objUnidadMedida)
        {
            string result = string.Empty;
            if (objUnidadMedida != null)
            {
                Regex val = new Regex(@"^[a-zA-Z]*$");
                if (val.IsMatch(objUnidadMedida.Siglas))
                {
                    using (BD_Indicadores contexto = new BD_Indicadores())
                    {
                        TB_UnidadMedida unidad = new TB_UnidadMedida
                        {
                            Siglas = objUnidadMedida.Siglas,
                            Descripcion = objUnidadMedida.Descripcion
                        };

                        contexto.TB_UnidadMedida.Add(unidad);
                        contexto.SaveChanges();
                        result = "Saved";
                    }
                }
                else
                {
                    ViewBag.Message = string.Format("Solo se admiten letras");
                }
            }
            return Json(result);
        }

        public ActionResult EliminarUnidadMedida(int idUnidad)
        {
            try
            {
                if (idUnidad > 0)
                {
                    using (BD_Indicadores delete = new BD_Indicadores())
                    {
                        TB_UnidadMedida unidad = (from u in delete.TB_UnidadMedida
                                                  where u.IdUnidad == idUnidad
                                                  select u).FirstOrDefault();
                        if (unidad != null)
                        {
                            delete.TB_UnidadMedida.Remove(unidad);
                            delete.SaveChanges();
                            ViewBag.Msg = string.Format("Registro eliminado correctamente!");
                        }
                    }
                }
            }
            catch (Exception)
            {

                ViewBag.Msg = string.Format("Ocurrio un error al eliminar el archivo, compruebe el listado de nuevo");
            }
            var resultado = ListUnidadMedida();

            return View("UnidadMedida", resultado);
        }

        public ActionResult ExportarExcel()
        {

            BD_Indicadores consultar = new BD_Indicadores();

            var stream = new MemoryStream();
            var serialicer = new XmlSerializer(typeof(List<UnidadMedida>));

            List<UnidadMedida> datos = (from u in consultar.TB_UnidadMedida
                                        select new UnidadMedida
                                        {
                                            IdUnidad = u.IdUnidad,
                                            Siglas = u.Siglas,
                                            Descripcion = u.Descripcion
                                        }).ToList();

            serialicer.Serialize(stream, datos);
            stream.Position = 0;

            return File(stream, "application/vnd.ms-excel", "Unidades de Medida.xls");
        }

        public ActionResult ImportXmlData(HttpPostedFileBase xmlFile)
        {
            try
            {
                if (xmlFile != null && xmlFile.ContentLength > 0)
                {
                    var nombreArchivo = Path.GetFileName(xmlFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/"), nombreArchivo);
                    xmlFile.SaveAs(path);

                    DataSet ds = new DataSet();
                    ds.ReadXml(path);

                    var _unidadMedida = new List<TB_UnidadMedida>();
                    _unidadMedida = (from rows in ds.Tables[0].AsEnumerable()
                                     select new TB_UnidadMedida
                                     {
                                         Siglas = rows[0].ToString(),
                                         Descripcion = rows[1].ToString(),
                                     }).ToList();

                    using (BD_Indicadores insert = new BD_Indicadores())
                    {
                        foreach (TB_UnidadMedida unidad in _unidadMedida)
                        {
                            insert.TB_UnidadMedida.Add(unidad);
                            insert.SaveChanges();
                            ViewBag.Msg = "Archivo cargado correctamente";
                        }
                    }

                }

            }
            catch (Exception)
            {
                ViewBag.Msg = string.Format("Ocurrio un error al intentar cargar el archivo seleccionado."); ;
            }
            var lista = ListUnidadMedida();
            return View("UnidadMedida", lista);
        }


        public ActionResult UnidadMedida()
        {
            var resultado = ListUnidadMedida();
            return View(resultado);
        }

        public ActionResult CrearUnidadMedida()
        {
            return View();
        }


    }
}