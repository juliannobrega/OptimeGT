using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1845320171005, "Margen bruto de generación")]
    public class TK18453_20171005 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Margen bruto de generación",
                Descripcion = "Margen bruto de generación",
                Folder = "margenBrutoDeGeneracion",
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
