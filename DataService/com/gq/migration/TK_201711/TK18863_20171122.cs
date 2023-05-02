using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1886320171122, "Actualizacion de user gmail para panama")]
    public class TK18863_20171122 : Migration
    {
        public override void Up()
        {
            Update.Table("gq_smtp_config").Set(new
            {
                NombreFrom = "Equipo Optime Panama",
                Pass = "0pt1m32017",
                EMailFrom = "equipo.optime.panama@gmail.com"
            }).Where(new { Nombre = "Gmail" });
        }

        public override void Down()
        {
        }
    }
}
