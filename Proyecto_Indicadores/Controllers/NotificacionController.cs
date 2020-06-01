using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Indicadores.Controllers
{
    public class NotificacionController : Controller
    {
        // GET: Notificacion
        public ActionResult Notificacion(string[] datos)
        {
            ViewBag.bandera = datos[0];
            ViewBag.mensaje = datos[1];
            ViewBag.vista = datos[2];
            ViewBag.controlador = datos[3];


            return View(ViewBag);
        }
    }
}