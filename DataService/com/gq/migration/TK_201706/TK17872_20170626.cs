using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1787220170626, "Grafico Flujo Gas")]
    public class TK17872_20170626 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Flujo de gas",
                Descripcion = "Flujo de gas y capacidad de gasoductos",
                Folder = "flujoDeGas",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2, 
                TipoSeleccion = 2,
            });
        }

        public override void Down()
        {
        }
    }
}
