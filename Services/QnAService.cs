using Microsoft.EntityFrameworkCore;
using ERP_QNA_WebApp.Data;
using ERP_QNA_WebApp.Models;

namespace ERP_QNA_WebApp.Services;

public class QnAService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public QnAService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<QnA>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QnAs.OrderByDescending(q => q.CreatedAt).ToListAsync();
    }

    public async Task<List<QnA>> SearchAsync(string? keyword, string? tags)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.QnAs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(q =>
                q.Question.ToLower().Contains(lowerKeyword) ||
                q.Answer.ToLower().Contains(lowerKeyword) ||
                (q.Remark != null && q.Remark.ToLower().Contains(lowerKeyword)));
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var lowerTags = tags.ToLower();
            query = query.Where(q => q.Tags != null && q.Tags.ToLower().Contains(lowerTags));
        }

        return await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
    }

    public async Task<QnA?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QnAs.FindAsync(id);
    }

    public async Task<QnA> CreateAsync(QnA qna)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        qna.CreatedAt = DateTime.UtcNow;
        context.QnAs.Add(qna);
        await context.SaveChangesAsync();
        return qna;
    }

    public async Task<QnA?> UpdateAsync(QnA qna)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.QnAs.FindAsync(qna.Id);
        if (existing == null) return null;

        existing.Question = qna.Question;
        existing.Answer = qna.Answer;
        existing.Tags = qna.Tags;
        existing.Remark = qna.Remark;
        existing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var qna = await context.QnAs.FindAsync(id);
        if (qna == null) return false;

        context.QnAs.Remove(qna);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> ImportFromExcelAsync(List<QnA> qnas)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var qna in qnas)
        {
            qna.CreatedAt = DateTime.UtcNow;
        }
        context.QnAs.AddRange(qnas);
        return await context.SaveChangesAsync();
    }
}
