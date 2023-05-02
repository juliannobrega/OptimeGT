
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System;
using System.Collections.Generic;

namespace MEMDataService.com.gq.dto
{
    public class Gq_menuDto : _Gq_menuDto
    {
        public Gq_menuDto():base()
        {
        }
       
        public Gq_menuDto(Gq_menu value):base(value)
        {
        }

        public List<Gq_menuDto> Child { get; set; } = new List<Gq_menuDto>();
    }
}
