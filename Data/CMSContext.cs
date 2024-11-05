﻿using CMS_Project.Models;
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

        }
    }
}
