


namespace Proyecto_Indicadores.Models
{
    public class Indicador
    {
        public int IdIndicador { get; set; }
        public int IdCatalogo { get; set; }
        public int IdUnidad { get; set; }
        public int IdPeriodo { get; set; }
        public int IdEstado { get; set; }
    }

    public class TablaIndicador {

        public int IdIndicador { get; set; }
        public string Catalogo { get; set; }
        public string Estado { get; set; }
        public string Perido { get; set; }
        public string Moneda { get; set; }
        public string Metadata { get; set; }
    }

}