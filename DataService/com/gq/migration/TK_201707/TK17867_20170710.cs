using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1786720170710, "Grafico Consumo de combustibles")]
    public class TK17867_20170710 : Migration
    {
        public override void Up()
        {
            Update.Table("Gq_grafico").Set(new
            {
                TipoId = 1
            }).Where(new
            {
                Folder = "consumoCombustible"
            });
        }

        public override void Down()
        {
        }
    }
}
