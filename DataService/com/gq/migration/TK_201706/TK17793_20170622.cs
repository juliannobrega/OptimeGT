using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1779320170622, "Supuesto")]
    public class TK17793_20170622 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_gasoductos",
                Descripcion = "sup_gasoductos",
                Folder = "sup_gasoductos",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_producciongas",
                Descripcion = "sup_producciongas",
                Folder = "sup_producciongas",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_tipocambio",
                Descripcion = "sup_tipocambio",
                Folder = "sup_tipocambio",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });           

        }

        public override void Down()
        {
        }
    }
}
