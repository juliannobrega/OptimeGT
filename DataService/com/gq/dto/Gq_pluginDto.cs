
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_pluginDto : _Gq_pluginDto
    {
        public Gq_pluginDto():base()
        {
        }
       
        public Gq_pluginDto(Gq_plugin value):base(value)
        {
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }
    }
}
