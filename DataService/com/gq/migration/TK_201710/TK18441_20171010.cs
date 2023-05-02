using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1844120171010, "Glosario")]
    public class TK18441_20171010 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Proyeccion Demanda",
                Descripcion = "sup_proyecciondemanda",
                Folder = "sup_proyecciondemanda",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1
                
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Estacionalidad Gen. Dist.",
                Descripcion = "sup_estacionalidadgendist",
                Folder = "sup_estacionalidadgendist",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1
                
            });


            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Glosario",
                Descripcion = "sup_glosario",
                Folder = "sup_glosario",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1
            });
        }

        public override void Down()
        {
        }
    }
}
