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
    public class MetadataController : Controller
    {

        public ActionResult CrearMetadatas()
        {
            return View();
        }

        public ActionResult ListarMetadatas()
        {
            var resultado = GetMetadatas();
            return View(resultado);
        }

        public List<Metadata> GetMetadatas()
        {
            BD_Indicadores contexto = new BD_Indicadores();

            List<Metadata> resultado = (from m in contexto.TB_Metadata
                                        select new Metadata
                                        {
                                            IdMetadata = m.IdMetadata,
                                            Nombre = m.Nombre,
                                            Descripcion = m.Descripcion,
                                            Prefijo = m.Prefijo
                                        }).ToList();
            return resultado;
        }

        public ActionResult EliminarMetadatas(int idMetadata)
        {
            try
            {
                if (idMetadata > 0)
                {
                    using (BD_Indicadores contexto = new BD_Indicadores())
                    {
                        var consulta = contexto.TB_Metadata.Where(x => x.IdMetadata == idMetadata).FirstOrDefault();

                        if (consulta != null)
                        {
                            contexto.TB_Metadata.Remove(consulta);
                            contexto.SaveChanges();
                            ViewBag.Msg = string.Format("Metadata {0} eliminado correctamente", consulta.Prefijo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = string.Format("No se puede eliminar el registro, compruebe que" +
                                             " no tenga PosValores enlazados al mismo.\n Error " + ex.Message);
            }
            var list = GetMetadatas();
            return View("ListarMetadatas", list);
        }

        public ActionResult MetadataDetails(int idMetadata)
        {
            BD_Indicadores contexto = new BD_Indicadores();
            var consultar = contexto.TB_Posvalor.Select(x => x);
            var consulta = contexto.TB_Posvalor.Where(x => x.IdMetadata == idMetadata);
            return PartialView("MetadataDetails", consulta);
        }

        public ActionResult EditarMetadatas(int idMetadata)
        {
            BD_Indicadores contexto = new BD_Indicadores();

            TB_Metadata objMetada = contexto.TB_Metadata.FirstOrDefault(x => x.IdMetadata == idMetadata);

            return View("CrearMetadatas", objMetada);
        }

        public JsonResult SaveMetadata(Metadata objMetadata)
        {
            try
            {
                string result = string.Empty;
                if (objMetadata != null)
                {
                    Regex val = new Regex(@"^[a-zA-Z]*$");
                    Regex val2 = new Regex(@"^[a-zA-Z]+[_]$");
                    if (val.IsMatch(objMetadata.Nombre) && val2.IsMatch(objMetadata.Prefijo))
                    {
                        using (BD_Indicadores insert = new BD_Indicadores())
                        {
                            TB_Metadata meta = new TB_Metadata
                            {
                                Nombre = objMetadata.Nombre,
                                Descripcion = objMetadata.Descripcion,
                                Prefijo = objMetadata.Prefijo
                            };
                            insert.TB_Metadata.Add(meta);
                            insert.SaveChanges();
                            result = "Saved";
                        }
                    }
                    else
                    {
                        ViewBag.Message = string.Format("Solo se admiten letras en el nombre y que el prefijo termine en guion bajo");
                    }
                }
                return Json(result);
            }
            catch (Exception exc) { return Json(exc.Message); }

        }

        public JsonResult EditMetadata(Metadata objMetadata)
        {
            try
            {
                string result = string.Empty;
                if (objMetadata != null)
                {
                    Regex val = new Regex(@"^[a-zA-Z]*$");
                    Regex val2 = new Regex(@"^[a-zA-Z]+[_]$");
                    if (val.IsMatch(objMetadata.Nombre) && val2.IsMatch(objMetadata.Prefijo))
                    {
                        using (BD_Indicadores edit = new BD_Indicadores())
                        {
                            TB_Metadata meta = edit.TB_Metadata.Single(m => m.IdMetadata == objMetadata.IdMetadata);
                            meta.Nombre = objMetadata.Nombre;
                            meta.Descripcion = objMetadata.Descripcion;
                            meta.Prefijo = objMetadata.Prefijo;
                            edit.SaveChanges();
                            result = "Saved";
                        }
                    }
                    else
                    {
                        ViewBag.Message = string.Format("Solo se admiten letras en el nombre y que el prefijo termine en guion bajo");
                    }
                }
                return Json(result);
            }
            catch (Exception exc) { return Json(exc.Message); }
        }

        public ActionResult MetadataXmlExcel()
        {
            BD_Indicadores consulta = new BD_Indicadores();
            TB_Metadata _meta = new TB_Metadata();
            var stream = new MemoryStream();
            var serialicer = new XmlSerializer(typeof(List<Metadata>));

            List<Metadata> datos = (from m in consulta.TB_Metadata
                                    select new Metadata
                                    {
                                        IdMetadata = m.IdMetadata,
                                        Nombre = m.Nombre,
                                        Descripcion = m.Descripcion,
                                        Prefijo = m.Prefijo
                                    }).ToList();

            serialicer.Serialize(stream, datos);
            stream.Position = 0;

            return File(stream, "application/vnd.ms-excel", "Metadatas.xls");
        }

        public ActionResult ImportXmlData(HttpPostedFileBase xmlFile)
        {
            try
            {
                //xmlFile es el name del input typo file en la vista
                //valido que no sea null y que tenga datos
                if (xmlFile != null && xmlFile.ContentLength > 0)
                {
                    //obtengo el nombre del archivo y lo guardo en una ruta temporarl para abrirlo desde ahi
                    var nombreArchivo = Path.GetFileName(xmlFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/"), nombreArchivo);
                    xmlFile.SaveAs(path);
                    //abro el archivo y meto los datos en un dataset
                    DataSet ds = new DataSet();//Using dataset to read xml file  
                    ds.ReadXml(path);
                    //creo por cada linea del data set un metadata para luego guardarlo en base
                    var _metadatas = new List<TB_Metadata>();
                    _metadatas = (from rows in ds.Tables[0].AsEnumerable()
                                  select new TB_Metadata
                                  {
                                      Nombre = rows[0].ToString(),
                                      Descripcion = rows[1].ToString(),
                                      Prefijo = rows[2].ToString(),
                                  }).ToList();
                    //recorro los metadatas en la lista y los guardo 
                    using (BD_Indicadores insert = new BD_Indicadores())
                    {
                        foreach (TB_Metadata meta in _metadatas)
                        {
                            insert.TB_Metadata.Add(meta);
                            insert.SaveChanges();
                            ViewBag.Msg = "Archivo cargado correctamente";
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Msg = string.Format("Ocurrio un error al intentar cargar el archivo seleccionado.");
            }
            //recargo la vista para que muestre los nuevos metadatas
            var list = GetMetadatas();
            return View("ListarMetadatas", list);
        }
    }
}