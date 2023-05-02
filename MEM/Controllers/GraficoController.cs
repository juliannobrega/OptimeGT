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
using MEM.com.gq.graficos;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static MEM.com.gq.graficos.ProcesarGraficos;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class GraficoController : BaseController, IABM<Gq_graficoDto>
    {
        [MenuDescription("30-00-10", "Graficos Generador", MEM.com.gq.security.Security.MENU_CONFIGURACION_AVANZADA)]
        [SecurityDescription("Configuración Avanzada - Graficos Generador - 01 - Ver Lista", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public IActionResult GraficosGenerador()
        {
            return PartialView();
        }
        
        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public Paging Buscar([FromBody]Paging paging)
        {
            try
            {
                var query = Services.Get<ServGq_grafico>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO);
                paging.Apply<Gq_grafico, Gq_graficoDto>(query);
                return paging;
            }
            catch (Exception ex)
            {
                Log.Error("Graficos Buscar", ex);
                return null;
            }

        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{id}")]
        public Gq_graficoDto GetGrafico(long id)
        {
            try
            {
                Gq_graficoDto model = new Gq_graficoDto().SetEntity(Services.Get<ServGq_grafico>(Services.statelessSession).findById(id));
                if (!string.IsNullOrWhiteSpace(model.Folder))
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();

                    using (var csRead = System.IO.File.OpenText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.cs"))
                    {
                        model.CodeSharp = csRead.ReadToEnd();
                    }

                    using (var jsRead = System.IO.File.OpenText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.js"))
                    {
                        model.Scritp = jsRead.ReadToEnd();
                    }

                    using (var htmlRead = System.IO.File.OpenText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.html"))
                    {
                        model.Template = htmlRead.ReadToEnd();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error("Graficos GetGrafico", ex);
                return null;
            }

        }

        [SecurityDescription("Configuración Avanzada - Graficos Generador - 02 - Guardar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Guardar([FromBody]Gq_graficoDto model)
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
                                entity.Scritp = "";
                                entity.Template = "";
                            }

                            if (entity.Id == null)
                            {
                                entity.Estado = Constantes.ESTADO_ACTIVO;
                                entity.CreadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                entity.Creado = DateTime.Now;
                                entity.Modificado = DateTime.Now;
                                entity = Services.Get<ServGq_grafico>(Services.statelessSession).Agregar(entity);
                            }
                            else
                            {
                                entity.Modificado = DateTime.Now;
                                entity.ModificadoPor = (long)com.gq.security.Security.usuarioLogueado.UsuarioId;
                                Services.Get<ServGq_grafico>(Services.statelessSession).Actualizar(entity);
                            }

                            if (!string.IsNullOrWhiteSpace(entity.Folder))
                            {
                                var dir = System.IO.Directory.GetCurrentDirectory();
                                if (!System.IO.Directory.Exists(dir + "\\wwwroot\\graficos\\" + model.Folder))
                                {
                                    System.IO.Directory.CreateDirectory(dir + "\\wwwroot\\graficos\\" + model.Folder);
                                }
                                System.IO.File.WriteAllText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.cs", model.CodeSharp);
                                System.IO.File.WriteAllText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.js", model.Scritp);
                                System.IO.File.WriteAllText(dir + "\\wwwroot\\graficos\\" + model.Folder + "\\grafico.html", model.Template);
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {                           
                            transaction.Rollback();
                            Log.Error("Graficos - Guardar", ex);
                            result.isError = true;
                            result.data = "Ocurrió un error al intentar guardar Grafico.";
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
                Log.Error("Graficos - Guardar", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar guardar Grafico.";
            }
            return result;
        }

        [SecurityDescription("Configuración Avanzada - Graficos Generador - 03 -Borrar", new string[] { MEM.com.gq.security.Security.ROL_ADMI })]
        public ReturnData Borrar([FromBody]Gq_graficoDto model)
        {
            ReturnData result = new ReturnData();

            using (var transaction = Services.session.BeginTransaction())
            {
                try
                {
                    var entity = Services.Get<ServGq_grafico>(Services.statelessSession).findById(model.Id);
                    //entity.Estado = Constantes.ESTADO_BORRADO;
                    Services.Get<ServGq_grafico>(Services.statelessSession).Borrar(entity);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.data = "Ocurrió el siguiente error al intentar borrar el grafico.";
                    result.isError = true;
                    Log.Error("Graficos - Borrar", ex);
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
                result = ProcesarGraficos.Ejecutar(model);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - Ejecutar", ex);
                return null;
            }

        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarTipos()
        {
            try
            {
                var s = Services.session.CreateSQLQuery("SELECT Id, Nombre as Label from gq_tipos_grafico ");
                s.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return s.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - BuscarTipos", ex);
                return null;
            }

        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{tipo}")]
        public ReturnData BuscarGraficosPorTipo(long tipo = 0)
        {
            ReturnData result = new ReturnData();
            try
            {
                //var tipoId = Services.Get<ServGq_grafico>().findByOne(x => x.Nombre.Contains(tipo)).Id.Value;
                result.data = Services.Get<ServGq_grafico>(Services.statelessSession).findBy(x => x.Estado != Constantes.ESTADO_BORRADO && x.TipoId == tipo);
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - BuscarGraficosPorTipo", ex);
                result.isError = true;
                result.data = "Ocurrió un error al intentar buscar graficos por tipo.";
            }

            return result;
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{schema},{tipo}")]
        public IList<ComboBoxDto> ObtenerNodos(string schema = "", long tipo = 0)
        {
            try
            {
                string tipos = string.Empty;
                if (tipo == 1)
                {
                    //Eléctrico
                    tipos = "('Linea','Vinculo','ExpImp')";
                }
                else if (tipo == 2)
                {
                    //Gas
                    tipos = "('Gasoducto')";
                }
                else
                {
                    //Hidráulico
                    tipos = "('Hidráulico')";
                }

                var sql = Services.session.CreateSQLQuery(string.Format(@"  SELECT DISTINCT NI as Id, NI as Label
                                                                            FROM `{0}`.sis_xylineas
                                                                            WHERE Tipo in {1}
                                                                            ORDER BY NI
                                                                            ", schema, tipos));
                sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return sql.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - BuscarGraficosPorTipo", ex);
                return null;
            }
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public IList<ComboBoxDto> BuscarCentrales()
        {
            try
            {
                var s = Services.session.CreateSQLQuery("SELECT distinct ID_GenT as Id, ID_GenT as Label from mem_escenario.s_generaciont_2 ");
                s.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
                return s.List<ComboBoxDto>();
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - BuscarCentrales", ex);
                return null;
            }
            
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{folderGrafico}")]
        public bool GetMostarBotonTodos(string folderGrafico)
        {
            try
            {
                //var grafico = Services.Get<ServGq_grafico>(Services.statelessSession).findById(idGrafico);
                //filtrar los que no deben mostrar
                if (folderGrafico == "flujoDeLineas"
                    || folderGrafico == "flujoDeGas"
                    || folderGrafico == "precioSpotCostoMarginal"
                    || folderGrafico == "priceDurationCurve")
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Graficos - GetMostarBotonTodos", ex);
                return false;
            }
        }

        public string ObtenerColor(string tipo)
        {
            switch (tipo)
            {
                case "ExpImp":
                    return "#00FF00";
                case "Gasoducto":
                    return "#FF0000";
                case "Linea":
                    return "#0000FF";
                case "Vinculo":
                    return "#00FFFF";
            }
            return "#6060FB";
        }

        public class NodoMapa
        {
            public const string TYPE_CIRCLE = "circle";
            public const string TYPE_POLYLINEA = "polyline";
            public const string TYPE_POLYGON = "polygon";
            public const string TYPE_RECTANGLE = "rectangle";
            public const string TYPE_GROUNDOVERLAY = "groundOverlay";
            public const string TYPE_IMAGE = "image";

            public string Id { get; set; }
            public string Type { get; set; }

            public string FillColor { get; set; }
            public double? FillOpacity { get; set; }

            public string StrokeColor { get; set; }
            public double? StrokeOpacity { get; set; }
            public double? StrokeWeight { get; set; }

            public List<double[]> Polyline { get; set; }
            public List<double[]> Polygon { get; set; }
            public List<double[]> Rectangle { get; set; }
            public double[] Circle { get; set; }
            public double? Radius { get; set; }

        }

        public class Marker
        {
            public const string TYPE_MARKER = "marker";

            public string Id { get; set; }
            public string Type { get; set; } = TYPE_MARKER;
            public string Label { get; set; }
            public string Icon { get; set; }
            public double[] Position { get; set; }
            public string FillColor { get; set; }
        }
    }
}
