using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785120170630, "Supuesto Update sup_anualese")]
    public class TK17851_20170630 : Migration
    {
        public override void Up()
        { 
            Update.Table("Gq_supuesto").Set(new {
                Nombre = "Demandas Energía Eléctrica",
                Descripcion = "Edición de información de largo plazo - Demandas Energía Eléctrica"
            }).Where(new {
                Folder = "sup_anualese"
            });
        }

        public override void Down()
        {
        }
    }
}
