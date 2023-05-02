using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827520170810, "Supuestos ")]
    public class TK18275_20170810 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_combustibles",
                Descripcion = "sup_combustibles",
                Folder = "sup_combustibles",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_disponibilidadh",
                Descripcion = "sup_disponibilidadh",
                Folder = "sup_disponibilidadh",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_disponibilidadt",
                Descripcion = "sup_disponibilidadt",
                Folder = "sup_disponibilidadt",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });
            
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_variacionestacionalg",
                Descripcion = "sup_variacionestacionalg",
                Folder = "sup_variacionestacionalg",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_variacionxatg",
                Descripcion = "sup_variacionxatg",
                Folder = "sup_variacionxatg",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
            });

            Update.Table("Gq_supuesto").Set(new
            {
                Estado = "A"
            }).Where(new
            {
                Folder = "sup_gasanualbase"
            });

            Update.Table("Gq_supuesto").Set(new
            {
                Estado = "A"
            }).Where(new
            {
                Folder = "sup_tipocambio"
            });
        }

        public override void Down()
        {
        }
    }
}
