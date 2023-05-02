using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785520170804, "Supuestos ")]
    public class TK17855_20170804 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_escenarios",
                Descripcion = "sup_escenarios",
                Folder = "sup_escenarios",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_generadorese",
                Descripcion = "sup_generadorese",
                Folder = "sup_generadorese",
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
