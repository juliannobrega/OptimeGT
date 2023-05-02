using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1788320170703, "Generacion de Escenario Base Relacion Clonado")]
    public class TK17883_20170703 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_escenarios").Column("EscenarioOrigenId").Exists())
            {
                Alter.Table("Gq_escenarios")
                    .AddColumn("EscenarioOrigenId").AsInt64().Nullable().WithDefaultValue(0)
                    ;
            }
        }

        public override void Down()
        {
        }
    }
}
