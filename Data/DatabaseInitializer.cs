using Dapper;

namespace ERP_QNA_WebApp.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var sql = _connectionFactory.DatabaseProvider switch
        {
            "PostgreSQL" => GetPostgreSqlScript(),
            "MSSQL" => GetMsSqlScript(),
            _ => throw new NotSupportedException($"Database provider '{_connectionFactory.DatabaseProvider}' is not supported.")
        };

        await connection.ExecuteAsync(sql);
    }

    private static string GetPostgreSqlScript()
    {
        return """
            CREATE TABLE IF NOT EXISTS ERP_QNA (
                id SERIAL PRIMARY KEY,
                question VARCHAR(2000) NOT NULL,
                answer TEXT NOT NULL,
                tags VARCHAR(500),
                remark VARCHAR(1000),
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP WITH TIME ZONE
            );
            """;
    }

    private static string GetMsSqlScript()
    {
        return """
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ERP_QNA' AND xtype='U')
            CREATE TABLE ERP_QNA (
                id INT IDENTITY(1,1) PRIMARY KEY,
                question NVARCHAR(2000) NOT NULL,
                answer NVARCHAR(MAX) NOT NULL,
                tags NVARCHAR(500),
                remark NVARCHAR(1000),
                created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                updated_at DATETIME2
            );
            """;
    }
}
