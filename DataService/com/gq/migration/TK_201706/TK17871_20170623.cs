using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1787120170623, "Eliminacion row grafico en el menu")]
    public class TK17871_20170623 : Migration
    {
        public override void Up()
        {
            Delete.FromTable("gq_menu").Row(new {MenuPosition="90-30-00" });
        }

        public override void Down()
        {
        }
    }
}
