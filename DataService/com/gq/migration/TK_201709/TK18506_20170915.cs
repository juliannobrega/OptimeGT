using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1850620170915, "Grafico Equilibrio Nodo Gas ")]
    public class TK18506_20170915 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Equilibrio Nodo Gas",
                Descripcion = "Equilibrio Nodo Gas",
                Folder = "equilibrionodogas",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2,
                TipoSeleccion = 1
            });

            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Flujo de Líneas"
            }).Where(new
            {
                Folder = "flujoDeLineas"
            });

            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Generación y Capacidad Neta"
            }).Where(new
            {
                Folder = "generaciont"
            });

            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Flujo de Gas"
            }).Where(new
            {
                Folder = "flujoDeGas"
            });

            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Consumo de Combustibles"
            }).Where(new
            {
                Folder = "consumoCombustible"
            });

            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Precio Spot y Costo Medio de Generación"
            }).Where(new
            {
                Folder = "precioSpotCostoMarginal"
            });
        }

        public override void Down()
        {
        }
    }
}
