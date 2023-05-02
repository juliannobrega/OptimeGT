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
using MEM.com.gq.plugins;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class EscenarioController : BaseController, IABM<Gq_escenariosDto>
    {
        public Gq_escenarios escenarioClonado { get; private set; }

        [MenuDescription("10-00-10", "Escenarios", MEM.com.gq.security.Security.MENU_ADMINISTACION_ESCENARIO)]
        [SecurityDescription("Administración de Escenarios - Escenarios - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI, MEM.com.gq.security.Security.ROL_USUARIO })]
        public IActionResult Index()
        {
            return PartialView();
        }

        [MenuDescription("10-00-20", "Borrar Escenarios", MEM.com.gq.security.Security.MENU_ADMINISTACION_ESCENARIO)]
        [SecurityDescription("Administración de Escenarios - Escenarios - 08 - Borrar Todos los Escenarios", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult BorrarEscenarios()
        {
            return PartialView();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody] Paging paging)
        {
            try
            {
                var usuarios = Services.Get<ServGq_usuarios>().findBy(x => x.GrupoEmpresario == com.gq.security.Security.usuarioLogueado.GrupoEmpresario).Select(x => x.UsuarioId).ToList();
                var query = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO
                                                                                                && x.EsBase == Constantes.ESTADO_NO
                                                                                                && ((x.Publico == "Publico" && x.Estado == Constantes.ESTADO_ESCENARIO_GENERADO)
                                                                                                    || usuarios.Contains(x.CreadoPor)));



                paging.Order.Add(new Paging.PagingOrder { Direction = "-", Property = "Publico" });
                paging.Apply<Gq_escenarios, Gq_escenariosDto>(query);
            }
            catch (Exception ex)
            {
                Log.Error("BuscarEscenario", ex);
            }

            return paging;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IEnumerable<ComboBoxEscenarioBaseDto> GetEscenarioBase()
        {
            try
            {
                return Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.Estado == Constantes.ESTADO_ESCENARIO_GENERADO && x.EsBase == Constantes.ESTADO_SI).Select(x => new ComboBoxEscenarioBaseDto() { Id = x.Id, Label = x.Nombre, FechaIni = x.FechaInicio, FechaFin = x.FechaFin, SemanaIni = x.SemanaInicio, SemanaFin = x.SemanaFin }).ToList<ComboBoxEscenarioBaseDto>();
            }
            catch (Exception ex)
            {
                Log.Error("GetEscenarioBase", ex);
                return null;
            }

        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IEnumerable<ComboBoxDto> GetPlugins()
        {
            try
            {
                return Services.Get<ServGq_plugin>(Services.statelessSession).findBy(x => x.Tipo == "M" && x.Estado != Constantes.ESTADO_BORRADO).Select(x => new ComboBoxDto() { Id = x.Id, Label = x.Nombre }).ToList<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("GetPlugins", ex);
                return null;
            }
        }

        [SecurityDescription("Administración de Escenarios - Escenarios - 02 - Agregar ", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody] Gq_escenariosDto model)
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

                            entity.Modificado = DateTime.Now;
                            entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;

                            if (entity.Id == null)
                            {
                                //entity.BaseDatos = "mem_panama_" + Guid.NewGuid().ToString();
                                entity.BaseDatos = "optime_guatemala_" + com.gq.security.Security.usuarioLogueado.UsuarioId.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                entity.EsBase = Constantes.ESTADO_NO;
                                entity.Estado = Constantes.ESTADO_ESCENARIO_SIN_BASE;
                                entity.CreadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity.Publico = "Publico";
                                entity.GrupoEmpresarioId = -1;
                                entity = Services.Get<ServGq_escenarios>().Agregar(entity);
                            }
                            else
                            {
                                Services.Get<ServGq_escenarios>().Actualizar(entity);
                            }

                            Services.session.CreateSQLQuery(@"DROP SCHEMA IF EXISTS `" + entity.BaseDatos + @"`; 
                                                                           CREATE SCHEMA `" + entity.BaseDatos + @"`; ").ExecuteUpdate();


                            transaction.Commit();


                            //Crear modelo
                            try
                            {
                                var server = Startup.Configuration.GetSection("ConnectionDB").GetSection("Server").Value;
                                var usuario = Startup.Configuration.GetSection("ConnectionDB").GetSection("UserId").Value;
                                var password = Startup.Configuration.GetSection("ConnectionDB").GetSection("pwd").Value;

                                var plugin = Services.Get<ServGq_plugin>(Services.statelessSession).findById(entity.PluginId);

                                using (var p = PluginsLoader.GetPlugin(plugin))
                                {

                                    var Command = p.Value.Command("generartablasplanas", new object[] { server, entity.BaseDatos, usuario, password });//, (entity.FechaInicio * 100) + entity.SemanaInicio, (entity.FechaFin * 100) + entity.SemanaFin });

                                    if (Command == "OK")
                                    {
                                        entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                                    }
                                    else
                                    {
                                        entity.Estado = Constantes.ESTADO_ESCENARIO_SIN_BASE;
                                        Log.Error(this, "Generartablasplanas" + Command);
                                    }

                                    Services.Get<ServGq_escenarios>().Actualizar(entity);

                                    Log.Info(this, "Generar - End ");
                                }
                            }
                            catch (Exception ex)
                            {
                                entity.Estado = Constantes.ESTADO_ESCENARIO_SIN_BASE;
                                Log.Error(this, "Generartablasplanas", ex);
                                result.isError = true;
                                result.data = "Ocurrió un error al intentar guardar Escenario.";
                            }


                        }
                        catch (Exception ex)
                        {
                            Log.Error("Escenarios - Agregar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Escenario.";
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
                Log.Error("Escenarios - Agregar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Escenario.";
            }
            return result;
        }

        [SecurityDescription("Administración de Escenarios - Escenarios - 03 - Editar ", new string[] { MEM.com.gq.security.Security.ROL_ADMI, MEM.com.gq.security.Security.ROL_USUARIO })]
        public ReturnData Editar([FromBody] Gq_escenariosDto model)
        {
            ReturnData result = new ReturnData();
            try
            {
                //Validacion del perfil del usuario y tipo de escenario 
                if (model.Publico == "Publico")
                {
                    if (!(model.Publico == "Publico" && com.gq.security.Security.IsAdminPerfil(com.gq.security.Security.usuarioLogueado.PerfilId)))
                    {
                        result.data = "Usted no puede Editar este Escenario";
                        result.isError = true;
                        return result;
                    }
                }
                else
                {
                    if (!(model.Publico == "Privado" && !com.gq.security.Security.IsAdminPerfil(com.gq.security.Security.usuarioLogueado.PerfilId)))
                    {
                        result.data = "Usted no puede Editar este Escenario";
                        result.isError = true;
                        return result;
                    }
                }

                var resultsValidation = new List<ValidationResult>();
                if (ValidateUtils.TryValidateModel(model, resultsValidation))
                {
                    using (var transaction = Services.session.BeginTransaction())
                    {
                        try
                        {
                            var entity = model.GetEntity();

                            entity.Modificado = DateTime.Now;
                            entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;

                            var escenario = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.BaseDatos == entity.BaseDatos).FirstOrDefault();
                            entity.Estado = escenario.Estado;

                            if (entity.PluginId != escenario.PluginId && escenario.Estado != Constantes.ESTADO_ESCENARIO_SIN_BASE)
                            {
                                entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                            }

                            Services.Get<ServGq_escenarios>().Actualizar(entity);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            result.isError = true;
                            result.data = GenericError.Create(ex);
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
                Log.Error("Escenarios - Editar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar editar Escenario.";
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public bool ControlLimite()
        {
            var escenariosPorGrupo = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.GrupoEmpresarioId == com.gq.security.Security.usuarioLogueado.GrupoEmpresario);
            var grupoEmpresario = Services.Get<ServGq_grupoEmpresario>(Services.statelessSession).findById(com.gq.security.Security.usuarioLogueado.GrupoEmpresario);
            if (escenariosPorGrupo.Count() >= grupoEmpresario.Limite)
            {
                return true;
            }
            return false;
        }

        [SecurityDescription("Administración de Escenarios - Escenarios - 04 - Clonar ", new string[] { MEM.com.gq.security.Security.ROL_USUARIO })]
        public ReturnData Clonar([FromBody] Gq_escenariosDto model)
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

                            bool limite = ControlLimite();
                            if (limite)
                            {
                                result.isError = true;
                                result.data = "LIMITE";
                            }
                            else
                            {
                                //Entra aca si es clonar
                                if (model.GenerarDatos && entity.EscenarioOrigenId.HasValue && entity.Estado != Constantes.ESTADO_ESCENARIO_PROCESANDO)
                                {
                                    entity.Modificado = DateTime.Now;
                                    entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                    entity.Publico = "Privado";

                                    if (entity.Id == null)
                                    {
                                        //entity.BaseDatos = "mem_panama_" + Guid.NewGuid().ToString();
                                        entity.BaseDatos = "optime_guatemala_" + com.gq.security.Security.usuarioLogueado.UsuarioId.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                        entity.EsBase = Constantes.ESTADO_NO;
                                        entity.Estado = Constantes.ESTADO_ESCENARIO_SIN_BASE;
                                        entity.CreadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                        entity.Creado = DateTime.Now;
                                        entity.GrupoEmpresarioId = (long)com.gq.security.Security.usuarioLogueado.GrupoEmpresario;
                                        entity = Services.Get<ServGq_escenarios>().Agregar(entity);
                                    }
                                    else
                                    {
                                        Services.Get<ServGq_escenarios>().Actualizar(entity);
                                    }


                                    Services.session.CreateSQLQuery(@"DROP SCHEMA IF EXISTS `" + entity.BaseDatos + @"`; 
                                                                           CREATE SCHEMA `" + entity.BaseDatos + @"`; ").ExecuteUpdate();

                                    transaction.Commit();

                                    Log.Info(this, "Generar - Start  ");

                                    var escenarioBase = Services.Get<ServGq_escenarios>().findById(entity.EscenarioOrigenId);
                                    if (escenarioBase == null)
                                    {
                                        result.isError = true;
                                        result.data = "Seleccione un escenario base";
                                        return result;
                                    }

                                    entity.Estado = Constantes.ESTADO_ESCENARIO_PROCESANDO;

                                    Services.Get<ServGq_escenarios>().Actualizar(entity);

                                    escenarioClonado = entity;

                                    Task.Factory.StartNew(async () =>
                                    {
                                        try
                                        {
                                            await Task.Delay(500);
                                            var server = Startup.Configuration.GetSection("ConnectionDB").GetSection("Server").Value;
                                            var usuario = Startup.Configuration.GetSection("ConnectionDB").GetSection("UserId").Value;
                                            var password = Startup.Configuration.GetSection("ConnectionDB").GetSection("pwd").Value;

                                            var en = entity;

                                            var dir = System.IO.Directory.GetCurrentDirectory();

                                            Process process = new Process();
                                            ProcessStartInfo startInfo = new ProcessStartInfo();
                                            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                            startInfo.FileName = "cmd.exe";
                                            startInfo.UseShellExecute = false;
                                            startInfo.RedirectStandardOutput = true;
                                            startInfo.RedirectStandardError = true;
                                            startInfo.RedirectStandardInput = true;
                                            startInfo.ErrorDialog = false;

                                            process.StartInfo = startInfo;
                                            process.Start();

                                            using (StreamWriter sw = process.StandardInput)
                                            {
                                                if (sw.BaseStream.CanWrite)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(password))
                                                        password = " -p" + password;
                                                    sw.WriteLine("\"" + dir + "\\MySql\\mysqldump.exe\" -h " + server + " -u" + usuario + password + " " + escenarioBase.BaseDatos + " | \"" + dir + "\\MySql\\mysql.exe\" -h " + server + " -u" + usuario + password + " " + entity.BaseDatos + " ");
                                                }
                                            }

                                            process.WaitForExit();

                                            var error = process.StandardError.ReadToEnd();
                                            var salida = process.StandardOutput.ReadToEnd();

                                            Log.Info(this, "Generar - ERROR  : " + error);
                                            Log.Info(this, "Generar - SALIDA : " + salida);

                                            process.Close();

                                            if (error.ToLower().IndexOf("[error]") < 0)
                                                en.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                                            else
                                                en.Estado = Constantes.ESTADO_ESCENARIO_SIN_BASE;

                                            Services.Get<ServGq_escenarios>().Actualizar(en);

                                            Log.Info(this, "Generar - End ");
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(this, "Guardar", e);
                                        }
                                    });
                                }
                                else
                                {
                                    result.data = "El escenario no se puede clonar";
                                    result.isError = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Escenarios - Clonar", ex);
                            result.isError = true;
                            result.data = GenericError.Create(ex);
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
                Log.Error("Escenarios - Clonar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar clonar Escenario.";
            }
            return result;
        }


        [SecurityDescription("Administración de Escenarios - Escenarios - 07 - Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI, MEM.com.gq.security.Security.ROL_USUARIO })]
        public ReturnData Borrar([FromBody] Gq_escenariosDto model)
        {
            ReturnData result = new ReturnData();
            int option = 1;

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    //Validacion del perfil del usuario y tipo de escenario 
                    if (model.Publico == "Publico")
                    {
                        if (!(model.Publico == "Publico" && com.gq.security.Security.IsAdminPerfil(com.gq.security.Security.usuarioLogueado.PerfilId)))
                        {
                            result.data = "Usted no puede Borrar este Escenario";
                            result.isError = true;
                            return result;
                        }
                    }
                    else
                    {
                        if (!(model.Publico == "Privado" && !com.gq.security.Security.IsAdminPerfil(com.gq.security.Security.usuarioLogueado.PerfilId)))
                        {
                            result.data = "Usted no puede Borrar este Escenario";
                            result.isError = true;
                            return result;
                        }
                    }

                    var sql = Services.session.CreateSQLQuery(string.Format(@"DELETE FROM gq_escenarios WHERE Id= {0}", model.Id));
                    sql.ExecuteUpdate();

                    option = 2;

                    sql = Services.session.CreateSQLQuery(string.Format(@"DROP SCHEMA `{0}`", model.BaseDatos));
                    sql.ExecuteUpdate();
                }
                catch (Exception ex)
                {
                    Log.Error("Escenarios - Borrar", ex);
                    transaction.Rollback();
                    if (option == 1)
                    {
                        result.data = "Ocurrió el siguiente error al intentar borrar el escenario. Comuníquese con el Administrador.";
                    }
                    else
                    {
                        result.data = "Ocurrió el siguiente error al intentar borrar el eschema: " + model.BaseDatos + ". Comuníquese con el Administrador.";
                    }

                    result.isError = true;
                }
            }
            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public ReturnData BorrarTodos()
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var escenarios = Services.Get<ServGq_escenarios>(Services.statelessSession).findBy(x => x.Id > 0);
                    var sql = Services.session.CreateSQLQuery("");
                    foreach (var escenario in escenarios)
                    {
                        sql = Services.session.CreateSQLQuery(string.Format(@"DROP SCHEMA `{0}`", escenario.BaseDatos));
                        sql.ExecuteUpdate();

                        sql = Services.session.CreateSQLQuery(string.Format(@"DELETE FROM gq_escenarios WHERE Id= {0}", escenario.Id));
                        sql.ExecuteUpdate();
                    }
                    result.data = "OK";
                }
                catch (Exception ex)
                {
                    Log.Error("Borrar todos los escenarios", ex);
                    transaction.Rollback();
                    result.data = "Ocurrió un error al intentar borrar los escenarios.";
                    result.isError = true;
                }
            }
            return result;
        }

        [SecurityDescription("Administración de Escenarios - Escenarios - 05 - Ejecutar Modelo Full", new string[] { MEM.com.gq.security.Security.ROL_ADMI, MEM.com.gq.security.Security.ROL_USUARIO })]
        [Route("[controller]/[action]/{id}")]
        public ReturnData EjecutarModeloFull(long id)
        {
            ReturnData result = new ReturnData();
            try
            {
                var entity = Services.Get<ServGq_escenarios>(Services.statelessSession).findById(id);
                if (entity.Estado == Constantes.ESTADO_ESCENARIO_EDICION || entity.Estado == Constantes.ESTADO_ESCENARIO_GENERADO)
                {
                    entity.Estado = Constantes.ESTADO_ESCENARIO_PROCESANDO;
                    entity = Services.Get<ServGq_escenarios>(Services.statelessSession).Actualizar(entity);

                    var plugin = Services.Get<ServGq_plugin>(Services.statelessSession).findById(entity.PluginId);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var server = Startup.Configuration.GetSection("ConnectionDB").GetSection("Server").Value;
                            var usuario = Startup.Configuration.GetSection("ConnectionDB").GetSection("UserId").Value;
                            var password = Startup.Configuration.GetSection("ConnectionDB").GetSection("pwd").Value;

                            using (var p = PluginsLoader.GetPlugin(plugin))
                            {

                                var Command = p.Value.Command("ejecutarModelo", new object[] { server, entity.BaseDatos, usuario, password });//, (entity.FechaInicio * 100) + entity.SemanaInicio, (entity.FechaFin * 100) + entity.SemanaFin });

                                if (Command == "OK")
                                {
                                    entity.Estado = Constantes.ESTADO_ESCENARIO_GENERADO;
                                }
                                else
                                {
                                    entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                                    Log.Error(this, "EjecutarModelo" + Command);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                            //TODO HAY QUE VER COMO VAMOS A MOSTRAR EL ERROR
                            Log.Error(this, "EjecutarModelo", e);
                        }
                        using (var t = Services.statelessSession.BeginTransaction())
                        {
                            Services.statelessSession.Update(entity);
                            t.Commit();
                        }
                    });
                }
                else
                {
                    result.isError = true;
                    result.data = "Tiene que estar activo el escenario";
                }
            }
            catch (Exception ex)
            {
                Log.Error("Escenarios - Borrar", ex);
                result.data = "Ocurrió un error al intentar ejecutar el modelo.";
                result.isError = true;
            }

            return result;
        }


        public ReturnData GenerarMod(long id)
        {
            ReturnData result = new ReturnData();
            try
            {
                var entity = Services.Get<ServGq_escenarios>(Services.statelessSession).findById(id);
                if (entity.Estado == Constantes.ESTADO_ESCENARIO_EDICION || entity.Estado == Constantes.ESTADO_ESCENARIO_GENERADO)
                {
                    entity.Estado = Constantes.ESTADO_ESCENARIO_PROCESANDO;
                    entity = Services.Get<ServGq_escenarios>(Services.statelessSession).Actualizar(entity);

                    var plugin = Services.Get<ServGq_plugin>(Services.statelessSession).findById(entity.PluginId);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var server = Startup.Configuration.GetSection("ConnectionDB").GetSection("Server").Value;
                            var usuario = Startup.Configuration.GetSection("ConnectionDB").GetSection("UserId").Value;
                            var password = Startup.Configuration.GetSection("ConnectionDB").GetSection("pwd").Value;

                            using (var p = PluginsLoader.GetPlugin(plugin))
                            {
                                var Command = p.Value.Command("generarTablasPlanas", new object[] { server, entity.BaseDatos, usuario, password });


                                if (Command == "OK")
                                {
                                    entity.Estado = Constantes.ESTADO_ESCENARIO_GENERADO;
                                }
                                else
                                {
                                    entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                                    Log.Error(this, "EjecutarModelo" + Command);
                                }

                                Log.Info(this, "Generar - generarTablasPlanas ");
                            }
                        }
                        catch (Exception e)
                        {
                            entity.Estado = Constantes.ESTADO_ESCENARIO_EDICION;
                            Log.Error(this, "Generar - GenerarTablasPlanas", e);
                        }
                        using (var t = Services.statelessSession.BeginTransaction())
                        {
                            Services.statelessSession.Update(entity);
                            t.Commit();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("Escenarios - GenerarMod", ex);
                result.data = "Ocurrió un error al intentar generar tablas mod.";
                result.isError = true;
            }

            return result;
        }

        [SecurityDescription("Administración de Escenarios - Escenarios - 06 - Ver Log", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        [Route("[controller]/[action]/{idEscenario}")]
        public ReturnData GetLog(long idEscenario)
        {
            var result = new ReturnData();
            try
            {
                var escenario = new Gq_escenariosDto().SetEntity(Services.Get<ServGq_escenarios>(Services.statelessSession).findById(idEscenario));
                var pluglin = new { escenario = escenario, log = PluginsLoader.GetLog(escenario.BaseDatos) };
                result.data = pluglin;
            }
            catch (Exception ex)
            {
                Log.Error("Escenarios - GetLog", ex);
                result.data = "Ocurrió un error al intentar mostrar Log.";
                result.isError = true;
            }

            return result;
        }
    }
}