using Microsoft.EntityFrameworkCore;
using ERP_QNA_WebApp.Models;

namespace ERP_QNA_WebApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<QnA> QnAs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QnA>(entity =>
        {
            entity.ToTable("qna");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Question).HasColumnName("question").IsRequired();
            entity.Property(e => e.Answer).HasColumnName("answer").IsRequired();
            entity.Property(e => e.Tags).HasColumnName("tags");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });
    }
}
