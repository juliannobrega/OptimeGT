
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_supuesto_grupo : _ServGq_supuesto_grupo
    {
    	#region Constructores

        public ServGq_supuesto_grupo(ISession session): base(session){}
        public ServGq_supuesto_grupo(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_supuesto_grupo(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
