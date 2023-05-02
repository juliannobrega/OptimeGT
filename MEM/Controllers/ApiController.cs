using FluentNHibernate.Data;
using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.dto;
using GQService.com.gq.log;
using GQService.com.gq.security;
using GQService.com.gq.service;
using MEM.Controllers.Dto;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class ApiController : BaseController
    {
        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public ReturnData CrearNuevoEscenario([FromBody] EscenarionClonarDto data)
        {

            Log.Info("CrearNuevoEscenario");
            var controller = new EscenarioController();
            var entity = Services.Get<ServGq_escenarios>().findBy(x => x.BaseDatos == data.BaseDatos).FirstOrDefault();
            Log.Info("entity : " + (entity?.Id ?? 0) + " | " + (entity?.Nombre ?? "---"));
            var dto = new Gq_escenariosDto().SetEntity(entity);
            dto.Nombre = string.Format(data.EscenarionName, data.Ronda);

            dto.EscenarioOrigenId = dto.Id;
            dto.Id = null;
            dto.GenerarDatos = true;
            dto.Estado = Constantes.ESTADO_ESCENARIO_GENERADO;

            var result = controller.Clonar(dto);
            Log.Info("clone : " + (controller.escenarioClonado?.Id ?? -1) + " | OK : " + (!result.isError));
            if (result.isError)
            {
                return result;
            }
            entity = Services.Get<ServGq_escenarios>().findById(controller.escenarioClonado.Id);
            Log.Info("Create Escenario : " + entity.Id + " | " + entity.Nombre);
            return new ReturnData { data = entity, isError = false, };
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]")]
        public ReturnData EnviarPrecios([FromBody] PreciosDto data)
        {
            Gq_escenarios entity;

            try
            {
                Log.Info("EnviarPrecios");
                entity = Services.Get<ServGq_escenarios>().findById(data.IdEscenario);
            }
            catch (Exception e)
            {
                Log.Error("EnviarPrecios", e);
                return new ReturnData { data = "No se encuentra el escenario", isError = true, };
            }

            try
            {
                Log.Info("entity : " + (entity?.Id ?? 0) + " | " + (entity?.Nombre ?? "---") + " | " + (entity?.BaseDatos ?? "---"));
                Services.session.CreateSQLQuery($@"truncate table `{entity.BaseDatos}`.`tabla_precios`").ExecuteUpdate();
                var sql = Services.session.CreateSQLQuery($@"INSERT INTO {entity.BaseDatos}.`tabla_precios`
                            (`Nombre`,`ID_Contrato`,`ID_Combustible1`,`ID_Combustible2`,`Estado`,`CTUNG`,`O&MNoRen`,`O&MRen`,`PEO`,`PPO`,`CI`,`CITT`,`CEM`,`CTE`,`PCAL`,`FAGN`,`HorasRen`,`HorasNoRen`,`PGMax`,`PGMin`,`AnioInicio`,`FPNoRen`,`FPRen`,`F`,`K`,`Habilitado_Pujar`,`IDRonda`)
                            VALUES
                            (:Nombre,:Id_contrato,:Id_combustible1,:Id_combustible2,:Estado,:Ctung,:Omnoren,:Omren,:Peo,:Ppo,:Ci,:Citt,:Cem,:Cte,:Pcal,:Fagn,:Horasren,:Horasnoren,:Pgmax,:Pgmin,:Anioinicio,:Fpnoren,:Fpren,:F,:K,:Habilitado_pujar,:Idronda);");

                foreach (var item in data.Precios)
                {
                    sql.SetParameter("Nombre", item.Nombre);
                    sql.SetParameter("Id_contrato", item.Id_contrato);
                    sql.SetParameter("Id_combustible1", item.Id_combustible1);
                    sql.SetParameter("Id_combustible2", item.Id_combustible2);
                    sql.SetParameter("Estado", item.Estado);
                    sql.SetParameter("Ctung", item.Ctung);
                    sql.SetParameter("Omnoren", item.Omnoren);
                    sql.SetParameter("Omren", item.Omren);
                    sql.SetParameter("Peo", item.Peo);
                    sql.SetParameter("Ppo", item.Ppo);
                    sql.SetParameter("Ci", item.Ci);
                    sql.SetParameter("Citt", item.Citt);
                    sql.SetParameter("Cem", item.Cem);
                    sql.SetParameter("Cte", item.Cte);
                    sql.SetParameter("Pcal", item.Pcal);
                    sql.SetParameter("Fagn", item.Fagn);
                    sql.SetParameter("Horasren", item.Horasren);
                    sql.SetParameter("Horasnoren", item.Horasnoren);
                    sql.SetParameter("Pgmax", item.Pgmax);
                    sql.SetParameter("Pgmin", item.Pgmin);
                    sql.SetParameter("Anioinicio", item.Anioinicio);
                    sql.SetParameter("Fpnoren", item.Fpnoren);
                    sql.SetParameter("Fpren", item.Fpren);
                    sql.SetParameter("F", item.F);
                    sql.SetParameter("K", item.K);
                    sql.SetParameter("Habilitado_pujar", item.Habilitado_pujar);
                    sql.SetParameter("Idronda", item.Idronda);

                    sql.ExecuteUpdate();

                }
                Log.Info("EnviarPrecios cargados");
                return new ReturnData { data = true, isError = false, };
            }
            catch (Exception ex)
            {
                Log.Error("EnviarPrecios", ex);
                var result = Services.session.CreateSQLQuery($@"SELECT EXISTS (
                    SELECT 
                        TABLE_NAME
                    FROM 
                    information_schema.TABLES 
                    WHERE 
                    TABLE_SCHEMA LIKE '{entity.BaseDatos}' AND 
                        TABLE_TYPE LIKE 'BASE TABLE' AND
                        TABLE_NAME = 'tabla_precios'").List<long>().First();
                if (result > 0)
                {
                    return new ReturnData { data = ex.Message, isError = true, };
                }
                else
                {
                    return new ReturnData { data = "No se encuentra la base de datos o la tabla", isError = true, };
                }
            }
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        [Route("[controller]/[action]/{idEscenario}")]
        public ReturnData ObtenerResultado(long idEscenario)
        {
            try
            {
                Log.Info("ObtenerResultado");
                var entity = Services.Get<ServGq_escenarios>().findById(idEscenario);
                Log.Info("entity : " + (entity?.Id ?? 0) + " | " + (entity?.Nombre ?? "---") + " | " + (entity?.BaseDatos ?? "---"));
                var s = Services.session.CreateSQLQuery($@"SELECT `Nombre`,
                `AE` as Ae,
                `Seleccionado`,
                `PGMx`,
                `PGMn`,
                `PMedAdj`,
                `Energia`,
                `PorcentajeReduccion`,
                `PorcentajeParaTotal`,
                `MonomicoTot`
            FROM {entity.BaseDatos}.`mod_adjudicados`;");
                s.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(AdjudicadosDto)));
                var result = s.List<AdjudicadosDto>();

                Log.Info("ObtenerResultado enviados");

                return new ReturnData { data = result };
            }
            catch (Exception ex)
            {
                Log.Error("ObtenerResultado", ex);
                return new ReturnData { data = ex.Message, isError = true, };
            }
        }
    }
}
