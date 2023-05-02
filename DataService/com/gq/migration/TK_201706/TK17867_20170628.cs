using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786720170628, "Grafico Consumo de combustibles")]
    public class TK17867_20170628 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Consumo de combustibles",
                Descripcion = "Consumo de combustibles y generación de CO2 total país y total x nodo",
                Folder = "consumoCombustible",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2, 
                TipoSeleccion = 1,
            });
        }

        public override void Down()
        {
        }
    }
}
