using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1779320170615, "Creacion de tablas para la vista de Supuesto")]
    public class TK17793_20170615 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_supuesto").Exists())
            {
                Create.Table("Gq_supuesto")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable()                   
                    .WithColumn("Descripcion").AsString(128).NotNullable()
                    .WithColumn("Folder").AsString(250).Nullable().Unique()
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
