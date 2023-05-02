
using GQService.com.gq.encriptation;
using GQService.com.gq.service;
using GQService.com.gq.validate;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto.codegen;
using MEMDataService.com.gq.service;
using System;
using System.ComponentModel.DataAnnotations;

namespace MEMDataService.com.gq.dto
{
    public class Gq_usuariosDto : _Gq_usuariosDto
    {
        public Gq_usuariosDto():base()
        {
        }
       
        public Gq_usuariosDto(Gq_usuarios value):base(value)
        {
            
        }

        public override Gq_usuariosDto SetEntity(Gq_usuarios value)
        {
            var dto = base.SetEntity(value);
            if (dto.GrupoEmpresario > 0)
            {
                try
                {
                    dto.GrupoLabel = Services.Get<ServGq_grupoEmpresario>().findById(dto.GrupoEmpresario).Nombre;
                }
                catch { }
            }
            return dto;
        }


        [Required(ErrorMessage = "Campo requerido")]
        public override string Usuario { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override string Apellido { get; set; }

        [Required(ErrorMessage = "Campo requerido con formato email")]
        public override string Email { get; set; }

        [Required(ErrorMessage = "La clave debe contener al menos una letra minúscula, una letra mayúscula  y un número, y debe tener una longitud mínima de 8")]
        public override string Clave { get; set; }

        [FunctionValidator(typeof(Gq_usuariosDto), "PasswordValidar")]
        [Required(ErrorMessage = "Las claves deben ser iguales")]
        public object ClaveChequed { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public override Int64 GrupoEmpresario { get; set; }

        public string GrupoLabel { get; set; }

        public static bool PasswordValidar(object value, object ObjectInstance)
        {
            Gq_usuariosDto data = (Gq_usuariosDto)ObjectInstance;
            if (string.IsNullOrWhiteSpace(data.Clave) == false)
            {
                if (data.Clave.Equals(data.ClaveChequed))
                {
                    return true;
                }
                else if (Encriptacion.Desencriptar(data.Clave, Constantes.CLAVE_ENCRIPTACION).Contains("Wrong Input. ") == false)
                {
                    return true;
                }
            }

            return false;
        }

        public string ClaveNew { set; get; }

        //public string ModificadoPorNombre
        //{
        //    get
        //    {
        //        if (this.ModificadoPor.HasValue && this.ModificadoPor.Value > 0)
        //        {
        //            return Services.Get<ServGq_usuarios>().findById(this.ModificadoPor.Value).Nombre;
        //        }
        //        else return "";
            
        //    }
        //}
    }
}
