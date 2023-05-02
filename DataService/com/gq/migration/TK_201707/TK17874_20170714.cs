using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1787420170714, "Supuestos ")]
    public class TK17874_20170714 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_coefvars",
                Descripcion = "sup_coefvars",
                Folder = "sup_coefvars",
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
                Nombre = "sup_mcalxunidad",
                Descripcion = "sup_mcalxunidad",
                Folder = "sup_mcalxunidad",
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
                Nombre = "sup_variaciondiatipicog",
                Descripcion = "sup_variaciondiatipicog",
                Folder = "sup_variaciondiatipicog",
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
                Folder = "sup_controloptimizacion"
            });

            Update.Table("Gq_supuesto").Set(new
            {
                Estado = "A"
            }).Where(new
            {
                Folder = "sup_costoens"
            });

            Update.Table("Gq_supuesto").Set(new
            {
                Estado = "A"
            }).Where(new
            {
                Folder = "sup_costogns"
            });

        }

        public override void Down()
        {
        }
    }
}
