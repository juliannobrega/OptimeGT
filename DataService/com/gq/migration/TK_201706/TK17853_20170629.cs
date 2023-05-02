using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785320170629, "Supuesto Generadores Térmicos")]
    public class TK17853_20170629 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Generadores Térmicos",
                Descripcion = "Edición de información de largo plazo - Generadores Térmicos",
                Folder = "sup_generadorest",
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
