using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1748320170517, "Creacion de tablas para la vista de GraficosChart")]
    public class TK17483_20170517 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_grafico").Exists())
            {
                Create.Table("Gq_grafico")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable()
                    .WithColumn("Tipo").AsString(50).NotNullable()
                    .WithColumn("Descripcion").AsString(128).NotNullable()
                    .WithColumn("Template").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Scritp").AsString(int.MaxValue).NotNullable()
                    .WithColumn("CodeSharp").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Estado").AsString(1).NotNullable()
                    .WithColumn("Creado").AsDateTime()
                    .WithColumn("CreadoPor").AsInt64()
                    .WithColumn("Modificado").AsDateTime().Nullable()
                    .WithColumn("ModificadoPor").AsInt64().Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
