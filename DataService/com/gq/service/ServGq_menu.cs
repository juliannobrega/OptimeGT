
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_menu : _ServGq_menu
    {
    	#region Constructores

        public ServGq_menu(ISession session): base(session){}
        public ServGq_menu(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_menu(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
