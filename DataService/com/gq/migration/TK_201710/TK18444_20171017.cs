using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1844420171017, "Importar y Exportar Todos - TablaModNombre")]
    public class TK18444_20171017 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_supuesto").Column("TablaModNombre").Exists())
            {
                Alter.Table("Gq_supuesto")
                    .AddColumn("TablaModNombre").AsString(50).Nullable()
                    ;
            }

            Update.Table("gq_supuesto").Set(new
            {                
                TablaModNombre = "Importar y Exportar Todos"
            }).Where(new { Folder = "sup_ImpExpTodos" });
        }

        public override void Down()
        {
        }
    }
}
