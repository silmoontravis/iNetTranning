using iNet.Service.Domain;

namespace handlang.service.Domain
{
    public class DmzContainer : DmzModelContainer
    {
        public DmzContainer()
            : base(new iNetConn().getConnectionString("Domain.DmzModel"))
        {

        }
    }

    public partial class DmzModelContainer
    {
        public DmzModelContainer(string _ConnectionString) : base(_ConnectionString)
        {

        }
    }
}
