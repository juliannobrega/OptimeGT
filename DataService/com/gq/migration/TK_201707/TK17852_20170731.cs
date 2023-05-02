using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785220170731, "Supuestos sup_anualesg")]
    public class TK17852_20170731 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Demandas de Gas",
                Descripcion = "Edición de información de largo plazo - Demandas de Gas",
                Folder = "sup_anualesg",
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
