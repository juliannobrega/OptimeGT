
using GQService.com.gq.service;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using MEMDataService.com.gq.service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_supuestoDto : _Gq_supuestoDto
    {
        public Gq_supuestoDto():base()
        {
        }
       
        public Gq_supuestoDto(Gq_supuesto value):base(value)
        {
        }

        public override Gq_supuestoDto SetEntity(Gq_supuesto value)
        {
            var dto = base.SetEntity(value);
            if (dto.Grupo > 0)
            {
                try
                {
                    dto.GrupoLabel = Services.Get<ServGq_supuesto_grupo>().findById(dto.Grupo).Nombre;
                }
                catch { }
            }
            if (dto.Depende > 0)
            {
                try
                {
                    dto.DependeLabel = Services.Get<ServGq_supuesto>().findById(dto.Depende).Nombre;
                }
                catch { }

            }
            return dto;
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }
        
        
        public override string Descripcion { get; set; }

        
        public override string Template { get; set; }

        
        public override string Scritp { get; set; }

        
        public override string CodeSharp { get; set; }

        
        public override string Folder { get; set; }

        
        public override long Grupo { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string TablaMod { get; set; }
              
        public override long Depende { get; set; }

        public override long Orden { get; set; }

        
        public override string Estado { get; set; }

        public string GrupoLabel { get; set; }

        public string DependeLabel { get; set; }

        public override string TablaModNombre { get; set; }
    }
}
