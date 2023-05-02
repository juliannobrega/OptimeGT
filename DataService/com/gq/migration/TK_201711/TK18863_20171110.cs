using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1886320171110, "Supuestos - TablaModNombre")]
    public class TK18863_20171110 : Migration
    {
        public override void Up()
        {
            Update.Table("gq_grafico").Set(new
            {
                Nombre = "Total por Tipo de Contrato"
            }).Where(new { Folder = "PotenciaTotalporTipo" });

            Update.Table("gq_grafico").Set(new
            {
                Nombre = "Por Oferente y contrato"
            }).Where(new { Folder = "PotenciaporOferenteyContrato" });

            Update.Table("gq_grafico").Set(new
            {
                Nombre = "Total por Tipo de Contrato"
            }).Where(new { Folder = "EnergiaTotalporTipodeContrato" });


            Update.Table("gq_grafico").Set(new
            {
                Nombre = "Por Oferente y contrato"
            }).Where(new { Folder = "EnergiaporOferenteyContrato" });

            Update.Table("gq_grafico").Set(new
            {
                Nombre = "Asociada a la Potencia"
            }).Where(new { Folder = "EnergiaAsociadaalaPotencia" });
            
        }

        public override void Down()
        {
        }
    }
}
