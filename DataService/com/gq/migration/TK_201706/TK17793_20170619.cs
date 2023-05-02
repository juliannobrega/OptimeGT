using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1779320170620, "Creacion de tablas para la vista de Supuesto")]
    public class TK17793_20170620 : Migration
    {
        public override void Up()
        {                           
            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "sup_anualese",
                Descripcion = "sup_anualese",
                Folder = "sup_anualese",
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
                Nombre = "sup_controloptimizacion",
                Descripcion = "sup_controloptimizacion",
                Folder = "sup_controloptimizacion",
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
                Nombre = "sup_costoens",
                Descripcion = "sup_costoens",
                Folder = "sup_costoens",
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
                Nombre = "sup_costogns",
                Descripcion = "sup_costogns",
                Folder = "sup_costogns",
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
                Nombre = "sup_gasanualbase",
                Descripcion = "sup_gasanualbase",
                Folder = "sup_gasanualbase",
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
