using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1782220170810, "Cambio de Escenario ")]
    public class TK17822_20170810 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("gq_escenarios").Column("SemanaInicio").Exists())
            {
                Alter.Table("gq_escenarios")
                    .AddColumn("SemanaInicio").AsInt32().Nullable()
                    .AddColumn("SemanaFin").AsInt32().Nullable()
                    ;
            }
        }

        public override void Down()
        {
        }
    }
}
