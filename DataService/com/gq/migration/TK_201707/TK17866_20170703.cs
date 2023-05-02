using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786620170703, "Grafico generacion")]
    public class TK178662_20170703 : Migration
    {
        public override void Up()
        {
            Update.Table("Gq_grafico").Set(new
            {
                Nombre = "Generación y capacidad neta",
            }).Where(new
            {
                Folder = "generaciont"
            });
        }

        public override void Down()
        {
        }
    }
}
