using api_sk1_02files.Models;
using Microsoft.EntityFrameworkCore;

namespace api_sk1_02files.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
