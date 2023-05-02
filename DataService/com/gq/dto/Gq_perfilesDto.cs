
using GQService.com.gq.service;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using MEMDataService.com.gq.service;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MEMDataService.com.gq.dto
{
    public class Gq_perfilesDto : _Gq_perfilesDto
    {
        public Gq_perfilesDto() : base()
        {
        }

        public Gq_perfilesDto(Gq_perfiles value) : base(value)
        {
        }

        public override Gq_perfilesDto SetEntity(Gq_perfiles value)
        {
            var result = base.SetEntity(value);

            if (result != null)
            {
                result.Accesos = new Gq_perfiles_accesosDto().SetEntity(Services.Get<ServGq_perfiles_accesos>().findBy(x => x.PerfilId == result.PerfilId).ToList());
            }

            return result;
        }

        public IEnumerable<Gq_perfiles_accesosDto> Accesos { get; set; } = new List<Gq_perfiles_accesosDto>();

        [Required(ErrorMessage = "Campo requerido")]
        public override System.String Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override System.String Estado { get; set; }

        public string ModificadoPorNombre
        {
            get
            {
                if (this.ModificadoPor.HasValue && this.ModificadoPor.Value > 0)
                {
                    return Services.Get<ServGq_usuarios>().findById(this.ModificadoPor.Value).Nombre;
                }
                else return "";

            }
        }
    }
}
