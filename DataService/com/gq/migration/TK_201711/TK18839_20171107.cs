using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1883920171107, "Cambios de Seguridad en Usuarios")]
    public class TK18839_20171107 : Migration
    {
        public override void Up()
        {
            if(!Schema.Table("Gq_usuarios").Column("LoginKey").Exists())
            {
                Alter.Table("Gq_usuarios").AddColumn("LoginKey").AsString(128).Nullable();
            }
            
        }

        public override void Down()
        {
        }
    }
}
