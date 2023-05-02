using System.Collections.Generic;

namespace MEM.Controllers.Dto
{
    public class PreciosDto
    {
        public long IdEscenario { get; set; }
        public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();
    }

    public class PrecioDto
    {
        public string Nombre { get; set; }
        public string Id_contrato { get; set; }
        public string Id_combustible1 { get; set; }
        public string Id_combustible2 { get; set; }
        public string Estado { get; set; }
        public decimal? Ctung { get; set; }
        public decimal? Omnoren { get; set; }
        public decimal? Omren { get; set; }
        public decimal? Peo { get; set; }
        public decimal? Ppo { get; set; }
        public decimal? Ci { get; set; }
        public decimal? Citt { get; set; }
        public decimal? Cte { get; set; }
        public decimal? Pcal { get; set; }
        public decimal? Cem { get; set; }
        public decimal? Fagn { get; set; }
        public decimal? Horasren { get; set; }
        public decimal? Horasnoren { get; set; }
        public decimal? Pgmax { get; set; }
        public decimal? Pgmin { get; set; }
        public long? Anioinicio { get; set; }
        public decimal? Fpnoren { get; set; }
        public decimal? Fpren { get; set; }
        public decimal? F { get; set; }
        public decimal? K { get; set; }
        public long? Habilitado_pujar { get; set; }
        public long? Idronda { get; set; }
    }
}
