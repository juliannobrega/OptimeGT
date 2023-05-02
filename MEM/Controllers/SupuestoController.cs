using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.dto;
using GQService.com.gq.exception;
using GQService.com.gq.log;
using GQService.com.gq.menu;
using GQService.com.gq.paging;
using GQService.com.gq.security;
using GQService.com.gq.service;
using GQService.com.gq.validate;
using MEM.com.gq.supuestos;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using static MEM.com.gq.supuestos.ProcesarSupuesto;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class SupuestoController : BaseController, IABM<Gq_supuestoDto>
    {
        public static Dictionary<string, SupuestoInfo> SupuestosChech { get; private set; } = new Dictionary<string, SupuestoInfo>();

        /// <summary>
        /// Tabla LIcitacion
        /// </summary>
        /// <returns>Reutilizamos Supuestos de AES, porque hacen lo mismo</returns>

        [MenuDescription("30-00-30", "Tablas Licitación", MEM.com.gq.security.Security.MENU_CONFIGURACION_AVANZADA)]
        [SecurityDescription("Configuración Avanzada - Tablas Licitación - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            var query = Services.Get<ServGq_supuesto>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO && x.Folder != "sup_ImpExpTodos");
            paging.Apply<Gq_supuesto, Gq_supuestoDto>(query);
            return paging;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{id}")]
        public Gq_supuestoDto GetSupuesto(long id)
        {
            try
            {
                Gq_supuestoDto model = new Gq_supuestoDto();

                model = new Gq_supuestoDto().SetEntity(Services.Get<ServGq_supuesto>(Services.statelessSession).findBy(x => x.Folder == "sup_ImpExpTodos")).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(model.Folder))
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();

                    using (var csRead = System.IO.File.OpenText(dir + "\\wwwroot\\supuestos\\" + model.Folder + "\\supuesto.cs"))
                    {
                        model.CodeSharp = csRead.ReadToEnd();
                    }

                    using (var jsRead = System.IO.File.OpenText(dir + "\\wwwroot\\supuestos\\" + model.Folder + "\\supuesto.js"))
                    {
                        model.Scritp = jsRead.ReadToEnd();
                    }

                    using (var htmlRead = System.IO.File.OpenText(dir + "\\wwwroot\\supuestos\\" + model.Folder + "\\supuesto.html"))
                    {
                        model.Template = htmlRead.ReadToEnd();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error("Supuesto - GetSupuesto", ex);
                return null;
            }
            
        }

        [SecurityDescription("Configuración Avanzada - Tablas Licitación - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_supuestoDto model)
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
                                
                                if (IsUniqueFolder(entity.TablaMod, 0) == false)
                                {
                                    result.data = "La tabla strong>" + entity.Folder + "</strong> ya existe";
                                    result.isError = true;
                                    return result;
                                }
                                entity.CodeSharp = "";
                                entity.Scritp = "";
                                entity.Template = "";
                                entity.Grupo = 0;
                                entity.Folder = model.TablaMod;
                                entity.TablaModNombre = model.TablaMod;
                                entity.Estado = Constantes.ESTADO_ACTIVO;
                                entity.CreadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity.Modificado = DateTime.Now;
                                entity = Services.Get<ServGq_supuesto>(Services.statelessSession).Agregar(entity);
                            }
                            else
                            {
                                if (IsUniqueFolder(entity.Folder, (long)entity.Id) == false)
                                {
                                    result.data = "El folder <strong>" + entity.Folder + "</strong> ya existe";
                                    result.isError = true;
                                    return result;
                                }

                                entity.Modificado = DateTime.Now;
                                entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                Services.Get<ServGq_supuesto>(Services.statelessSession).Actualizar(entity);
                            }                           

                            transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            Log.Error("Supuestos - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Supuesto.";
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
                Log.Error("Supuestos - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Supuesto.";
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.Desactivo)]
        public Boolean IsUniqueFolder([FromBody]string folder, [FromBody]long Id)
        {
            try
            {
                return !Services.Get<ServGq_supuesto>(Services.statelessSession).findBy(x => x.Folder == folder && x.Id != Id).Any();
            }
            catch (Exception ex)
            { 
                Log.Error("Supuestos - IsUniqueFolder", ex);               
                return false;
            }
        }

        [SecurityDescription("Configuración Avanzada - Tablas Licitación - 03 -Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_supuestoDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_supuesto>(Services.statelessSession).findById(model.Id);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_supuesto>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error("Supuestos - Borrar", ex);
                    result.isError = true;
                    result.data = "Ocurrió un error al intentar borrar Supuesto.";
                }
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public dynamic Ejecutar([FromBody]EjecutarDto model)
        {
            try
            {
                dynamic result = null;
                result = ProcesarSupuesto.Ejecutar(model);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - Ejecutar", ex);
                return null;
            }
          
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Consumes("multipart/form-data")]
        public dynamic EjecutarUpload()
        {
            try
            {
                dynamic result = null;

                StringValues json = "";
                Request.Form.TryGetValue("JsonOject", out json);
                EjecutarDto model = JsonConvert.DeserializeObject<EjecutarDto>(json.ToString());
                string filename = "";
                if (Request.Form.Files.Count > 0)
                {
                    filename = Path.GetTempFileName();
                    using (var stream = System.IO.File.Create(filename))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                }

                var param = new List<object>();
                param.AddRange(model.Parametros);
                param.Add(filename);
                model.Parametros = param.ToArray();
                result = ProcesarSupuesto.Ejecutar(model);

                if(result.data is SupuestoInfo)
                {
                    var item = result.data as SupuestoInfo;
                    SupuestosChech.Add(item.Id, item);
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - EjecutarUpload", ex);
                return null;
            }
            
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{id}")]
        public dynamic SupuestoInfo(string id)
        {
            if(SupuestosChech.ContainsKey(id))
            {
                return SupuestosChech[id];
            }
            return null;
        }


        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarSupuestos()
        {
            try
            {
                var sql = Services.session.CreateSQLQuery(string.Format(@"
                                                    SELECT Id, Nombre as Label from gq_supuesto WHERE Estado = '{0}'",
                                                    Constantes.ESTADO_ACTIVO));
                sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return sql.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarSupuestos", ex);
                return null;
            }
            
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{schema},{grupo}")]
        public IList<ComboBoxDto> BuscarSupuestos(string schema, int grupo)
        {
            try
            {
                var sql = Services.session.CreateSQLQuery(string.Format(@"SELECT * from gq_supuesto WHERE Estado = '{0}' AND Grupo = '{1}' ORDER BY Orden, Folder", Constantes.ESTADO_ACTIVO, grupo));
                var result = sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Gq_supuestoDto))).List<Gq_supuestoDto>();

                var lista = new List<ComboBoxDto>();
                var dependeLablel = string.Empty;

                foreach (var item in result)
                {
                    //caso que sea padre y no depende de nadie
                    if (item.Depende <= 0)
                    {
                        lista.Add(new ComboBoxDto { Id = item.Id, Label = item.Nombre });
                    }
                    else
                    {
                        dependeLablel = Services.Get<ServGq_supuesto>(Services.statelessSession).findById(item.Depende).Folder;
                        var values = Services.session.CreateSQLQuery(string.Format(@"SELECT count(1) from  `{0}`.{1}", schema, dependeLablel)).UniqueResult<long>();

                        if (values > 0)
                        {
                            lista.Add(new ComboBoxDto { Id = item.Id, Label = item.Nombre });
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarSupuestos", ex);
                return null;
            }
            
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{schema},{grupo}")]
        public IList<ComboBoxDto> BuscarMods(string schema, int grupo)
        {
            try
            {
                var sql = Services.session.CreateSQLQuery(string.Format(@"SELECT * from gq_supuesto 
                                                                        WHERE Estado = '{0}' 
                                                                        AND Grupo = '{1}' 
                                                                        AND TablaMod != ''
                                                                        ORDER BY Orden, Folder", Constantes.ESTADO_ACTIVO, grupo));
                var result = sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Gq_supuestoDto))).List<Gq_supuestoDto>();

                var lista = new List<ComboBoxDto>();
                foreach (var item in result)
                {
                    lista.Add(new ComboBoxDto { Id = item.Id, Label = item.TablaModNombre });
                }

                return lista;
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarMods", ex);
                return null;
            }
           
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarGrupos()
        {
            try
            {
                var s = Services.session.CreateSQLQuery("SELECT Id, Nombre as Label from gq_supuesto_grupo ");
                s.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return s.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarGrupos", ex);
                return null;
            }
           
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarGruposMod()
        {
            try
            {
                var s = Services.session.CreateSQLQuery(@"SELECT distinct supgrup.Id, supgrup.Nombre as Label 
                                                    from gq_supuesto_grupo as supgrup
                                                    inner join gq_supuesto as sup on supgrup.Id = sup.Grupo
                                                    where sup.TablaMod != ''
                                                    order by supgrup.Id
                                                    ");
                s.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return s.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarGruposMod", ex);
                return null;
            }
            
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{grupo},{supuestoId}")]
        public IList<ComboBoxDto> BuscarDepende(int grupo, int supuestoId)
        {
            try
            {
                var sql = Services.session.CreateSQLQuery(string.Format("SELECT Id, Nombre as Label from gq_supuesto where grupo = {0} and Id != {1} order by Folder", grupo, supuestoId));
                sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                var result = sql.List<ComboBoxDto>();
                ComboBoxDto fistValue = new ComboBoxDto { Id = "0", Label = "(ninguno)" };

                result.Insert(0, fistValue);
                return result;

            }
            catch (Exception ex)
            {
                Log.Error("Supuestos - BuscarDepende", ex);
                return null;
            }
            
        }
    }
}
