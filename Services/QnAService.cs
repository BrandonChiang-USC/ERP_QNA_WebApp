using Dapper;
using ERP_QNA_WebApp.Data;
using ERP_QNA_WebApp.Models;

namespace ERP_QNA_WebApp.Services;

public class QnAService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public QnAService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<QnA>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT id AS Id, question AS Question, answer AS Answer, tags AS Tags, remark AS Remark, created_at AS CreatedAt, updated_at AS UpdatedAt FROM ERP_QNA ORDER BY created_at DESC";
        var result = await connection.QueryAsync<QnA>(sql);
        return result.ToList();
    }

    public async Task<List<QnA>> SearchAsync(string? keyword, string? tags)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "SELECT id AS Id, question AS Question, answer AS Answer, tags AS Tags, remark AS Remark, created_at AS CreatedAt, updated_at AS UpdatedAt FROM ERP_QNA WHERE 1=1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            if (_connectionFactory.DatabaseProvider == "PostgreSQL")
            {
                sql += " AND (LOWER(question) LIKE @Keyword OR LOWER(answer) LIKE @Keyword OR LOWER(COALESCE(remark, '')) LIKE @Keyword)";
            }
            else
            {
                sql += " AND (LOWER(question) LIKE @Keyword OR LOWER(answer) LIKE @Keyword OR LOWER(ISNULL(remark, '')) LIKE @Keyword)";
            }
            parameters.Add("Keyword", $"%{keyword.ToLower()}%");
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            if (_connectionFactory.DatabaseProvider == "PostgreSQL")
            {
                sql += " AND LOWER(COALESCE(tags, '')) LIKE @Tags";
            }
            else
            {
                sql += " AND LOWER(ISNULL(tags, '')) LIKE @Tags";
            }
            parameters.Add("Tags", $"%{tags.ToLower()}%");
        }

        sql += " ORDER BY created_at DESC";

        var result = await connection.QueryAsync<QnA>(sql, parameters);
        return result.ToList();
    }

    public async Task<QnA?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT id AS Id, question AS Question, answer AS Answer, tags AS Tags, remark AS Remark, created_at AS CreatedAt, updated_at AS UpdatedAt FROM ERP_QNA WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<QnA>(sql, new { Id = id });
    }

    public async Task<QnA> CreateAsync(QnA qna)
    {
        using var connection = _connectionFactory.CreateConnection();
        qna.CreatedAt = DateTime.UtcNow;

        string sql;
        if (_connectionFactory.DatabaseProvider == "PostgreSQL")
        {
            sql = """
                INSERT INTO ERP_QNA (question, answer, tags, remark, created_at)
                VALUES (@Question, @Answer, @Tags, @Remark, @CreatedAt)
                RETURNING id
                """;
        }
        else
        {
            sql = """
                INSERT INTO ERP_QNA (question, answer, tags, remark, created_at)
                VALUES (@Question, @Answer, @Tags, @Remark, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT)
                """;
        }

        qna.Id = await connection.ExecuteScalarAsync<int>(sql, qna);
        return qna;
    }

    public async Task<QnA?> UpdateAsync(QnA qna)
    {
        using var connection = _connectionFactory.CreateConnection();

        var existing = await GetByIdAsync(qna.Id);
        if (existing == null) return null;

        qna.UpdatedAt = DateTime.UtcNow;

        var sql = """
            UPDATE ERP_QNA SET
                question = @Question,
                answer = @Answer,
                tags = @Tags,
                remark = @Remark,
                updated_at = @UpdatedAt
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(sql, qna);
        return qna;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "DELETE FROM ERP_QNA WHERE id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<int> ImportFromExcelAsync(List<QnA> qnas)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var now = DateTime.UtcNow;
            var sql = """
                INSERT INTO ERP_QNA (question, answer, tags, remark, created_at)
                VALUES (@Question, @Answer, @Tags, @Remark, @CreatedAt)
                """;

            var count = 0;
            foreach (var qna in qnas)
            {
                qna.CreatedAt = now;
                await connection.ExecuteAsync(sql, qna, transaction);
                count++;
            }

            transaction.Commit();
            return count;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
