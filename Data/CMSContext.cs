using CMS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS_Project.Data
{
    public class CMSContext : DbContext
    {
        public DbSet<ContentType>? ContentTypes { get; set; }
        public DbSet<Document>? Documents { get; set; }
        public DbSet<Folder>? Folders { get; set; }
        public DbSet<User>? Users { get; set; }
        
        public CMSContext(DbContextOptions<CMSContext> options) : base(options) {}
        
        protected override async void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //SEEDING

            //Users
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Username="Bjørn", Password="test123", Email="test@test.com", CreatedDate= new DateTime(2020,1,1)  });
            modelBuilder.Entity<User>().HasData(new User { Id = 2, Username = "Said", Password = "test123", Email = "test@test.com", CreatedDate = new DateTime(2020, 1, 1) });
            modelBuilder.Entity<User>().HasData(new User { Id = 3, Username = "Morten", Password = "test123", Email = "test@test.com", CreatedDate = new DateTime(2020, 1, 1) });

            //ContentTypes
            modelBuilder.Entity<ContentType>().HasData(new ContentType { Id = 1, Type="Text" });
            modelBuilder.Entity<ContentType>().HasData(new ContentType { Id = 2, Type = "Url" });
            modelBuilder.Entity<ContentType>().HasData(new ContentType { Id = 3, Type = "Picture" });

            //Folder
            modelBuilder.Entity<Folder>().HasData(new Folder { Id = 1, Name = "Bjørn", UserId=1 });
            modelBuilder.Entity<Folder>().HasData(new Folder { Id = 2, Name = "Media", UserId = 1 , ParentId=1});
            modelBuilder.Entity<Folder>().HasData(new Folder { Id = 3, Name = "Said", UserId = 2 });
            modelBuilder.Entity<Folder>().HasData(new Folder { Id = 4, Name = "Morten", UserId = 3 });

            //Document
            modelBuilder.Entity<Document>().HasData(new Document { Id = 1, Title = "Test Text", Content = "This is a Test Text.", Created = new DateTime(2020, 1, 1), FolderId = 4, UserId = 3, ContentTypeId=1});
            modelBuilder.Entity<Document>().HasData(new Document { Id = 2, Title = "Test Url", Content = "https://uit.instructure.com", Created = new DateTime(2020, 1, 1), FolderId = 3, UserId = 2, ContentTypeId = 2 });
        }
    }
}
