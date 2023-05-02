using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1844420171011, "Importar y Exportar Todos")]
    public class TK18444_20171011 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Importar y Exportar Todos",
                Descripcion = "Template de Importacion y exportacion de todos los supuestos",
                Folder = "sup_ImpExpTodos",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1

            });
        }

        public override void Down()
        {
        }
    }
}
