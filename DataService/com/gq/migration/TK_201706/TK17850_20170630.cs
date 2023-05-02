using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1785020170630, "Supuesto Desactivacion de los que no se utilizan")]
    public class TK17850_20170630 : Migration
    {
        public override void Up()
        { 
            Update.Table("Gq_supuesto").Set(new { Estado = "D" }).Where(new { Folder = "sup_controloptimizacion" });

            Update.Table("Gq_supuesto").Set(new { Estado = "D" }).Where(new { Folder = "sup_costoens" });
             
            Update.Table("Gq_supuesto").Set(new { Estado = "D" }).Where(new { Folder = "sup_costogns" });

            Update.Table("Gq_supuesto").Set(new { Estado = "D" }).Where(new { Folder = "sup_gasanualbase" });

            Update.Table("Gq_supuesto").Set(new { Estado = "D" }).Where(new { Folder = "sup_tipocambio" });

        }

        public override void Down()
        {
        }
    }
}
