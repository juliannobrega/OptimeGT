using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1899220171201, "Grupo empresario en escernario")]
    public class TK18992_20171129 : Migration
    {
        public override void Up()
        {
            //Busqueda avanzada privado/publico
            Alter.Table("Gq_escenarios").AddColumn("GrupoEmpresarioId").AsInt32().WithDefaultValue(-1);

        }

        public override void Down()
        {
        }
    }
}
