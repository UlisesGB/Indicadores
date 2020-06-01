using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Indicadores.Models
{
    public class IndicadorPosvalor
    {
        [Key]
        public int IdIndiPos { get; set; }
        public int IdIndicador { get; set; }       
        public int IdPosvalor { get; set; }
        public int Orden { get; set; }
    }
}