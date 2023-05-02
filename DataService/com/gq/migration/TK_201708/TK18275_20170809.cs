using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827520170809, "Supuestos ")]
    public class TK18275_20170809 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_afluentes",
                Descripcion = "sup_afluentes",
                Folder = "sup_afluentes",
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
                Nombre = "sup_afluentescambio",
                Descripcion = "sup_afluentescambio",
                Folder = "sup_afluentescambio",
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
                Nombre = "sup_disponibilidade",
                Descripcion = "sup_disponibilidade",
                Folder = "sup_disponibilidade",
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
