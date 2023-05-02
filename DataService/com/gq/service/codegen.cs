
using MEMDataService.com.gq.domain;
using GQService.com.gq.service;
using NHibernate;
using GQDataService.com.gq.domain;

namespace MEMDataService.com.gq.service.codegen
{


	public class _ServGq_accesos : GenericService<Gq_accesos>
    {
    	#region Constructores

        public _ServGq_accesos(ISession session): base(session){}
        public _ServGq_accesos(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_accesos(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_escenarios : GenericService<Gq_escenarios>
    {
    	#region Constructores

        public _ServGq_escenarios(ISession session): base(session){}
        public _ServGq_escenarios(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_escenarios(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_grafico : GenericService<Gq_grafico>
    {
    	#region Constructores

        public _ServGq_grafico(ISession session): base(session){}
        public _ServGq_grafico(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_grafico(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_menu : GenericService<Gq_menu>
    {
    	#region Constructores

        public _ServGq_menu(ISession session): base(session){}
        public _ServGq_menu(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_menu(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_perfiles : GenericService<Gq_perfiles>
    {
    	#region Constructores

        public _ServGq_perfiles(ISession session): base(session){}
        public _ServGq_perfiles(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_perfiles(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_perfiles_accesos : GenericService<Gq_perfiles_accesos>
    {
    	#region Constructores

        public _ServGq_perfiles_accesos(ISession session): base(session){}
        public _ServGq_perfiles_accesos(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_perfiles_accesos(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_plugin : GenericService<Gq_plugin>
    {
    	#region Constructores

        public _ServGq_plugin(ISession session): base(session){}
        public _ServGq_plugin(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_plugin(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

    public class _ServGq_descargas : GenericService<Gq_descargas>
    {
        #region Constructores

        public _ServGq_descargas(ISession session) : base(session) { }
        public _ServGq_descargas(IStatelessSession statelessSession) : base(statelessSession) { }
        public _ServGq_descargas(ISession session, IStatelessSession statelessSession) : base(session, statelessSession) { }

        #endregion


    }

    public class _ServGq_smtp_config : GenericService<Gq_smtp_config>
    {
    	#region Constructores

        public _ServGq_smtp_config(ISession session): base(session){}
        public _ServGq_smtp_config(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_smtp_config(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_supuesto : GenericService<Gq_supuesto>
    {
    	#region Constructores

        public _ServGq_supuesto(ISession session): base(session){}
        public _ServGq_supuesto(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_supuesto(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_tipos_grafico : GenericService<Gq_tipos_grafico>
    {
    	#region Constructores

        public _ServGq_tipos_grafico(ISession session): base(session){}
        public _ServGq_tipos_grafico(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_tipos_grafico(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

	public class _ServGq_usuarios : GenericService<Gq_usuarios>
    {
    	#region Constructores

        public _ServGq_usuarios(ISession session): base(session){}
        public _ServGq_usuarios(IStatelessSession statelessSession): base(statelessSession){}
        public _ServGq_usuarios(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
             

    }

    public class _ServGq_grupoEmpresario : GenericService<Gq_grupoEmpresario>
    {
        #region Constructores

        public _ServGq_grupoEmpresario(ISession session) : base(session) { }
        public _ServGq_grupoEmpresario(IStatelessSession statelessSession) : base(statelessSession) { }
        public _ServGq_grupoEmpresario(ISession session, IStatelessSession statelessSession) : base(session, statelessSession) { }

        #endregion


    }

    public class _ServGq_supuesto_grupo : GenericService<Gq_supuesto_grupo>
    {
        #region Constructores

        public _ServGq_supuesto_grupo(ISession session) : base(session) { }
        public _ServGq_supuesto_grupo(IStatelessSession statelessSession) : base(statelessSession) { }
        public _ServGq_supuesto_grupo(ISession session, IStatelessSession statelessSession) : base(session, statelessSession) { }

        #endregion


    }

    public class _ServGq_mailTemplate : GenericService<Gq_mailTemplate>
    {
        #region Constructores

        public _ServGq_mailTemplate(ISession session) : base(session) { }
        public _ServGq_mailTemplate(IStatelessSession statelessSession) : base(statelessSession) { }
        public _ServGq_mailTemplate(ISession session, IStatelessSession statelessSession) : base(session, statelessSession) { }

        #endregion


    }
}
