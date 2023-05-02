using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.encriptation;
using GQService.com.gq.log;
using GQService.com.gq.security;
using GQService.com.gq.service;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
    public class LoginController : BaseController
    {
        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public IActionResult Redirected()
        {
            Security.usuarioLogueado = null;
            return View();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public IActionResult Index()
        {
            Security.usuarioLogueado = null;
            return View();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public ReturnData Login([FromBody] Gq_usuariosDto data)
        {
            var result = new ReturnData();

            try
            {
                Log.Info("Login " + data?.Usuario ?? "");
                Security.usuarioLogueado = null;

                var user = Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => (x.Usuario == data.Usuario || x.Email == data.Usuario) && (x.Clave == Encriptacion.Encriptar(data.Clave, Constantes.CLAVE_ENCRIPTACION) || x.Clave == data.Clave)).FirstOrDefault();

                if (user != null)
                {
                    //Controlar perfil de acceso
                    var perfil = Services.Get<ServGq_perfiles>(Services.statelessSession).findById(user.PerfilId);
                    if (perfil == null || perfil.Estado == Constantes.ESTADO_DESACTIVO || user.Estado == Constantes.ESTADO_DESACTIVO)
                    {
                        result.data = "perfil";
                        return result;
                    }

                    //Ingresar nueva clave
                    if (user.RequiereClave == "S" && !string.IsNullOrEmpty(data.ClaveNew))
                    {
                        user.Clave = Encriptacion.Encriptar(data.ClaveNew, Constantes.CLAVE_ENCRIPTACION);
                        user.Modificado = DateTime.Now;
                        user.ModificadoPor = user.UsuarioId;
                        user.RequiereClave = "N";
                        Services.Get<ServGq_usuarios>(Services.statelessSession).Actualizar(user);
                    }

                    //Ingresar al menu
                    if (user.RequiereClave == "S" && string.IsNullOrEmpty(data.ClaveNew))
                    {
                        result.data = "clave";
                        return result;
                    }
                    else
                    {
                        user.LoginKey = Guid.NewGuid().ToString();
                        Services.Get<ServGq_usuarios>(Services.statelessSession).Actualizar(user);

                        MEM.com.gq.security.Security.AddUsuarioLogin(user);

                        Security.usuarioLogueado = new Gq_usuariosDto().SetEntity(user);
                    }

                }

                result.data = (Gq_usuariosDto)Security.usuarioLogueado;
                result.isError = result.data == null;
            }
            catch (Exception ex)
            {
                Log.Error("Login", ex);
                result.isError = true;
                result.data = "Ocurrió un error en el Login.";
            }

            return result;
        }



        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public bool Logout()
        {
            Security.usuarioLogueado = null;
            return true;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        [Route("[controller]/[action]/{value}")]
        public bool RecuperarClave(string value)
        {
            var controller = new UsuarioController();
            return controller.RecuperarClave(value);
        }
    }
}
