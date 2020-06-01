using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Indicadores.Models
{
    public class CargarDropDown
    {
        public void CargarDropDownList()
        {
            this.listaEstados = new SelectList(new List<string>());
            this.listaUnidades = new SelectList(new List<string>());
            this.listaPeriodos = new SelectList(new List<string>());
        }


        public int idEstado { get; set; }
        public string siglaEstado { get; set; }
        public SelectList listaEstados { get; set; }


        public int idUnidad { get; set; }
        public string siglaUnidad { get; set; }
        public SelectList listaUnidades { get; set; }


        public string idPeriodo { get; set; }
        public string siglaPeriodo { get; set; }
        public SelectList listaPeriodos { get; set; }


        public string idCatalogo { get; set; }
        public string prefijoCatalogo { get; set; }
        public SelectList listaCatalogos { get; set; }
    }
}