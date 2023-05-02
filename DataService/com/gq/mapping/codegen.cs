
using FluentNHibernate.Mapping;
using GQDataService.com.gq.domain;
using MEMDataService.com.gq.domain;

namespace MEMDataService.com.gq.mapping.codegen
{

    public class _MapGq_accesos : ClassMap<Gq_accesos>
    {
        public _MapGq_accesos()
        {
        	Table("gq_accesos");
        	
			Id(c => c.AccesoId).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(45);

			Map(c => c.Descripcion).Length(255);

			Map(c => c.Tipo);

			Map(c => c.Clase).Length(255);

			Map(c => c.Metodo).Length(255);

			Map(c => c.Orden).Length(15);

		}
    }

    public class _MapGq_escenarios : ClassMap<Gq_escenarios>
    {
        public _MapGq_escenarios()
        {
        	Table("gq_escenarios");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

			Map(c => c.BaseDatos).Length(50);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.CreadoPor);

			Map(c => c.Modificado);

			Map(c => c.ModificadoPor);

			Map(c => c.EsBase).Length(1);

			Map(c => c.PluginId);

			Map(c => c.Descripcion).Length(int.MaxValue);

			Map(c => c.FechaInicio);

			Map(c => c.FechaFin);

            Map(c => c.EscenarioOrigenId);

            Map(c => c.SemanaInicio);

            Map(c => c.SemanaFin);

            Map(c => c.Publico);

            Map(c => c.GrupoEmpresarioId);

        }
    }

    public class _MapGq_grafico : ClassMap<Gq_grafico>
    {
        public _MapGq_grafico()
        {
        	Table("gq_grafico");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

			Map(c => c.Descripcion).Length(128);

			Map(c => c.Template).Length(int.MaxValue);

			Map(c => c.Scritp).Length(int.MaxValue);

			Map(c => c.CodeSharp).Length(int.MaxValue);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.CreadoPor);

			Map(c => c.Modificado);

			Map(c => c.ModificadoPor);

			Map(c => c.Folder).Length(250);

			Map(c => c.TipoId);

			Map(c => c.TipoSeleccion);

		}
    }

    public class _MapGq_menu : ClassMap<Gq_menu>
    {
        public _MapGq_menu()
        {
        	Table("gq_menu");
        	
			Id(c => c.MenuId).GeneratedBy.Identity();
			Map(c => c.MenuPosition).Length(50);

			Map(c => c.MenuUrl).Length(128);

			Map(c => c.MenuIcono).Length(255);

			Map(c => c.KeyName).Length(50);

			Map(c => c.Nombre).Length(50);

			Map(c => c.MenuPadre).Length(50);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.Modificado);

			Map(c => c.CreadoPor);

			Map(c => c.ModificadoPor);

		}
    }

    public class _MapGq_perfiles : ClassMap<Gq_perfiles>
    {
        public _MapGq_perfiles()
        {
        	Table("gq_perfiles");
        	
			Id(c => c.PerfilId).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

			Map(c => c.KeyName).Length(50);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.Modificado);

			Map(c => c.CreadoPor);

			Map(c => c.ModificadoPor);

		}
    }

    public class _MapGq_perfiles_accesos : ClassMap<Gq_perfiles_accesos>
    {
        public _MapGq_perfiles_accesos()
        {
        	Table("gq_perfiles_accesos");
        	
			Id(c => c.PerfilesAccesosId).GeneratedBy.Identity();
			Map(c => c.PerfilId);

			Map(c => c.AccesoId);

			Map(c => c.GrantPermition).Length(1);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.Modificado);

			Map(c => c.CreadoPor);

			Map(c => c.ModificadoPor);

		}
    }

    public class _MapGq_plugin : ClassMap<Gq_plugin>
    {
        public _MapGq_plugin()
        {
        	Table("gq_plugin");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

			Map(c => c.Tipo).Length(1);

			Map(c => c.Folder).Length(int.MaxValue);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.CreadoPor);

			Map(c => c.Modificado);

			Map(c => c.ModificadoPor);

		}
    }

    public class _MapGq_descargas : ClassMap<Gq_descargas>
    {
        public _MapGq_descargas()
        {
            Table("gq_descargas");

            Id(c => c.Id).GeneratedBy.Identity();

            Map(c => c.Nombre).Length(100);
            
            Map(c => c.NombreArchivo).Length(100);

            Map(c => c.Folder).Length(int.MaxValue);

            Map(c => c.Estado).Length(1);

            Map(c => c.Creado);

            Map(c => c.CreadoPor);

            Map(c => c.Modificado);

            Map(c => c.ModificadoPor);

        }
    }

    public class _MapGq_smtp_config : ClassMap<Gq_smtp_config>
    {
        public _MapGq_smtp_config()
        {
        	Table("gq_smtp_config");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(255);

			Map(c => c.NombreFrom).Length(255);

			Map(c => c.Pass).Length(255);

			Map(c => c.Host).Length(255);

			Map(c => c.Port);

			Map(c => c.UseDefaultCredentials);

			Map(c => c.EnableSsl);

			Map(c => c.EMailFrom).Length(150);

		}
    }

    public class _MapGq_supuesto : ClassMap<Gq_supuesto>
    {
        public _MapGq_supuesto()
        {
        	Table("gq_supuesto");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

			Map(c => c.Descripcion).Length(128);

			Map(c => c.Folder).Length(250);

			Map(c => c.Template).Length(int.MaxValue);

			Map(c => c.Scritp).Length(int.MaxValue);

			Map(c => c.CodeSharp).Length(int.MaxValue);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.CreadoPor);

			Map(c => c.Modificado);

			Map(c => c.ModificadoPor);

            Map(c => c.Grupo);

            Map(c => c.TablaMod);

            Map(c => c.Depende);

            Map(c => c.Orden);

            Map(c => c.TablaModNombre);

        }
    }

    public class _MapGq_tipos_grafico : ClassMap<Gq_tipos_grafico>
    {
        public _MapGq_tipos_grafico()
        {
        	Table("gq_tipos_grafico");
        	
			Id(c => c.Id).GeneratedBy.Identity();
			Map(c => c.Nombre).Length(50);

		}
    }

    public class _MapGq_usuarios : ClassMap<Gq_usuarios>
    {
        public _MapGq_usuarios()
        {
        	Table("gq_usuarios");
        	
			Id(c => c.UsuarioId).GeneratedBy.Identity();
			Map(c => c.Usuario).Length(255);

			Map(c => c.Nombre).Length(255);

			Map(c => c.Apellido).Length(255);

			Map(c => c.Email).Length(255);

			Map(c => c.Clave).Length(255);

			Map(c => c.PerfilId);

			Map(c => c.RequiereClave).Length(1);

			Map(c => c.Estado).Length(1);

			Map(c => c.Creado);

			Map(c => c.Modificado);

			Map(c => c.CreadoPor);

			Map(c => c.ModificadoPor);

            Map(c => c.GrupoEmpresario);            
            
            Map(c => c.LoginKey);

        }
    }

    public class _MapGq_grupoEmpresario : ClassMap<Gq_grupoEmpresario>
    {
        public _MapGq_grupoEmpresario()
        {
            Table("gq_grupoEmpresario");

            Id(c => c.GrupoEmpresarioId).GeneratedBy.Identity();

            Map(c => c.Nombre).Length(255);

            Map(c => c.Descripcion).Length(255);

            Map(c => c.Estado).Length(1);

            Map(c => c.Creado);

            Map(c => c.Modificado);

            Map(c => c.CreadoPor);

            Map(c => c.ModificadoPor);

            Map(c => c.Limite);

        }
    }

    public class _MapGq_supuesto_grupo : ClassMap<Gq_supuesto_grupo>
    {
        public _MapGq_supuesto_grupo()
        {
            Table("gq_supuesto_grupo");

            Id(c => c.Id).GeneratedBy.Identity();
            Map(c => c.Nombre).Length(50);

        }
    }

    public class _MapGq_mailTemplate : ClassMap<Gq_mailTemplate>
    {
        public _MapGq_mailTemplate()
        {
            Table("gq_mailTemplate");

            Id(c => c.Id).GeneratedBy.Identity();

            Map(c => c.Nombre).Length(50);

            Map(c => c.Folder).Length(250);

            Map(c => c.Template).Length(int.MaxValue);

            Map(c => c.CodeSharp).Length(int.MaxValue);

            Map(c => c.Estado).Length(1);

            Map(c => c.Creado);

            Map(c => c.CreadoPor);

            Map(c => c.Modificado);

            Map(c => c.ModificadoPor);

        }
    }
}
