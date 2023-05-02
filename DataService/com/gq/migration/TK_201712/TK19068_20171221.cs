using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1906820171221, "Tabla Descargas")]
    public class TK19068_20171221 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_descargas").Exists())
            {
                Create.Table("Gq_descargas")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(100).NotNullable()
                    .WithColumn("NombreArchivo").AsString(100).NotNullable()
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
