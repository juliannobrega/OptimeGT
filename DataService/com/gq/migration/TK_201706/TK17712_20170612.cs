using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1771220170612, "Escenario")]
    public class TK17712_20170612 : Migration
    {
        public override void Up()
        {            
            if (!Schema.Table("Gq_grafico").Column("TipoSeleccion").Exists())
            {
                Alter.Table("Gq_grafico").AddColumn("TipoSeleccion").AsInt64().NotNullable().WithDefaultValue(1);
            }
        }

        public override void Down()
        {
        }
    }
}
