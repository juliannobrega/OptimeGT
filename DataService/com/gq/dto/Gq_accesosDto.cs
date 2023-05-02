
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System;
using System.Collections.Generic;
using GQService.com.gq.security;

namespace MEMDataService.com.gq.dto
{
    public class Gq_accesosDto : _Gq_accesosDto
    {
        public SecurityDescription extra;

        public Gq_accesosDto():base()
        {
        }
       
        public Gq_accesosDto(Gq_accesos value):base(value)
        {
        }
    }
}
