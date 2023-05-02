using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827520170811, "Supuestos ")]
    public class TK18275_20170811 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_supuesto").Column("Grupo").Exists())
            {
                Alter.Table("Gq_supuesto")
                    .AddColumn("Grupo").AsInt64().Nullable()
                    ;
            }            

            if (!Schema.Table("Gq_supuesto").Column("TablaMod").Exists())
            {
                Alter.Table("Gq_supuesto")
                    .AddColumn("TablaMod").AsString(50).Nullable()
                    ;
            }

            //Grupo supuesto
            #region Grupo supuesto
            Create.Table("GQ_Supuesto_Grupo")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Nombre").AsString(50);

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 1,
                Nombre = "Sistema"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 2,
                Nombre = "Centrales Eólicas"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 3,
                Nombre = "Centrales Hidráulicas"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 4,
                Nombre = "Centrales Térmicas"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 5,
                Nombre = "Sistema Transmisión"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 6,
                Nombre = "Demandas Energía"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 7,
                Nombre = "Sistema Gasoductos"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 8,
                Nombre = "Demandas Gas"
            });

            Insert.IntoTable("GQ_Supuesto_Grupo").Row(new
            {
                Id = 9,
                Nombre = "Embalses"
            });

            #endregion

            #region Update Nombre + Grupo + TablaMod en Supuesto
            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Drivers Demanda de EE",               
                TablaMod = "Mod_Demanda",
                Grupo = 6

            }).Where(new {Folder = "sup_anualese"});

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Parámetros de Optimización",               
                Grupo = 1
            }).Where(new { Folder = "sup_controloptimizacion" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Costos de Energía No Suministrada",
                Grupo = 6

            }).Where(new { Folder = "sup_costoens" });

             Update.Table("Gq_supuesto").Set(new
             {
                 Nombre = "Costos de Gas No Suministrado",
                 Grupo = 8

             }).Where(new { Folder = "sup_costogns" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Sistema de Gasoductos",
                Grupo = 7

            }).Where(new { Folder = "sup_gasoductos" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Producción y Precio del Gas",
                Grupo = 7

            }).Where(new { Folder = "sup_producciongas" });
            
            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Generadores Térmicos",
                TablaMod = "Mod_GeneradoresT",
                Grupo = 4

            }).Where(new { Folder = "sup_generadorest" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Líneas de Transporte",
                TablaMod = "Mod_Lineas",
                Grupo = 5

            }).Where(new { Folder = "sup_lineas" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Parámetros de los Coeficientes Variables",
                Grupo = 9

            }).Where(new { Folder = "sup_coefvars" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Cantidad de Calorías por Unidad de Combustible",
                TablaMod = "Mod_MCalxUnidad",
                Grupo = 4

            }).Where(new { Folder = "sup_mcalxunidad" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Variación de la Demanda por Día Típico",
                TablaMod = "Mod_DemandaGas",
                Grupo = 8

            }).Where(new { Folder = "sup_variaciondiatipicog" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Demandas de Gas",
                TablaMod = "Mod_CrecimientoDemandaG",
                Grupo = 8

            }).Where(new { Folder = "sup_anualesg" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Participación Porcentual",
                TablaMod = "Mod_DemandaGas",
                Grupo = 8

            }).Where(new { Folder = "sup_variaciondemandag" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Generadores Hidráulicos",
                TablaMod = "Mod_GeneradoresH",
                Grupo = 3

            }).Where(new { Folder = "sup_generadoresh" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Años Hidráulicos",
                Grupo = 1

            }).Where(new { Folder = "sup_escenarios" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Generadores Eólicos y Solares",
                TablaMod = "Mod_GeneradoresE",
                Grupo = 2

            }).Where(new { Folder = "sup_generadorese" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Embalses",
                TablaMod = "Mod_Embalses",
                Grupo = 3

            }).Where(new { Folder = "sup_embalses" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Hidrodúctos",
                TablaMod = "Mod_HidroDuctos",
                Grupo = 3

            }).Where(new { Folder = "sup_hidroductos" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Caudal Máximo y Mínimo",
                TablaMod = "Mod_CaudalesMxMn",
                Grupo = 3

            }).Where(new { Folder = "sup_caudalesmxmn" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Volumen Máximo y Mínimo",
                TablaMod = "Mod_VolumenesMaxMin",
                Grupo = 3

            }).Where(new { Folder = "sup_volumenesmaxmin" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Restricciones de Área",
                TablaMod = "Mod_RestriccionArea",
                Grupo = 5

            }).Where(new { Folder = "sup_restriccionarea" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Generación Forzada",
                TablaMod = "Mod_GenForzada",
                Grupo = 5

            }).Where(new { Folder = "sup_genforzada" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Restricción de Tránsito Máximo",
                TablaMod = "Mod_RestriccionTransMax",
                Grupo = 5

            }).Where(new { Folder = "sup_restricciontransmax" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Variación Semanal",
                TablaMod = "Mod_VSemE",
                Grupo = 2

            }).Where(new { Folder = "sup_vseme" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Generadores Eólicos",
                TablaMod = "Mod_DisponibilidadE",
                Grupo = 2

            }).Where(new { Folder = "sup_disponibilidade" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Afluentes Cambio",
                TablaMod = "Mod_AfluentesCambio",
                Grupo = 3

            }).Where(new { Folder = "sup_afluentescambio" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Afluentes",
                TablaMod = "Mod_Afluentes",
                Grupo = 3

            }).Where(new { Folder = "sup_afluentes" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad H",
                TablaMod = "Mod_DisponibilidadH",
                Grupo = 3

            }).Where(new { Folder = "sup_disponibilidadh" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad T",
                TablaMod = "Mod_DisponibilidadT",
                Grupo = 4

            }).Where(new { Folder = "sup_disponibilidadt" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Combustibles",
                TablaMod = "Mod_Combustibles",
                Grupo = 4

            }).Where(new { Folder = "sup_combustibles" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Tipo Cambio",               
                Grupo = 1

            }).Where(new { Folder = "sup_tipocambio" });

            Update.Table("Gq_supuesto").Set(new
            {
                Grupo = 3

            }).Where(new { Folder = "sup_volcoef" });

            Update.Table("Gq_supuesto").Set(new
            {
                Grupo = 8

            }).Where(new { Folder = "sup_gasanualbase" });

            Update.Table("Gq_supuesto").Set(new
            {
                Grupo = 8

            }).Where(new { Folder = "sup_variacionestacionalg" });

            Update.Table("Gq_supuesto").Set(new
            {
                Grupo = 8

            }).Where(new { Folder = "sup_variacionxatg" });

            #endregion

        }

        public override void Down()
        {
        }
    }
}
