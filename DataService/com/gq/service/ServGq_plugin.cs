
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_plugin : _ServGq_plugin
    {
    	#region Constructores

        public ServGq_plugin(ISession session): base(session){}
        public ServGq_plugin(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_plugin(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
