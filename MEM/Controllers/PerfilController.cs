using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.log;
using GQService.com.gq.menu;
using GQService.com.gq.paging;
using GQService.com.gq.security;
using GQService.com.gq.service;
using GQService.com.gq.validate;
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
    public class PerfilController : BaseController, IABM<Gq_perfilesDto>
    {
        [MenuDescription("20-00-10", "Perfiles", MEM.com.gq.security.Security.MENU_ADMINISTACION_USUARIO)]
        [SecurityDescription("Administración de Usuarios - Perfiles - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_perfiles>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO);
            paging.Apply<Gq_perfiles, Gq_perfilesDto>(query);
            return paging;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IEnumerable<Gq_accesosDto> GetAccesos()
        {
            try
            {
                return new Gq_accesosDto().SetEntity(Services.Get<ServGq_accesos>(Services.statelessSession).findBy().OrderBy(acceso => acceso.Nombre));
            }
            catch (Exception ex)
            {
                Log.Error("Perfiles - GetAccesos", ex);
                return null;
            }
           
        }

        [SecurityDescription("Administración de Usuarios - Perfiles - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_perfilesDto model)
        {
            ReturnData result = new ReturnData();
            try
            {
                var resultsValidation = new List<ValidationResult>();
                if (ValidateUtils.TryValidateModel(model, resultsValidation))
                {
                    using (var transaction = Services.session.BeginTransaction())
                    {
                        try
                        {
                            var entity = model.GetEntity();
                            if (entity.PerfilId == null || entity.PerfilId.Equals(0))
                            {
                                entity.CreadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity = Services.Get<ServGq_perfiles>(Services.statelessSession).Agregar(entity);
                            }
                            else
                            {
                                entity.Modificado = DateTime.Now;
                                entity.ModificadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                Services.Get<ServGq_perfiles>(Services.statelessSession).Actualizar(entity);
                            }

                            #region Búsqueda y eliminación y creación de PerfilesAcesos

                            var accesosDePerfil = Services.Get<ServGq_perfiles_accesos>(Services.statelessSession).findBy(x => x.PerfilId == entity.PerfilId).ToList();
                            Services.Get<ServGq_perfiles_accesos>(Services.statelessSession).Borrar(accesosDePerfil);

                            foreach (var item in model.Accesos)
                            {
                                item.PerfilId = entity.PerfilId.Value;
                                item.Estado = Constantes.ESTADO_ACTIVO;
                                item.CreadoPor = entity.CreadoPor;
                                item.Creado = entity.Creado;
                                item.Modificado = entity.Modificado;
                                item.ModificadoPor = entity.ModificadoPor;
                            }

                            List<Gq_perfiles_accesos> perfilesAccesos = new Gq_perfiles_accesosDto().GetEntity(model.Accesos).ToList();
                            Services.Get<ServGq_perfiles_accesos>(Services.statelessSession).Agregar(perfilesAccesos);

                            #endregion

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Perfiles - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Perfil.";
                            transaction.Rollback();
                        }
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
                Log.Error("Perfiles - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Perfil.";
            }
            return result;
        }

        [SecurityDescription("Administración de Usuarios - Perfiles - 03 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_perfilesDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_perfiles>(Services.statelessSession).findById(model.PerfilId);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_perfiles>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error("Perfiles - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar Perfil.";
                }
            }
            return result;
        }


        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{IdPerfil}")]
        public bool IsAdminPerfil(long IdPerfil) {
            return MEM.com.gq.security.Security.IsAdminPerfil(IdPerfil);
        }

       
    }
}
