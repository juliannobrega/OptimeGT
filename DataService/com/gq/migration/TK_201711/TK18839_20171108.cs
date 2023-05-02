using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1883920171108, "Escenario + Campo Publico compo String")]
    public class TK18839_20171108 : Migration
    {
        public override void Up()
        {            
            Alter.Table("Gq_escenarios").AlterColumn("Publico").AsString(255).WithDefaultValue("Público");
        }

        public override void Down()
        {
        }
    }
}
