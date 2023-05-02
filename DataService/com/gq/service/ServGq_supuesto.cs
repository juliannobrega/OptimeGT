
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_supuesto : _ServGq_supuesto
    {
    	#region Constructores

        public ServGq_supuesto(ISession session): base(session){}
        public ServGq_supuesto(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_supuesto(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
