using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1873620171023, "Supuesto Grupo Económico")]
    public class TK18736_20171023 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_grupoEconomico").Exists())
            {
                Create.Table("Gq_grupoEconomico")
                    .WithColumn("IdGen").AsString(50).PrimaryKey()
                    .WithColumn("Grupo").AsString(100).Nullable();
            }

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Grupo Económico",
                Descripcion = "Grupo Económico",
                Folder = "sup_grupoEconomico",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1

            });

           
        }

        public override void Down()
        {
        }
    }
}
