
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GQService.com.gq.validate;
using GQService.com.gq.service;
using MEMDataService.com.gq.service;

namespace MEMDataService.com.gq.dto
{
    public class Gq_graficoDto : _Gq_graficoDto
    {
        public Gq_graficoDto() : base()
        {
        }

        public Gq_graficoDto(Gq_grafico value) : base(value)
        {
        }

        public override Gq_graficoDto SetEntity(Gq_grafico value)
        {
            var result = base.SetEntity(value);

            if (result != null && result.TipoId > 0)
            {
                var tipo = Services.Get<ServGq_tipos_grafico>().findById(result.TipoId);
                if (tipo != null)
                {
                    result.TipoNombre = tipo.Nombre;
                }
            }

            if (result != null && result.TipoSeleccion > 0)
            {
                switch (result.TipoSeleccion)
                {
                    case 1: result.TipoSeleccionNombre = "Nodo";
                        break;
                    case 2: result.TipoSeleccionNombre = "Linea";
                        break;
                    default:
                        result.TipoSeleccionNombre = "";
                        break;
                }               
            }

            return result;
        }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override long TipoId { get; set; }
                
        public override string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Template { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Scritp { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string CodeSharp { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Folder { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Estado { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override long TipoSeleccion { get; set; }
                
        public string TipoNombre { get; set; }
               
        public string TipoSeleccionNombre { get; set; }

    }
}
