//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Indicadores.Models.BaseDatos
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_Posvalor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_Posvalor()
        {
            this.TB_IndicadorPosvalor = new HashSet<TB_IndicadorPosvalor>();
        }
    
        public int IdPosvalor { get; set; }
        public string Valor { get; set; }
        public string Descripcion { get; set; }
        public int IdMetadata { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_IndicadorPosvalor> TB_IndicadorPosvalor { get; set; }
        public virtual TB_Metadata TB_Metadata { get; set; }
    }
}
