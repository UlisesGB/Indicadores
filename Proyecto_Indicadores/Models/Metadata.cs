using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Indicadores.Models
{
    [Serializable]
    [XmlRoot("Metadata"), XmlType("Metadata")]
    public class Metadata
    {
        public int IdMetadata { get; set; }
        public String Nombre { get; set; }
        public String Descripcion { get; set; }

        [StringLength(3)]
        public String Prefijo { get; set; }
    }
}