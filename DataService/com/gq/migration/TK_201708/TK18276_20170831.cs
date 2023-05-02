using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1827620170831, "Supuestos Correccion de nombres y tablas mod")]
    public class TK18276_20170831 : Migration
    {
        //originalmente era TK18274_20170831, cambiamos el orden ya que necesitamos crear la tabla grupo primero
        public override void Up()
        {
            Delete.FromTable("gq_supuesto_grupo").Row(new { Nombre = "Embalses" });


            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Parámetros de los Coeficientes Variables",
                Grupo = 3
            }).Where(new { Folder = "Sup_CoefVars" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad Generadores Hidráulicos"
            }).Where(new { Folder = "Sup_DisponibilidadH" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Coeficiente de Prod. Versus Volumen",
                TablaMod = "Mod_VolCoef"
            }).Where(new { Folder = "Sup_VolCoef" });

            Update.Table("Gq_supuesto").Set(new
            {
                TablaMod = "Mod_CaudalesMxMn"
            }).Where(new { Folder = "Sup_CaudalesMxMn" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad Generadores Térmicos"
            }).Where(new { Folder = "Sup_DisponibilidadT" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Total de Gas Anual de base",
                TablaMod = "Mod_DemandaGas"
            }).Where(new { Folder = "Sup_GasAnualBase" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Variación de la Demanda por Semana",
                TablaMod = "Mod_DemandaGas"
            }).Where(new { Folder = "Sup_VariacionEstacionalG" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Variación de la Demanda por Año Térmico",
                TablaMod = "Mod_DemandaGas"
            }).Where(new { Folder = "Sup_VariacionxATG" });
        }

        public override void Down()
        {
        }
    }
}
