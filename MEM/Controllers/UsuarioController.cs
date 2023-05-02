using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.dto;
using GQService.com.gq.encriptation;
using GQService.com.gq.exception;
using GQService.com.gq.log;
using GQService.com.gq.menu;
using GQService.com.gq.paging;
using GQService.com.gq.security;
using GQService.com.gq.service;
using GQService.com.gq.utils;
using GQService.com.gq.validate;
using MEM.Helper;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class UsuarioController : BaseController, IABM<Gq_usuariosDto>
    {        
        [MenuDescription("20-00-30", "Usuarios", MEM.com.gq.security.Security.MENU_ADMINISTACION_USUARIO)]
        [SecurityDescription("Administración de Usuarios - Usuarios - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{done}")]
        public IActionResult MisDatos(string done)
        {
            ViewData["done"] = done;
            return PartialView("Index");
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_NO);
            paging.Apply<Gq_usuarios, Gq_usuariosDto>(query);
            return paging;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IEnumerable<Gq_perfilesDto> GetPerfiles()
        {
            var perfiles = Services.Get<ServGq_perfiles>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO);
            Gq_perfilesDto perfilLog = com.gq.security.Security.getPerfilUserLogueado();
            return new Gq_perfilesDto().SetEntity(perfiles.ToList());
        }

        [SecurityDescription("Administración de Usuarios - Usuarios - 02 -Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_usuariosDto model)
        {
            ReturnData result = new ReturnData();
            string claveSinEncrip = "";
            try
            {
                var resultsValidation = new List<ValidationResult>();
               
                if (ValidateUtils.TryValidateModel(model, resultsValidation))
                {
                    using (var transaction = Services.session.BeginTransaction())
                    {
                        try
                        {
                            //editar password
                            if (model.UsuarioId != null)
                            {
                                var user = Services.Get<ServGq_usuarios>(Services.statelessSession).findById(model.UsuarioId);                                
                                if (user != null && model.Clave.Equals(model.ClaveChequed) && !model.Clave.Equals(user.Clave))
                                {
                                    claveSinEncrip = model.Clave;
                                    model.Clave = Encriptacion.Encriptar(model.Clave, Constantes.CLAVE_ENCRIPTACION);
                                }
                            }                            

                            var entity = model.GetEntity();
                            //temporal
                            if (entity.PerfilId == 0) entity.PerfilId = 1;

                            //control grupo empresario
                            if (MEM.com.gq.security.Security.IsAdminPerfil(entity.PerfilId))
                            {
                                entity.GrupoEmpresario = -1;
                            }



                            if (entity.UsuarioId == null)
                            {
                                if (IsUniqueUser(entity.Usuario, 0) == false)
                                {
                                    result.data = "El usuario <strong>" + entity.Usuario + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else if (IsUniqueMail(entity.Email, 0) == false)
                                {
                                    result.data = "El mail <strong>" + entity.Email + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else
                                {
                                    entity.Creado = DateTime.Now;
                                    entity.CreadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    entity.Modificado = DateTime.Now;
                                    entity.ModificadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    entity.RequiereClave = "S";
                                    Services.Get<ServGq_usuarios>(Services.statelessSession).Agregar(entity);
                                }
                            }
                            else
                            {
                                if (IsUniqueUser(entity.Usuario, (long)entity.UsuarioId) == false)
                                {
                                    result.data = "El usuario <strong>" + entity.Usuario + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else if (IsUniqueMail(entity.Email, (long)entity.UsuarioId) == false)
                                {
                                    result.data = "El mail <strong>" + entity.Email + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else {
                                    entity.Modificado = DateTime.Now;
                                    entity.ModificadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    Services.Get<ServGq_usuarios>(Services.statelessSession).Actualizar(entity);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Usuarios - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Usuario.";
                            transaction.Rollback();
                        }

                        if (!result.isError)
                            transaction.Commit();
                        else
                            transaction.Rollback();

                    }
                }
                else
                {
                    result.data = resultsValidation;
                    result.isError = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Usuario.";
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public Boolean IsUniqueUser([FromBody]string user, [FromBody]long userId)
        {
            try
            {
                return !Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.Usuario == user && x.UsuarioId != userId).Any();
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - IsUniqueUser", ex);
                return false;
            }
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public Boolean IsUniqueMail([FromBody]string mail, [FromBody]long userId )
        {
            try
            {
                return !Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.Email == mail && x.UsuarioId != userId).Any();
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - IsUniqueMail", ex);
                return false;
            }
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public Boolean RecuperarClave([FromBody]string value)
        {
            try
            {
                var usr = Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.Email.ToLower() == value.ToLower()).FirstOrDefault();
                if (usr != null)
                {
                    return RecoverClave(usr);
                }
                else
                {
                    usr = Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.Usuario.ToLower() == value.ToLower()).FirstOrDefault();
                    if (usr != null)
                    {
                        return RecoverClave(usr);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - RecuperarClave", ex);
                return false;
            }
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public ReturnData ClaveRecuperada([FromBody]Gq_usuariosDto model)
        {
            ReturnData result = new ReturnData();
            try
            {
                if (model != null)
                {
                    if (model.Clave.Equals(model.ClaveChequed))
                    {
                        model.Clave = Encriptacion.Encriptar(model.Clave, Constantes.CLAVE_ENCRIPTACION);
                        var entity = model.GetEntity();

                        List<string> emails = new List<string> { entity.Email };

                        entity.Clave = model.Clave;
                        entity.RequiereClave = "N";

                        using (var transaction = Services.session.BeginTransaction())
                        {
                            try
                            {
                                Services.Get<ServGq_usuarios>(Services.statelessSession).Actualizar(entity);
                            }
                            catch
                            {

                            }
                            transaction.Commit();
                        }

                        result.data = new Gq_usuariosDto().makeDto(entity);

                        /*if (MailHelper.Enviar_AvisoDeMOdClaveOK(entity))
                        {
                            return result;
                        }
                        else
                        {
                            //ToDo: avisar que no se envio el mail
                        }*/
                    }
                    else
                    {
                        result.data = null;
                        result.isError = true;
                    }
                }
                else
                {
                    result.data = null;
                    result.isError = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Usuario - ClaveRecuperada", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentarrecuparar clave.";
            }
            return result;
        }

        private bool RecoverClave(Gq_usuarios entity)
        {
            try
            {
                string newPass = UtilsFunctions.CreateRandomCode(6);
                if (entity != null)
                {
                    
                    List<string> emails = new List<string> { entity.Email };

                    entity.RequiereClave = "S";
                    entity.Clave = Encriptacion.Encriptar(newPass, Constantes.CLAVE_ENCRIPTACION);
                    using (var transaction = Services.session.BeginTransaction())
                    {
                        try
                        {
                            Services.Get<ServGq_usuarios>(Services.statelessSession).Actualizar(entity);
                        }
                        catch
                        {

                        }
                        transaction.Commit();
                    }
                }
                return MailHelper.Enviar_RecuperarClave(entity, newPass);
            }
            catch (Exception ex)
            {
                Log.Error("Usuario - RecoverClave", ex);
                return false;
            }
        }

        [SecurityDescription("Administración de Usuarios - Usuarios - 03 -Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_usuariosDto model)
        {
            ReturnData result = new ReturnData();
            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    //borrar escenarios asociados
                    var borrado = BorrarEscenariosAsociados((long)model.UsuarioId);

                    if (!borrado)
                    {
                        Log.Error("Usuarios - BorrarEscenariosAsociados " + model.UsuarioId, new Exception ());
                    }

                    var entity = Services.Get<ServGq_usuarios>(Services.statelessSession).findById(model.UsuarioId);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_usuarios>(Services.statelessSession).Borrar(entity);                    
                }
                catch (Exception ex)
                {
                    Log.Error("Usuarios - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar Usuario.";
                }
            }

            return result;
        }

        private bool BorrarEscenariosAsociados(long UserId) {
            var result = false;

            try
            {
                var escenarios = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.CreadoPor == UserId);
               
                var controller = new EscenarioController();
                foreach (var escenario in escenarios)
                {
                    controller.Borrar(new Gq_escenariosDto(escenario));
                }
                result = true;
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - BorrarEscenariosAsociados", ex);
                
            }           

            return result;
        }
        
        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarGrupoEmpresarioAll()
        {
            try
            {
                var sql = Services.session.CreateSQLQuery("SELECT GrupoEmpresarioId as Id, Nombre as Label from GQ_GrupoEmpresario  order by Nombre");
                sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));

                ComboBoxDto fistValue = new ComboBoxDto { Id = "", Label = "Todos" };
                var result = sql.List<ComboBoxDto>();
                result.Insert(0, fistValue);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - BuscarGrupoEmpresarioAll", ex);
                return null;
            }
          
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarGrupoEmpresarioToAdd()
        {
            try
            {
                var sql = Services.session.CreateSQLQuery("SELECT GrupoEmpresarioId as Id, Nombre as Label from GQ_GrupoEmpresario  order by Nombre");
                sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return sql.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Usuarios - BuscarGrupoEmpresarioToAdd", ex);
                return null;
            }
           
        }

    }
}