using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1844420171023, "Importar y Exportar Todos - TablaModNombre")]
    public class TK18444_20171023 : Migration
    {
        public override void Up()
        {           
            Update.Table("gq_supuesto_grupo").Set(new
            {                
                Nombre = "Centrales Eólicas y Solares"
            }).Where(new { Nombre = "Centrales Eólicas" });
        }

        public override void Down()
        {
        }
    }
}
