using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1894420171129, "Limitar la cantidad de escenarios de los grupos empresarios")]
    public class TK18944_20171129 : Migration
    {
        public override void Up()
        {
            //Busqueda avanzada privado/publico
            Alter.Table("Gq_grupoempresario").AddColumn("Limite").AsInt32().WithDefaultValue(5);
            
        }

        public override void Down()
        {
        }
    }
}
