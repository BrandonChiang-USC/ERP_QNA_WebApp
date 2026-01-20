using System.Data;

namespace ERP_QNA_WebApp.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    string DatabaseProvider { get; }
}
