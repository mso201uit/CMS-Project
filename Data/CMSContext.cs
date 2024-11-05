using Microsoft.EntityFrameworkCore;

namespace CMS_Project.Data
{
    public class CMSContext : DbContext
    {
        protected override async void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public CMSContext(DbContextOptions<CMSContext> options)
        : base(options)
        {
        }
    }
}
