using api_sk1_01efc.Models;
using Microsoft.EntityFrameworkCore;

namespace api_sk1_01efc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // fluent 
            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    ArticleId = 1,
                    Title = "First Article",
                    Content = "This is the content of the first article."
                },
                new Article
                {
                    ArticleId = 2,
                    Title = "Second Article",
                    Content = "This is the content of the second article."
                }
            );
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    CommentId = 1,
                    ArticleId = 1,
                    Text = "This is the first comment of the first article."
                },
                new Comment
                {
                    CommentId = 2,
                    ArticleId = 1,
                    Text = "This is the second comment of the first article."
                },
                new Comment
                {
                    CommentId = 3,
                    ArticleId = 2,
                    Text = "This is the first comment of the second article."
                }
            );
        }
    }
}
