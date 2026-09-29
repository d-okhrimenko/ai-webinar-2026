using Microsoft.EntityFrameworkCore;
using ReviewsAssistant.Core.Reviews;

namespace ReviewsAssistant.Infrastructure.Data;

public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : DbContext(options)
{
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var review = modelBuilder.Entity<Review>();
        review.HasKey(item => item.Id);
        review.Property(item => item.AuthorName).HasMaxLength(150).IsRequired();
        review.Property(item => item.Email).HasMaxLength(320).IsRequired();
        review.Property(item => item.Text).HasMaxLength(4000).IsRequired();
        review.Property(item => item.Summary).HasMaxLength(1000);
        review.Property(item => item.AiAnalysisProvider).HasMaxLength(100);
        review.Property(item => item.AiAnalysisModel).HasMaxLength(100);
        review.Property(item => item.AiDraftResponse).HasMaxLength(4000);
        review.Property(item => item.AiDraftResponseProvider).HasMaxLength(100);
        review.Property(item => item.AiDraftResponseModel).HasMaxLength(100);
        review.HasIndex(item => item.CreatedAtUtc);
    }
}
