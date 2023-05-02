
using MEMDataService.com.gq.domain;
using System;
using System.Collections.Generic;
using GQService.com.gq.dto;
using GQDataService.com.gq.domain;
using GQDataService.com.gq.dto;

namespace MEMDataService.com.gq.dto.codegen
{


    public class _Gq_accesosDto : Dto<Gq_accesos,Gq_accesosDto>
    {
    	public _Gq_accesosDto() : base()
    	{
    	}
    	
    	public _Gq_accesosDto( Gq_accesos value) : base(value)
    	{
    	}

        public virtual System.Int64? AccesoId { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Descripcion { get; set; }


        public virtual System.Int64 Tipo { get; set; }


        public virtual System.String Clase { get; set; }


        public virtual System.String Metodo { get; set; }


        public virtual System.String Orden { get; set; }


    }


    public class _Gq_escenariosDto : Dto<Gq_escenarios,Gq_escenariosDto>
    {
    	public _Gq_escenariosDto() : base()
    	{
    	}
    	
    	public _Gq_escenariosDto( Gq_escenarios value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String BaseDatos { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


        public virtual System.String EsBase { get; set; }


        public virtual System.Int64? PluginId { get; set; }


        public virtual System.String Descripcion { get; set; }


        public virtual System.Int32? FechaInicio { get; set; }


        public virtual System.Int32? FechaFin { get; set; }


        public virtual System.Int64? EscenarioOrigenId { get; set; }


        public virtual System.Int32? SemanaInicio { get; set; }


        public virtual System.Int32? SemanaFin { get; set; }


        public virtual System.String Publico { get; set; }


        public virtual System.Int64? GrupoEmpresarioId { get; set; }
    }


    public class _Gq_graficoDto : Dto<Gq_grafico,Gq_graficoDto>
    {
    	public _Gq_graficoDto() : base()
    	{
    	}
    	
    	public _Gq_graficoDto( Gq_grafico value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Descripcion { get; set; }


        public virtual System.String Template { get; set; }


        public virtual System.String Scritp { get; set; }


        public virtual System.String CodeSharp { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime Modificado { get; set; }


        public virtual System.Int64 ModificadoPor { get; set; }


        public virtual System.String Folder { get; set; }


        public virtual System.Int64 TipoId { get; set; }


        public virtual System.Int64 TipoSeleccion { get; set; }


    }


    public class _Gq_menuDto : Dto<Gq_menu,Gq_menuDto>
    {
    	public _Gq_menuDto() : base()
    	{
    	}
    	
    	public _Gq_menuDto( Gq_menu value) : base(value)
    	{
    	}

        public virtual System.Int64? MenuId { get; set; }


        public virtual System.String MenuPosition { get; set; }


        public virtual System.String MenuUrl { get; set; }


        public virtual System.String MenuIcono { get; set; }


        public virtual System.String KeyName { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String MenuPadre { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime? Creado { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? CreadoPor { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


    }


    public class _Gq_perfilesDto : Dto<Gq_perfiles,Gq_perfilesDto>
    {
    	public _Gq_perfilesDto() : base()
    	{
    	}
    	
    	public _Gq_perfilesDto( Gq_perfiles value) : base(value)
    	{
    	}

        public virtual System.Int64? PerfilId { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String KeyName { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime? Creado { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? CreadoPor { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


    }


    public class _Gq_perfiles_accesosDto : Dto<Gq_perfiles_accesos,Gq_perfiles_accesosDto>
    {
    	public _Gq_perfiles_accesosDto() : base()
    	{
    	}
    	
    	public _Gq_perfiles_accesosDto( Gq_perfiles_accesos value) : base(value)
    	{
    	}

        public virtual System.Int64? PerfilesAccesosId { get; set; }


        public virtual System.Int64 PerfilId { get; set; }


        public virtual System.Int64 AccesoId { get; set; }


        public virtual System.String GrantPermition { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime? Creado { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? CreadoPor { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


    }


    public class _Gq_pluginDto : Dto<Gq_plugin,Gq_pluginDto>
    {
    	public _Gq_pluginDto() : base()
    	{
    	}
    	
    	public _Gq_pluginDto( Gq_plugin value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Tipo { get; set; }


        public virtual System.String Folder { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


    }


    public class _Gq_descargasDto : Dto<Gq_descargas, Gq_descargasDto>
    {
        public _Gq_descargasDto() : base()
        {
        }

        public _Gq_descargasDto(Gq_descargas value) : base(value)
        {
        }

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String NombreArchivo { get; set; }


        public virtual System.String Folder { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


    }


    public class _Gq_smtp_configDto : Dto<Gq_smtp_config,Gq_smtp_configDto>
    {
    	public _Gq_smtp_configDto() : base()
    	{
    	}
    	
    	public _Gq_smtp_configDto( Gq_smtp_config value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String NombreFrom { get; set; }


        public virtual System.String Pass { get; set; }


        public virtual System.String Host { get; set; }


        public virtual System.Int32 Port { get; set; }


        public virtual System.Boolean UseDefaultCredentials { get; set; }


        public virtual System.Boolean EnableSsl { get; set; }


        public virtual System.String EMailFrom { get; set; }


    }


    public class _Gq_supuestoDto : Dto<Gq_supuesto,Gq_supuestoDto>
    {
    	public _Gq_supuestoDto() : base()
    	{
    	}
    	
    	public _Gq_supuestoDto( Gq_supuesto value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Descripcion { get; set; }


        public virtual System.String Folder { get; set; }


        public virtual System.String Template { get; set; }


        public virtual System.String Scritp { get; set; }


        public virtual System.String CodeSharp { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


        public virtual System.Int64 Grupo { get; set; }


        public virtual System.String TablaMod { get; set; }


        public virtual System.Int64 Depende { get; set; }

        
        public virtual System.Int64 Orden { get; set; }


        public virtual System.String TablaModNombre { get; set; }
    }


    public class _Gq_tipos_graficoDto : Dto<Gq_tipos_grafico,Gq_tipos_graficoDto>
    {
    	public _Gq_tipos_graficoDto() : base()
    	{
    	}
    	
    	public _Gq_tipos_graficoDto( Gq_tipos_grafico value) : base(value)
    	{
    	}

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


    }


    public class _Gq_usuariosDto : Dto<Gq_usuarios,Gq_usuariosDto>
    {
    	public _Gq_usuariosDto() : base()
    	{
    	}
    	
    	public _Gq_usuariosDto( Gq_usuarios value) : base(value)
    	{
    	}

        public virtual System.Int64? UsuarioId { get; set; }


        public virtual System.String Usuario { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Apellido { get; set; }


        public virtual System.String Email { get; set; }


        public virtual System.String Clave { get; set; }


        public virtual System.Int64 PerfilId { get; set; }


        public virtual System.String RequiereClave { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime? Creado { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? CreadoPor { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


        public virtual System.Int64 GrupoEmpresario { get; set; }
        

        public virtual System.String LoginKey { get; set; }

    }

    public class _Gq_grupoEmpresarioDto : Dto<Gq_grupoEmpresario, Gq_grupoEmpresarioDto>
    {
        public _Gq_grupoEmpresarioDto() : base()
        {
        }

        public _Gq_grupoEmpresarioDto(Gq_grupoEmpresario value) : base(value)
        {
        }

        public virtual System.Int64? GrupoEmpresarioId { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Descripcion { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime? Creado { get; set; }


        public virtual System.DateTime? Modificado { get; set; }


        public virtual System.Int64? CreadoPor { get; set; }


        public virtual System.Int64? ModificadoPor { get; set; }


        public virtual System.Int64? Limite { get; set; }

    }

    public class _Gq_supuesto_grupoDto : Dto<Gq_supuesto_grupo, Gq_supuesto_grupoDto>
    {
        public _Gq_supuesto_grupoDto() : base()
        {
        }

        public _Gq_supuesto_grupoDto(Gq_supuesto_grupo value) : base(value)
        {
        }

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


    }

    public class _Gq_mailTemplateDto : Dto<Gq_mailTemplate, Gq_mailTemplateDto>
    {
        public _Gq_mailTemplateDto() : base()
        {
        }

        public _Gq_mailTemplateDto(Gq_mailTemplate value) : base(value)
        {
        }

        public virtual System.Int64? Id { get; set; }


        public virtual System.String Nombre { get; set; }


        public virtual System.String Folder { get; set; }


        public virtual System.String Template { get; set; }


        public virtual System.String CodeSharp { get; set; }


        public virtual System.String Estado { get; set; }


        public virtual System.DateTime Creado { get; set; }


        public virtual System.Int64 CreadoPor { get; set; }


        public virtual System.DateTime Modificado { get; set; }


        public virtual System.Int64 ModificadoPor { get; set; }


    }
}
