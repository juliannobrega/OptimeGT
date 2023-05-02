using GQService.com.gq.service;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using MEMDataService.com.gq.service;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_escenariosDto : _Gq_escenariosDto
    {
        public Gq_escenariosDto() : base()
        {
        }

        public Gq_escenariosDto(Gq_escenarios value) : base(value)
        {

        }

        public override Gq_escenariosDto SetEntity(Gq_escenarios value)
        {
            var dto = base.SetEntity(value);
            if (dto.EscenarioOrigenId > 0)
            {
                try
                {
                    dto.EscenarioBaseNombre = Services.Get<ServGq_escenarios>().findById(dto.EscenarioOrigenId).Nombre;
                }
                catch { }
            }

            switch (dto.Estado)
            {
                case "S":
                    dto.EstadoLabel = "SIN BASE";
                    break;
                case "E":
                    dto.EstadoLabel = "EDICION";
                    break;
                case "G":
                    dto.EstadoLabel = "GENERADO";
                    break;
                case "P":
                    dto.EstadoLabel = "PROCESANDO";
                    break;                
            }
            if (dto.CreadoPor > 0 )
            {
                try
                {
                    var user =  Services.Get<ServGq_usuarios>().findById(dto.CreadoPor);
                    dto.CreadoPorNombre = user.Nombre + " " + user.Apellido;
                }
                catch (System.Exception)
                {
                    dto.CreadoPorNombre = string.Empty;
                    //throw;
                }
            }
            if (dto.ModificadoPor > 0)
            {
                try
                {
                    var user = Services.Get<ServGq_usuarios>().findById(dto.CreadoPor);
                    dto.ModificadoPorNombre = user.Nombre + " " + user.Apellido;
                }
                catch (System.Exception)
                {
                    dto.ModificadoPorNombre = string.Empty;
                    //throw;
                }
            }

            return dto;
        }

        [Required]
        public override System.String Nombre { get; set; }

        [Required]
        public override System.String Estado { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override System.Int64? PluginId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override System.Int32? FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override System.Int32? FechaFin { get; set; }
               
        public override System.Int64? EscenarioOrigenId { get; set; }

        public bool GenerarDatos { get; set; }

        public string EscenarioBaseNombre { get; set; }

        public string EstadoLabel { get; set; }

        public override System.Int64 CreadoPor { get; set; }

        public System.String CreadoPorNombre { get; set; }

        public System.String ModificadoPorNombre { get; set; }

        public override System.String Publico { get; set; }

        public override System.Int64? GrupoEmpresarioId { get; set; }
    }
}
