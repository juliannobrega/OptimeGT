using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1845920171024, "Equilibrio Nodo Nombre")]
    public class TK18459_20171024 : Migration
    {
        public override void Up()
        {           
            Update.Table("gq_grafico").Set(new
            {                
                Nombre = "Balance de Nodo"
            }).Where(new { Folder = "equilibrionodo" });
        }

        public override void Down()
        {
        }
    }
}
