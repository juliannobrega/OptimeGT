using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785620170630, "Supuesto Update sup_producciongas")]
    public class TK17856_20170630 : Migration
    {
        public override void Up()
        { 
            Update.Table("Gq_supuesto").Set(new {
                Nombre = "Producción y precios de gas",
                Descripcion = "Edición de información de largo plazo - Producción y precios de gas"
            }).Where(new {
                Folder = "sup_producciongas"
            });
        }

        public override void Down()
        {
        }
    }
}
