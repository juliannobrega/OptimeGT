using FluentMigrator;

namespace MEMDataService.com.gq.migrations
{
    [Migration(1, "Creación de las tablas de usuarios y perfiles")]
    public class TABLAS_SECURITY : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("GQ_Perfiles").Exists())
            {
                Create.Table("GQ_Perfiles")
                .WithColumn("PerfilId").AsInt64().PrimaryKey().Identity()
                .WithColumn("Nombre").AsString(50)
                .WithColumn("KeyName").AsString(50).Nullable()
                .WithColumn("Estado").AsString(1).WithDefaultValue("A")
                .WithColumn("Creado").AsDateTime().Nullable()
                .WithColumn("Modificado").AsDateTime().Nullable()
                .WithColumn("CreadoPor").AsInt64().Nullable()
                .WithColumn("ModificadoPor").AsInt64().Nullable();
            }

            if (!Schema.Table("GQ_Accesos").Exists())
            {
                Create.Table("GQ_Accesos")
                .WithColumn("AccesoId").AsInt64().PrimaryKey().Identity()
                .WithColumn("Nombre").AsString(45)
                .WithColumn("Descripcion").AsString(255)
                .WithColumn("Tipo").AsInt64().WithDefaultValue(0)
                .WithColumn("Clase").AsString(255).Nullable()
                .WithColumn("Metodo").AsString(255).Nullable()
                .WithColumn("Orden").AsString(15).Nullable();
            }

            if (!Schema.Table("GQ_Perfiles_accesos").Exists())
            {
                Create.Table("GQ_Perfiles_accesos")
                .WithColumn("PerfilesAccesosId").AsInt64().PrimaryKey().Identity()
                .WithColumn("PerfilId").AsInt64()
                .WithColumn("AccesoId").AsInt64()
                .WithColumn("GrantPermition").AsString(1).WithDefaultValue("Y")
                .WithColumn("Estado").AsString(1).WithDefaultValue("A")
                .WithColumn("Creado").AsDateTime().Nullable()
                .WithColumn("Modificado").AsDateTime().Nullable()
                .WithColumn("CreadoPor").AsInt64().Nullable()
                .WithColumn("ModificadoPor").AsInt64().Nullable();
            }

            if (!Schema.Table("GQ_Usuarios").Exists())
            {
                Create.Table("GQ_Usuarios")
                .WithColumn("UsuarioId").AsInt64().PrimaryKey().Identity()
                .WithColumn("Usuario").AsString(255).Unique()
                .WithColumn("Nombre").AsString(255)
                .WithColumn("Apellido").AsString(255)
                .WithColumn("Email").AsString(255).Unique()
                .WithColumn("Clave").AsString(255)
                .WithColumn("PerfilId").AsInt64()
                .WithColumn("RequiereClave").AsString(1).WithDefaultValue("N")
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
