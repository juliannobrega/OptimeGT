using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1754520170711, "Escenario")]
    public class TK17545_20170711 : Migration
    {
        public override void Up()
        {
            Delete.FromTable("Gq_tipos_grafico").Row(new { Nombre = "Hidráulico" });
        }

        public override void Down()
        {
        }
    }
}
