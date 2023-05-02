using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1911320180103, "Usuarios admin con grupo empresario  -1")]
    public class TK19113_20180103 : Migration
    {
        public override void Up()
        {  
            Execute.Sql(@"SET SQL_SAFE_UPDATES = 0; 
                            UPDATE gq_usuarios
                            SET GrupoEmpresario = -1
                            WHERE PerfilId in (SELECT PerfilId
                                                FROM gq_perfiles
                                                WHERE Nombre like '%admin%');");
        }

        public override void Down()
        {
        }
    }
}
