
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_perfiles : _ServGq_perfiles
    {
    	#region Constructores

        public ServGq_perfiles(ISession session): base(session){}
        public ServGq_perfiles(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_perfiles(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
