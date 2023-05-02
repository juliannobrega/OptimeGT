using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785420170807, "Supuestos ")]
    public class TK17854_20170807 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_generadoresh",
                Descripcion = "sup_generadoresh",
                Folder = "sup_generadoresh",
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
