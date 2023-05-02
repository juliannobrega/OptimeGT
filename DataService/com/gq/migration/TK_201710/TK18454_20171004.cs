using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1845420171004, "Price Duration Curve")]
    public class TK18454_20171004 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Price Duration Curve",
                Descripcion = "Price Duration Curve",
                Folder = "priceDurationCurve",
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
