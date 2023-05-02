using GQDataService.com.gq.domain;
using GQDataService.com.gq.dto;
using GQDataService.com.gq.service;
using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.exception;
using GQService.com.gq.log;
using GQService.com.gq.menu;
using GQService.com.gq.paging;
using GQService.com.gq.security;
using GQService.com.gq.service;
using GQService.com.gq.validate;
using MEMDataService.com.gq.constantes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEM.Controllers
{

    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class MailTemplateController : BaseController, IABM<Gq_mailTemplateDto>
    {
        [MenuDescription("30-00-50", "Mail Template", MEM.com.gq.security.Security.MENU_CONFIGURACION_AVANZADA)]
        [SecurityDescription("Configuración Avanzada - MailTemplate - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_mailTemplate>().findBy(x => x.Estado != Constantes.ESTADO_BORRADO);
            paging.Apply<Gq_mailTemplate, Gq_mailTemplateDto>(query);
            return paging;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{id}")]
        public Gq_mailTemplateDto GetMailTemplate(long id)
        {
            try
            {
                Gq_mailTemplateDto model = new Gq_mailTemplateDto().SetEntity(Services.Get<ServGq_mailTemplate>().findById(id));
                if (!string.IsNullOrWhiteSpace(model.Folder))
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();

                    using (var csRead = System.IO.File.OpenText(dir + "\\wwwroot\\mailTemplate\\" + model.Folder + "\\mailTemplate.cs"))
                    {
                        model.CodeSharp = csRead.ReadToEnd();
                    }

                    using (var htmlRead = System.IO.File.OpenText(dir + "\\wwwroot\\mailTemplate\\" + model.Folder + "\\mailTemplate.html"))
                    {
                        model.Template = htmlRead.ReadToEnd();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error("GetMailTemplate", ex);
                return null;
            }
            
        }

        [SecurityDescription("Configuración Avanzada - MailTemplate - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_mailTemplateDto model)
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

                            if (!string.IsNullOrWhiteSpace(entity.Folder))
                            {
                                entity.CodeSharp = "";
                                entity.Template = "";
                            }

                            if (entity.Id == null || entity.Id.Equals(0))
                            {
                                entity.Estado = Constantes.ESTADO_ACTIVO;
                                entity.CreadoPor = (long)MEM.com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity = Services.Get<ServGq_mailTemplate>().Agregar(entity);
                            }
                            else
                            {
                                entity.Modificado = DateTime.Now;
                                entity.ModificadoPor = (long)MEM.com.gq.security.Security.usuarioLogueado.UsuarioId;
                                Services.Get<ServGq_mailTemplate>().Actualizar(entity);
                            }

                            if (!string.IsNullOrWhiteSpace(entity.Folder))
                            {
                                var dir = System.IO.Directory.GetCurrentDirectory();
                                if (!System.IO.Directory.Exists(dir + "\\wwwroot\\mailTemplate\\" + model.Folder))
                                {
                                    System.IO.Directory.CreateDirectory(dir + "\\wwwroot\\mailTemplate\\" + model.Folder);
                                }
                                System.IO.File.WriteAllText(dir + "\\wwwroot\\mailTemplate\\" + model.Folder + "\\mailTemplate.cs", model.CodeSharp);
                                System.IO.File.WriteAllText(dir + "\\wwwroot\\mailTemplate\\" + model.Folder + "\\mailTemplate.html", model.Template);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("MailTemplate - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Mail Template.";
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
                Log.Error("MailTemplate - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Mail Template.";
            }
            return result;
        }

        [SecurityDescription("Configuración Avanzada - MailTemplate - 03 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_mailTemplateDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_mailTemplate>(Services.statelessSession).findById(model.Id);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_mailTemplate>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error("MailTemplate - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar Mail Template.";
                }
            }
            return result;
        }
    }
}
