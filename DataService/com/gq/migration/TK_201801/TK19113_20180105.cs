using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1911320180105, "Limpiar Menu / perfiles / accesos")]
    public class TK19113_20180105 : Migration
    {
        public override void Up()
        {
            Alter.Table("Gq_accesos").AlterColumn("Nombre").AsString(128).NotNullable();

            //Limpiar Menu / perfiles / accesos para dar un orden y facil acceso de configuracion 
            Delete.FromTable("Gq_menu").AllRows();
            Delete.FromTable("Gq_accesos").AllRows();
            Delete.FromTable("Gq_perfiles_accesos").AllRows();            

        }

        public override void Down()
        {
        }
    }
}
