using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786620170627, "Grafico generacion T")]
    public class TK178662_20170627 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Generacion T",
                Descripcion = "Generación y capacidad neta por grupo de generadores o por generador",
                Folder = "generaciont",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 1, 
                TipoSeleccion = 1
            });
        }

        public override void Down()
        {
        }
    }
}
