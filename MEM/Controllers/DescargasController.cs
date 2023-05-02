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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;



namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class DescargasController : BaseController //, IABM<Gq_pluginDto>
    {

        [MenuDescription("40-00-10", "Archivos", MEM.com.gq.security.Security.MENU_DESCARGAS)]
        [SecurityDescription("Descargas - Archivos - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI, MEM.com.gq.security.Security.ROL_USUARIO })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            try
            {
                var query = Services.Get<ServGq_descargas>(Services.statelessSession).findBy(x => x.Id > 0);
                paging.Apply<Gq_descargas, Gq_descargasDto>(query);
                return paging;
            }
            catch (Exception ex)
            {
                Log.Error("Descargas - Buscar", ex);
            }
            return null;
        }

        [SecurityDescription("Descargas - Archivos - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        [Consumes("multipart/form-data")]
        public ReturnData Guardar()
        {
            ReturnData result = new ReturnData();
            try
            {
                StringValues json = "";
                Request.Form.TryGetValue("JsonOject", out json);
                Gq_descargasDto model = Newtonsoft.Json.JsonConvert.DeserializeObject<Gq_descargasDto>(json.ToString());
                var resultsValidation = new List<ValidationResult>();
                if (ValidateUtils.TryValidateModel(model, resultsValidation))
                {
                    using (var transaction = Services.session.BeginTransaction())
                    {
                        try
                        {
                            var entity = model.GetEntity();

                            if (string.IsNullOrWhiteSpace(entity.Folder))
                            {
                                entity.Folder = Guid.NewGuid().ToString();
                            }
                            if (Request.Form.Files.Count > 0)
                            {
                                entity.NombreArchivo = Request.Form.Files[0].FileName;
                            }                                

                            if (entity.Id == null)
                            {
                                entity.Estado = Constantes.ESTADO_ACTIVO;
                                entity.CreadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity.Modificado = DateTime.Now;                                
                                entity = Services.Get<ServGq_descargas>(Services.statelessSession).Agregar(entity);
                            }
                            else
                            {
                                entity.Modificado = DateTime.Now;
                                entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                Services.Get<ServGq_descargas>(Services.statelessSession).Actualizar(entity);
                            }

                            if (Request.Form.Files.Count > 0)
                            {
                                var f = Request.Form.Files[0];

                                var path = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\descargas\\" + entity.Folder + "\\";

                                if (!System.IO.Directory.Exists(path))
                                {
                                    System.IO.Directory.CreateDirectory(path);
                                }

                                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                                foreach (FileInfo file in di.GetFiles())
                                {
                                    file.Delete();
                                }

                                using (var fw = System.IO.File.Create(path + f.FileName))
                                {
                                    f.CopyTo(fw);
                                }

                                //if (f.FileName.ToLower().Contains(".zip"))
                                //{
                                //    using (ZipFile zip = new ZipFile(path + f.FileName))
                                //    {
                                //        zip.ExtractAll(path);
                                //    }
                                //}
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Descargas - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar el archivo de descarga.";
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
                Log.Error("Descargas - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar el archivo de descarga.";
            }
            return result;
        }


        private static Encoding GetEncoding(MultipartSection section)
        {
            try
            {
                MediaTypeHeaderValue mediaType;
                var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
                // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
                // most cases.
                if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
                {
                    return Encoding.UTF8;
                }
                return mediaType.Encoding;
            }
            catch (Exception ex)
            {
                Log.Error("Plugin - GetEncoding", ex);
                return null;
            }
        }


        [SecurityDescription("Descargas - Archivos - 03 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_descargasDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_descargas>(Services.statelessSession).findById(model.Id);
                    Services.Get<ServGq_descargas>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error("Descargas - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar el archivo de descarga.";
                }
            }
            return result;
        }


        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]        
        public IActionResult Descargar([FromBody]Gq_descargasDto model)
        {
            try
            {
                var path = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\descargas\\" + model.Folder + "\\" + model.NombreArchivo;
                return File(System.IO.File.OpenRead(path), "application/octet-stream");
            }
            catch (Exception ex)
            {
                Log.Error("Descargas - Descargar", ex);                
            }
            return null;
        }
    }
}