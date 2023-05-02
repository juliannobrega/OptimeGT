using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786820170707, "Grafico Precio Spot y Costo Marginal ")]
    public class TK17868_20170707 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Precio Spot y costo medio de generación",
                Descripcion = "Precio Spot y costo medio de generación. Concepto: costo marginal/spot",
                Folder = "precioSpotCostoMarginal",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 1, 
                TipoSeleccion = 1,
            });
        }

        public override void Down()
        {
        }
    }
}
