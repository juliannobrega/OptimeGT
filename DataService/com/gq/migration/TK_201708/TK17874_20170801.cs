using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1787420170801, "Supuestos ")]
    public class TK17874_20170801 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_variaciondemandag",
                Descripcion = "sup_variaciondemandag",
                Folder = "sup_variaciondemandag",
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
