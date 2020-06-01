using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Indicadores.Models
{
    public class Posvalor
    {
        #region Properties
        public int IdPosvalor { get; set; }
        //[Required(ErrorMessage = "Valor es requerido.")]
        public string Valor { get; set; }
        //[Required(ErrorMessage = "Descripcion es requerida.")]
        public string Descripcion { get; set; }
        //[Required(ErrorMessage = "Metadata es requerida.")]
        public int IdMetadata { get; set; }
        #endregion       
    }
}