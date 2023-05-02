using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1782420170628, "Generacion de Escenario Base")]
    public class TK17824_20170628 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_escenarios").Column("EsBase").Exists())
            {
                Alter.Table("Gq_escenarios")
                    .AddColumn("EsBase").AsString(1).NotNullable().WithDefaultValue(constantes.Constantes.ESTADO_NO)
                    .AddColumn("PluginId").AsInt64().Nullable()
                    .AddColumn("Descripcion").AsString(int.MaxValue).Nullable()
                    .AddColumn("FechaInicio").AsInt32().Nullable()
                    .AddColumn("FechaFin").AsInt32().Nullable()
                    ;
            }

            if (!Schema.Table("Gq_plugin").Exists())
            {
                Create.Table("Gq_plugin")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable()
                    .WithColumn("Tipo").AsString(1).NotNullable() // I - Importacion datos // E - Ejecucion Modelo
                    .WithColumn("Folder").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Estado").AsString(1).NotNullable()
                    .WithColumn("Creado").AsDateTime()
                    .WithColumn("CreadoPor").AsInt64()
                    .WithColumn("Modificado").AsDateTime().Nullable()
                    .WithColumn("ModificadoPor").AsInt64().Nullable();
                ;
            }

        }

        public override void Down()
        {
        }
    }
}
