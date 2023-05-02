using FluentMigrator;

namespace MEMDataService.com.gq.migrations
{
    [Migration(1883920171106, "Grupo Empresario")]
    public class TK18839_20171106 : Migration
    {       

        public override void Up()
        {            
            if (!Schema.Table("GQ_GrupoEmpresario").Exists())
            {
                Create.Table("GQ_GrupoEmpresario")
                .WithColumn("GrupoEmpresarioId").AsInt64().PrimaryKey().Identity()
                .WithColumn("Nombre").AsString(255) 
                .WithColumn("Descripcion").AsString(255).Nullable()
                .WithColumn("Estado").AsString(1).WithDefaultValue("A")
                .WithColumn("Creado").AsDateTime().Nullable()
                .WithColumn("Modificado").AsDateTime().Nullable()
                .WithColumn("CreadoPor").AsInt64().Nullable()
                .WithColumn("ModificadoPor").AsInt64().Nullable();
            }

            if (!Schema.Table("GQ_Usuarios").Column("GrupoEmpresario").Exists())
            {
                Alter.Table("GQ_Usuarios")
                    .AddColumn("GrupoEmpresario").AsInt64().NotNullable();
            }
        }

        public override void Down()
        {
        }
    }
}