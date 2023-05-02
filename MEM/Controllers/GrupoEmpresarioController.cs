using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.exception;
using GQService.com.gq.log;
using GQService.com.gq.menu;
using GQService.com.gq.paging;
using GQService.com.gq.security;
using GQService.com.gq.service;
using GQService.com.gq.validate;
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
    public class GrupoEmpresarioController : BaseController, IABM<Gq_grupoEmpresarioDto>
    {

        // GET: /<controller>/
        [MenuDescription("20-00-20", "Grupo Empresario", MEM.com.gq.security.Security.MENU_ADMINISTACION_USUARIO)]
        [SecurityDescription("Administración de Usuarios - Grupo Empresario - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).findBy(x => x.GrupoEmpresarioId > 0);
            paging.Apply<Gq_grupoEmpresario, Gq_grupoEmpresarioDto>(query);
            return paging;
        }

        [SecurityDescription("Administración de Usuarios - Grupo Empresario - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_grupoEmpresarioDto model)
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
                            if (entity.Limite < 1 || entity.Limite > 10)
                            {
                                entity.Limite = 5;
                            }

                            if (entity.GrupoEmpresarioId == null)
                            {
                                if (IsUniqueGrupo(entity.Nombre, 0) == false)
                                {
                                    result.data = "El Grupo Economico <strong>" + entity.Nombre + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else
                                {
                                    entity.Estado = "A";
                                    entity.Creado = DateTime.Now;
                                    entity.CreadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    entity.Modificado = DateTime.Now;
                                    entity.ModificadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).Agregar(entity);
                                }
                            }
                            else
                            {
                                if (IsUniqueGrupo(entity.Nombre, (long)entity.GrupoEmpresarioId) == false)
                                {
                                    result.data = "El Grupo Economico <strong>" + entity.Nombre + "</strong> ya existe";
                                    result.isError = true;
                                }
                                else
                                {
                                    entity.Modificado = DateTime.Now;
                                    entity.ModificadoPor = com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).Actualizar(entity);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Grupo Empresario - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Grupo Empresario.";
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
                Log.Error("Grupo Empresario - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Grupo Empresario.";
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public Boolean IsUniqueGrupo([FromBody]string nombre, [FromBody]long Id)
        {
            try
            {
                return !Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).findBy(x => x.Nombre == nombre && x.GrupoEmpresarioId != Id).Any();
            }
            catch (Exception ex)
            {
                Log.Error("IsUniqueGrupo", ex);
                return false;
            }
        }


        [SecurityDescription("Administración de Usuarios - Grupo Empresario - 03 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_grupoEmpresarioDto model)
        {
            ReturnData result = new ReturnData();
            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).findById(model.GrupoEmpresarioId);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Log.Error("Grupo Empresario - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar Grupo Empresario.";
                }
            }

            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public ReturnData GetGrupoInfo()
        {
            ReturnData result = new ReturnData();

            try
            {
                var grupo = Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).findBy(x => x.GrupoEmpresarioId == com.gq.security.Security.usuarioLogueado.GrupoEmpresario).FirstOrDefault();
                var usuarios = Services.Get<ServGq_usuarios>(Services.statelessSession).findBy(x => x.GrupoEmpresario == com.gq.security.Security.usuarioLogueado.GrupoEmpresario);
                var countEscenarios = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.GrupoEmpresarioId == com.gq.security.Security.usuarioLogueado.GrupoEmpresario).Count();

                List<string> usuariosLabel = new List<string>();
                foreach (var user in usuarios)
                {
                    usuariosLabel.Add(user.Nombre + " " + user.Apellido);
                }

                result.data = new GrupoInfoDto()
                {
                    Grupo = grupo,
                    Usuarios = usuariosLabel,
                    CountEscenarios = countEscenarios
                };

            }
            catch (Exception ex)
            {
                Log.Error("Grupo Empresario - GetGrupoInfo", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar borrar Grupo Info.";
            }          

            return result;
        }
    }

    public class GrupoInfoDto
    {
        public object Grupo { get; set; }
        public object Usuarios { get; set; }
        public object CountEscenarios { get; set; }
    }
}