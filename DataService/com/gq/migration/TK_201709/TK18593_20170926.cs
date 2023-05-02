using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1859320170926, "Cambiar 'Demanda de Gas' por 'Proyección de la demanda' / En Centrales Térmicas, cambiar 'Combustible' por 'Disponibilidad de Combustible'/ Sacar acento a 'Hidroductos' / Cambiar 'Generadores Eólicos' por 'Disponibilidad de Generadores Eólicos'")]
    public class TK18593_20170926 : Migration
    {
        public override void Up()
        {

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad de Generadores Eólicos"
            }).Where(new { Folder = "sup_disponibilidade" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Disponibilidad de Combustible"
            }).Where(new { Folder = "sup_combustibles" });

            Update.Table("gq_supuesto").Set(new
            {
                Nombre = "Proyección de la demanda"
            }).Where(new { Folder = "sup_anualesg" });

            Update.Table("Gq_supuesto").Set(new
            {
                Nombre = "Hidroductos"

            }).Where(new { Folder = "sup_hidroductos" });

            Update.Table("Gq_supuesto").Set(new
            {
                Descripcion = "Año"

            }).Where(new { Folder = "sup_escenarios" });


        }

        public override void Down()
        {
        }
    }
}
