using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1787120170622, "Grafico Equilibrio Nodo /Flujo Linea")]
    public class TK17871_20170622 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Equilibrio Nodo",
                Descripcion = "Equilibrio Nodo",
                Folder = "equilibrionodo",
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

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Flujo de líneas",
                Descripcion = "Flujo de potencia y capacidad de líneas de transporte",
                Folder = "flujoDeLineas",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 1, 
                TipoSeleccion = 2
            });
        }

        public override void Down()
        {
        }
    }
}
