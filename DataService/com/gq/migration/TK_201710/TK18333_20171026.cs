using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1833320171026, "Escenario + Campo Publico")]
    public class TK18333_20171026 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Gq_escenarios").Column("Publico").Exists())
            {
                Alter.Table("Gq_escenarios").AddColumn("Publico").AsInt32().WithDefaultValue(0);
            }
            
        }

        public override void Down()
        {
        }
    }
}
