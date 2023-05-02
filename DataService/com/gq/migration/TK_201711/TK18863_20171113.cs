using FluentMigrator;

namespace MEMDataService.com.gq.migration
{
    [Migration(1886320171113, "Busqueda privacidad / ocultar boton Energia Total Contrato")]
    public class TK18863_20171113 : Migration
    {
        public override void Up()
        {
            //Busqueda avanzada privado/publico
            Alter.Table("Gq_escenarios").AlterColumn("Publico").AsString(255).WithDefaultValue("Publico");

            //Quitar el boton Total por tipo de contrato en energía
            Update.Table("gq_grafico").Set(new
            {
                Estado = "B"
            }).Where(new { Folder = "EnergiaTotalporTipodeContrato" });
        }

        public override void Down()
        {
        }
    }
}
