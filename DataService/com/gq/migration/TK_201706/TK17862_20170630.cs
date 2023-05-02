using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786220170630, "Supuesto Update sup_gasoductos")]
    public class TK17862_20170630 : Migration
    {
        public override void Up()
        { 
            Update.Table("Gq_supuesto").Set(new {
                Nombre = "Gasoductos",
                Descripcion = "Edición de información de largo plazo - Gasoductos"
            }).Where(new {
                Folder = "sup_gasoductos"
            });
        }

        public override void Down()
        {
        }
    }
}
