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

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class SMTPController : BaseController, IABM<Gq_smtp_configDto>
    {
        [MenuDescription("30-00-40", "SMTP", MEM.com.gq.security.Security.MENU_CONFIGURACION_AVANZADA)]
        [SecurityDescription("Configuración Avanzada - SMTP - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_smtp_config>().findBy(x => x.Id >= 0 );
            paging.Apply<Gq_smtp_config, Gq_smtp_configDto>(query);
            return paging;
        }

        [SecurityDescription("Configuración Avanzada - SMTP - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_smtp_configDto model)
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
                            if (entity.Id == null)
                            {
                                entity = Services.Get<ServGq_smtp_config>().Agregar(entity);
                            }
                            else
                            {
                                Services.Get<ServGq_smtp_config>().Actualizar(entity);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("SMTP - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar SMTP.";
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
                Log.Error("SMTP - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar SMTP.";
            }
            return result;
        }

        [SecurityDescription("Configuración Avanzada - SMTP - 03 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_smtp_configDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_smtp_config>().findById(model.Id);
                    Services.Get<ServGq_smtp_config>().Actualizar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error("SMTP - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar SMTP.";
                }
            }
            return result;
        }

    }
}
