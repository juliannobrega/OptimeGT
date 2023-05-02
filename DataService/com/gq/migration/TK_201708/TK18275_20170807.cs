using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827520170801, "Supuestos ")]
    public class TK18275_20170801 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_embalses",
                Descripcion = "sup_embalses",
                Folder = "sup_embalses",
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
                Nombre = "sup_hidroductos",
                Descripcion = "sup_hidroductos",
                Folder = "sup_hidroductos",
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
                Nombre = "sup_caudalesmxmn",
                Descripcion = "sup_caudalesmxmn",
                Folder = "sup_caudalesmxmn",
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
                Nombre = "sup_volumenesmaxmin",
                Descripcion = "sup_volumenesmaxmin",
                Folder = "sup_volumenesmaxmin",
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
