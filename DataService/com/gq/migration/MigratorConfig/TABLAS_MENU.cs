using FluentMigrator;

namespace MEMDataService.com.gq.migrations
{
    [Migration(2, "Creación de las tablas de menu")]
    public class TABLAS_MENU : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("GQ_Menu").Exists())
            {
                Create.Table("GQ_Menu")
                .WithColumn("MenuId").AsInt64().PrimaryKey().Identity()
                .WithColumn("MenuPosition").AsString(50).Nullable()
                .WithColumn("MenuUrl").AsString(128).Nullable()
                .WithColumn("MenuIcono").AsString(255).Nullable()
                .WithColumn("KeyName").AsString(50).Nullable()
                .WithColumn("Nombre").AsString(50).Nullable()
                .WithColumn("MenuPadre").AsString(50).Nullable()
                .WithColumn("Estado").AsString(1).WithDefaultValue("A")
                .WithColumn("Creado").AsDateTime().Nullable()
                .WithColumn("Modificado").AsDateTime().Nullable()
                .WithColumn("CreadoPor").AsInt64().Nullable()
                .WithColumn("ModificadoPor").AsInt64().Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
