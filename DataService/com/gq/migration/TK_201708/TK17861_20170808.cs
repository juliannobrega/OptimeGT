using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786120170808, "Supuestos ")]
    public class TK17861_20170808 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_restriccionarea",
                Descripcion = "sup_restriccionarea",
                Folder = "sup_restriccionarea",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });
        }

        public override void Down()
        {
        }
    }
}
