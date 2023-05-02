using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827520170808, "Supuestos ")]
    public class TK18275_20170808 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_genforzada",
                Descripcion = "sup_genforzada",
                Folder = "sup_genforzada",
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
                Nombre = "sup_restricciontransmax",
                Descripcion = "sup_restricciontransmax",
                Folder = "sup_restricciontransmax",
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
                Nombre = "sup_volcoef",
                Descripcion = "sup_volcoef",
                Folder = "sup_volcoef",
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
                Nombre = "sup_vseme",
                Descripcion = "sup_vseme",
                Folder = "sup_vseme",
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
