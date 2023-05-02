namespace MEM.Controllers.Dto
{
    public class AdjudicadosDto
    {
        public string Nombre { get; set; }
        public long Ae { get; set; }
        public long Seleccionado { get; set; }
        public double? PGMx { get; set; }
        public double? PGMn { get; set; }
        public double? PMedAdj { get; set; }
        public double? Energia { get; set; }
        public double? PorcentajeReduccion { get; set; }
        public double? PorcentajeParaTotal { get; set; }
        public double? MonomicoTot { get; set; }
    }
}
