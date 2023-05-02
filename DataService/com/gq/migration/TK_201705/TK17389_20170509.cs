using FluentMigrator;

namespace MEMDataService.com.gq.migrations
{
    [Migration(1738920170509, "Creacion de tablas para el envio de mail")]
    public class TK17389_20170509 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_smtp_config").Exists())
            {
                Create.Table("Gq_smtp_config")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Nombre").AsString(255).NotNullable()
                .WithColumn("NombreFrom").AsString(255).NotNullable()
                .WithColumn("User").AsString(255).NotNullable()
                .WithColumn("Pass").AsString(255).NotNullable()
                .WithColumn("Host").AsString(255).NotNullable()
                .WithColumn("Port").AsInt32().NotNullable()
                .WithColumn("UseDefaultCredentials").AsBoolean()
                .WithColumn("EnableSsl").AsBoolean()
                .WithColumn("EMailFrom").AsString(150).NotNullable();

                Insert.IntoTable("Gq_smtp_config").Row(new {Nombre="Gmail",
                    NombreFrom = "GQ - MEM - No Responder",
                    User = "gq.mem.test@gmail.com",
                    Pass = "SeptimoDia",
                    Host = "smtp.gmail.com",
                    Port = "465",
                    UseDefaultCredentials = 0,
                    EnableSsl = 0,
                    EMailFrom = "gq.mem.test@gmail.com"
                });
            }
        }

        public override void Down()
        {
        }
    }
}
