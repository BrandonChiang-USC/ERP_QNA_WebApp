using System.Data;
using Microsoft.Data.SqlClient;

namespace ERP_QNA_WebApp.Data;

public class MsSqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MsSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string DatabaseProvider => "MSSQL";

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
