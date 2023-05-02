
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_descargasDto : _Gq_descargasDto
    {
        public Gq_descargasDto():base()
        {
        }
       
        public Gq_descargasDto(Gq_descargas value):base(value)
        {
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }
    }
}
