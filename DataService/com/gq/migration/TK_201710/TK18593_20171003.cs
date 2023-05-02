using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1859320171003, "Supuestos - TablaModNombre")]
    public class TK18593_20171003 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_supuesto").Column("TablaModNombre").Exists())
            {
                Alter.Table("Gq_supuesto")
                    .AddColumn("TablaModNombre").AsString(50).Nullable()
                    ;
            }

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Generadores Eólicos y Solares Futuros",
                TablaModNombre = "Generadores Eólicos y Solares"
            }).Where(new { Folder = "Sup_GeneradoresE" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaModNombre = "Variación Semanal"
            }).Where(new { Folder = "Sup_VSemE" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad de Generacións",
                TablaModNombre = "Disponibilidad de Generación"
            }).Where(new { Folder = "Sup_DisponibilidadE" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Generadores Hidráulicos Futuros",
                TablaModNombre = "Generadores Hidráulicos"
            }).Where(new { Folder = "Sup_GeneradoresH" });


            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Embalses"
            }).Where(new { Folder = "Sup_Embalses" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Hidroductos"
            }).Where(new { Folder = "Sup_HidroDuctos" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Afluentes Cambio"
            }).Where(new { Folder = "Sup_AfluentesCambio" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Afluentes"
            }).Where(new { Folder = "Sup_Afluentes" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad de Generación",
                TablaModNombre = "Disponibilidad de Generación"
            }).Where(new { Folder = "Sup_DisponibilidadH" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Volumen Máximo y Mínimo"
            }).Where(new { Folder = "Sup_VolumenesMaxMin" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaModNombre = "Caudal Máximo y Mínimo"
            }).Where(new { Folder = "Sup_CaudalesMxMn" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Coef. Prod Vs. Volumen",
                TablaModNombre = "Coef. Prod Vs. Volumen"
            }).Where(new { Folder = "Sup_VolCoef" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Param. De Coef. Variables"
            }).Where(new { Folder = "Sup_CoefVars" });


            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Generadores Térmicos Futuros",
                TablaModNombre = "Generadores Térmicos"
            }).Where(new { Folder = "Sup_GeneradoresT" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Cantidad de MCal por Unidad de Comb.",
                TablaModNombre = "Cantidad de MCal por Unidad de Comb."
            }).Where(new { Folder = "Sup_MCalxUnidad" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaModNombre = "Disponibilidad de Combustible"
            }).Where(new { Folder = "Sup_Combustibles" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad de Generación",
                TablaModNombre = "Disponibilidad de Generación"
            }).Where(new { Folder = "Sup_DisponibilidadT" });
            
            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Líneas de Transporte Futuras",
                TablaModNombre = "Líneas de Transporte"
            }).Where(new { Folder = "Sup_Lineas" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaModNombre = "Restricción de Tránsito Máximo"
            }).Where(new { Folder = "Sup_RestriccionTransMax" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaModNombre = "Generación Forzada"
            }).Where(new { Folder = "Sup_GenForzada" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaModNombre = "Restricciones de Área"
            }).Where(new { Folder = "Sup_RestriccionArea" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Drivers Demanda de Energía",
                TablaMod = ""
            }).Where(new { Folder = "Sup_AnualesE" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaMod = ""
            }).Where(new { Folder = "Sup_GasAnualBase" });

            Update.Table("gq_supuesto").Set(new
            {
                TablaMod = ""
            }).Where(new { Folder = "Sup_AnualesG" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Participación por Categoría",
                TablaMod = ""
            }).Where(new { Folder = "Sup_VariacionDemandaG" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Participación por Día Típico",
                TablaMod = ""
            }).Where(new { Folder = "Sup_VariacionDiaTipicoG" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Participación por Semana",
                TablaMod = ""
            }).Where(new { Folder = "Sup_VariacionEstacionalG" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Variación por Año Térmico",
                TablaMod = ""
            }).Where(new { Folder = "Sup_VariacionxATG" });
        }

        public override void Down()
        {
        }
    }
}
