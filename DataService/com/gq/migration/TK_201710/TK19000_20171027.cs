using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1900020171027, "Nuevas tablas Mod para Panama")]
    public class TK19000_20171027 : Migration
    {
        //es el 18333 pero este script debe correr ultimo
        public override void Up()
        {
            Delete.FromTable("Gq_supuesto").AllRows();

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "Importar y Exportar Tablas",
                Descripcion = "Template de Importacion y exportacion de todas las tablas",
                Folder = "sup_ImpExpTodos",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TablaModNombre = "Importar y Exportar Tablas"
            });


            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_licitacionmes",
                Descripcion = "mod_licitacionmes",
                Folder = "mod_licitacionmes",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_licitacionmes",
                TablaModNombre = "mod_licitacionmes"
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_licitacionperfil",
                Descripcion = "mod_licitacionperfil",
                Folder = "mod_licitacionperfil",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_licitacionperfil",
                TablaModNombre = "mod_licitacionperfil"
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_oferente",
                Descripcion = "mod_oferente",
                Folder = "mod_oferente",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_oferente",
                TablaModNombre = "mod_oferente"
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_oferentemes",
                Descripcion = "mod_oferentemes",
                Folder = "mod_oferentemes",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_oferentemes",
                TablaModNombre = "mod_oferentemes"
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_oferenteperfil",
                Descripcion = "mod_oferenteperfil",
                Folder = "mod_oferenteperfil",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_oferenteperfil",
                TablaModNombre = "mod_oferenteperfil"
            });

            Insert.IntoTable("Gq_supuesto").Row(new
            {
                Nombre = "mod_parametros",
                Descripcion = "mod_parametros",
                Folder = "mod_parametros",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = "1",
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                Grupo = 1,
                TablaMod = "mod_parametros",
                TablaModNombre = "mod_parametros"
            });


        }

        public override void Down()
        {
        }
    }
}
