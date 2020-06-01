using Proyecto_Indicadores.Models;
using Proyecto_Indicadores.Models.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Data;

namespace Proyecto_Indicadores.Controllers
{
    public class PosValorController : Controller
    {
        // GET: PosValor

        public List<Posvalor> Consulta() {

            BD_Indicadores consulta = new BD_Indicadores();
            var resultado = (from c in consulta.TB_Posvalor
                             join a in consulta.TB_Metadata
                             on c.IdMetadata equals a.IdMetadata
                             select new Posvalor
                             {
                                 IdPosvalor = c.IdPosvalor,
                                 Valor = a.Prefijo + c.Valor,
                                 Descripcion = a.Nombre + ": " + c.Descripcion
                             }).ToList();
            return resultado;
        }

        public ActionResult PosValor()
        {
            var resultado = Consulta();
            return View(resultado);
        }

        public ActionResult CrearPosValor()
        {
            BD_Indicadores contexto = new BD_Indicadores();
            var lstMetadatos = contexto.TB_Metadata.Select(x => x).ToList();
            ViewBag.ddlMetadata = new SelectList(lstMetadatos, "IdMetadata", "Prefijo");

            return View();
        }

        public ActionResult ModificarPosValor(int idposvalor) {

            BD_Indicadores contexto = new BD_Indicadores();

            var consultaPosvalor = (from p in contexto.TB_Posvalor
                                    where p.IdPosvalor == idposvalor
                                    select new Posvalor
                                    {
                                        IdPosvalor = idposvalor,
                                        Valor = p.Valor,
                                        Descripcion = p.Descripcion,
                                        IdMetadata = p.IdMetadata
                                    }).FirstOrDefault();

            var lstMetadatos = contexto.TB_Metadata.Select(x => x).ToList();

            ViewBag.ddlMetadata = new SelectList(lstMetadatos, "IdMetadata", "Prefijo");


            return View("CrearPosValor", consultaPosvalor);
        }

        public JsonResult GuardarPosValor(Posvalor objPosvalor)
        {
            string result = string.Empty;
            if (objPosvalor != null)
            {
                Regex val = new Regex(@"^[a-zA-Z]*$");
                if (val.IsMatch(objPosvalor.Valor))
                {
                    using (BD_Indicadores insert = new BD_Indicadores())
                    {
                        TB_Posvalor posvalor = new TB_Posvalor
                        {
                            Valor = objPosvalor.Valor,
                            IdMetadata = objPosvalor.IdMetadata,
                            Descripcion = objPosvalor.Descripcion
                        };

                        insert.TB_Posvalor.Add(posvalor);
                        insert.SaveChanges();
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

        public void EditarPosValor(Posvalor objPosValor)
        {
            Regex val = new Regex(@"^[a-zA-Z]*$");
            if (val.IsMatch(objPosValor.Valor))
            {
                using (BD_Indicadores contexto = new BD_Indicadores())
                {
                    TB_Posvalor objPosvalor = contexto.TB_Posvalor.Single(p => p.IdPosvalor == objPosValor.IdPosvalor);
                    objPosvalor.Valor = objPosValor.Valor;
                    objPosvalor.Descripcion = objPosValor.Descripcion;
                    objPosvalor.IdMetadata = objPosValor.IdMetadata;

                    contexto.SaveChanges();
                    contexto.Dispose();
                }
            }
            else
            {
                ViewBag.Message = string.Format("Solo se admiten letras");
            }
        }

        public ActionResult EliminarPosValor(int idposvalor)
        {
            try
            {
                if (idposvalor > 0)
                {
                    using (BD_Indicadores contexto = new BD_Indicadores())
                    {
                        TB_Posvalor objPosValor = contexto.TB_Posvalor.FirstOrDefault(x => x.IdPosvalor == idposvalor);

                        if (objPosValor != null)
                        {
                            contexto.TB_Posvalor.Remove(objPosValor);
                            contexto.SaveChanges();
                            ViewBag.Msg = string.Format("Registro eliminado correctamente!");
                        }
                    }
                }
            }
            catch
            {
                ViewBag.Msg = string.Format("Ocurrio un error al intentar elmimanar el Posvalor.\n" +
                                            "Compruebe que no esté ligado a un indicador");
            }
            var resultado = Consulta();
            return View("PosValor", resultado);
        }

        public ActionResult ExportarExcel()
        {

            BD_Indicadores contexto = new BD_Indicadores();

            var stream = new MemoryStream();
            var serialicer = new XmlSerializer(typeof(List<Posvalor>));


            List<TB_Posvalor> data = contexto.TB_Posvalor.Select(x => x).ToList();

            //List<Posvalor> datos = (from p in contexto.TB_Posvalor
            //                        select new Posvalor
            //                        {
            //                            IdPosvalor = p.IdPosvalor,
            //                            Valor = p.Valor,
            //                            Descripcion = p.Descripcion,
            //                            IdMetadata = p.IdMetadata
            //                            }).ToList();

            serialicer.Serialize(stream, data);
            stream.Position = 0;

            return File(stream, "application/vnd.ms-excel", "PosValores.xls");
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

                    DataSet ds = new DataSet();//Using dataset to read xml file  
                    ds.ReadXml(path);

                    var _posvalor = new List<TB_Posvalor>();
                    _posvalor = (from rows in ds.Tables[0].AsEnumerable()
                                     select new TB_Posvalor
                                     {
                                         Valor = rows[0].ToString(),
                                         Descripcion = rows[1].ToString(),
                                         IdMetadata = Convert.ToInt32(rows[2]),
                                     }).ToList();

                    using (BD_Indicadores insert = new BD_Indicadores())
                    {
                        foreach (TB_Posvalor posvalor in _posvalor)
                        {
                            insert.TB_Posvalor.Add(posvalor);
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

            var lista = Consulta();
            return View("PosValor", lista);

        }

    }


}

