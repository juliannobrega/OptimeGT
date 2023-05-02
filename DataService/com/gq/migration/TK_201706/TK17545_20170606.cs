using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1754520170606, "Escenario")]
    public class TK17545_20170606 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_escenarios").Exists())
            {
                Create.Table("Gq_escenarios")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable()
                    .WithColumn("BaseDatos").AsString(50).NotNullable()
                    .WithColumn("Estado").AsString(1).NotNullable()
                    .WithColumn("Creado").AsDateTime()
                    .WithColumn("CreadoPor").AsInt64()
                    .WithColumn("Modificado").AsDateTime().Nullable()
                    .WithColumn("ModificadoPor").AsInt64().Nullable();
            }

            if (!Schema.Table("Gq_tipos_grafico").Exists())
            {
                Create.Table("Gq_tipos_grafico")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable();

                Insert.IntoTable("Gq_tipos_grafico")
                    .Row(new { Nombre = "Electricidad" })
                    .Row(new { Nombre = "Gas" })
                    .Row(new { Nombre = "Hidráulico" });
            }

            if (!Schema.Table("Gq_grafico").Column("Folder").Exists())
            {
                Alter.Table("Gq_grafico").AddColumn("Folder").AsString(250).Nullable().Unique();
            }

            if (!Schema.Table("Gq_grafico").Column("TipoId").Exists())
            {
                Alter.Table("Gq_grafico").AddColumn("TipoId").AsInt64().NotNullable().WithDefaultValue(1);
            }

            if (Schema.Table("Gq_grafico").Column("Tipo").Exists())
            {
                Delete.Column("Tipo").FromTable("Gq_grafico");
            }
        }

        public override void Down()
        {
        }
    }
}
