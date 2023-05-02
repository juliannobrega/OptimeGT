using System.ComponentModel.DataAnnotations;
using GQDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;

namespace GQDataService.com.gq.dto
{
    public class Gq_mailTemplateDto : _Gq_mailTemplateDto
    {
        public Gq_mailTemplateDto() : base()
        {
        }

        public Gq_mailTemplateDto(Gq_mailTemplate value) : base(value)
        {
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Folder { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Estado { get; set; }

    }
}
