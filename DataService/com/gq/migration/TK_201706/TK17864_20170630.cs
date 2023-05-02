using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786420170630, "Supuesto Generadores Térmicos")]
    public class TK17864_20170630 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Líneas de transporte",
                Descripcion = "Edición de información de largo plazo - Líneas de transporte",
                Folder = "sup_lineas",
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
