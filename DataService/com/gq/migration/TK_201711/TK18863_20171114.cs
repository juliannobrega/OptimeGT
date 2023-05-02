using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1886320171114, "Creacion de tabla MailTemplate")]
    public class TK18863_20171114 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_mailTemplate").Exists())
            {
                Create.Table("Gq_mailTemplate")
                    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("Nombre").AsString(50).NotNullable()
                    .WithColumn("Folder").AsString(250).Nullable().Unique()
                    .WithColumn("Template").AsString(int.MaxValue).NotNullable()
                    .WithColumn("CodeSharp").AsString(int.MaxValue).NotNullable()
                    .WithColumn("Estado").AsString(1).NotNullable()
                    .WithColumn("Creado").AsDateTime()
                    .WithColumn("CreadoPor").AsInt64()
                    .WithColumn("Modificado").AsDateTime().Nullable()
                    .WithColumn("ModificadoPor").AsInt64().Nullable();
            }

            Insert.IntoTable("Gq_mailTemplate").Row(new
            {
                Nombre = "Creacion_usuario",
                Folder = "Creacion_usuario",
                Template = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_mailTemplate").Row(new
            {
                Nombre = "Clave_recuperada",
                Folder = "Clave_recuperada",
                Template = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_mailTemplate").Row(new
            {
                Nombre = "Clave_modificada",
                Folder = "Clave_modificada",
                Template = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            if (Schema.Table("gq_smtp_config").Column("User").Exists())
            {
                Delete.Column("User").FromTable("gq_smtp_config");
            }

            Update.Table("gq_smtp_config").Set(new
            {
                NombreFrom = "MEM - No Responder",
                Pass = "viernesdefacturas",
                EMailFrom = "geminus.qhom.test@gmail.com"
            }).Where(new { Nombre = "Gmail" });
        }

        public override void Down()
        {
        }
    }
}
