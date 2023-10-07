using iNet.Helper;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace iNet.Service.Domain
{
    public class iNetConn
    {
        public delegate string EntityConn(string connectionString, string domain);
        private EntityConn getConn;

        public iNetConn()
        {
            getConn = getEntityConnection;
        }

        public iNetConn(EntityConn getEntityConnection)
        {
            getConn = getEntityConnection;
        }

        public string getConnectionString(string domain)
        {
            var conn = ConfigurationManager.ConnectionStrings["DB_Dmz"].ConnectionString;
            return getConn(EncryptHelper.dec(conn), domain);
        }

        public string getEntityConnection(string connectionString, string domain)
        {
            try
            {
                EntityConnectionStringBuilder ecsb = new EntityConnectionStringBuilder();
                ecsb.Metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", domain);
                ecsb.Provider = "System.Data.SqlClient"; ////不可省略
                ecsb.ProviderConnectionString = connectionString;
                string conn = ecsb.ToString();

                return conn;
            }
            catch
            {
                throw;
            }
        }
    }
}
