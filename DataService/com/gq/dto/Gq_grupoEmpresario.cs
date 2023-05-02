
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GQService.com.gq.encriptation;
using GQService.com.gq.service;
using GQService.com.gq.validate;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.service;

namespace MEMDataService.com.gq.dto
{
    public class Gq_grupoEmpresarioDto : _Gq_grupoEmpresarioDto
    {
        public Gq_grupoEmpresarioDto():base()
        {
        }
       
        public Gq_grupoEmpresarioDto(Gq_grupoEmpresario value):base(value)
        {
        }
        
        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }
                

        public override string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override long? Limite { get; set; }
    }
}
