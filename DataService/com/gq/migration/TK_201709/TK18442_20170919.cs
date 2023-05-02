using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1844220170919, "gq_supuesto columnas depende y orden ")]
    public class TK18442_20170919 : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("gq_supuesto").Column("Depende").Exists())
            {
                Alter.Table("gq_supuesto")
                    .AddColumn("Depende").AsInt32().Nullable();
            }

            if (!Schema.Table("gq_supuesto").Column("Orden").Exists())
            {
                Alter.Table("gq_supuesto")
                    .AddColumn("Orden").AsInt32().Nullable();
            }
        }

        public override void Down()
        {
        }
    }
}
