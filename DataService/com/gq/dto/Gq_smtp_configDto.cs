
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_smtp_configDto : _Gq_smtp_configDto
    {
        public Gq_smtp_configDto():base()
        {
        }
       
        public Gq_smtp_configDto(Gq_smtp_config value):base(value)
        {
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string NombreFrom { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Host { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override Int32 Port { get; set; }

        [Required(ErrorMessage = "Campo requerido con formato email")]
        public override string EMailFrom { get; set; }
    }
}
