﻿using CMS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS_Project.Data
{
    public class CMSContext : DbContext
    {
        public CMSContext(DbContextOptions<CMSContext> options) : base(options)
        {
        }

        // DbSet Properties
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<ContentType> ContentTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurer relasjoner og sletteatferd
            
            // Document -> User (Restrict delete)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Document -> Folder (Cascade delete)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Folder)
                .WithMany(f => f.Documents)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Folder -> ParentFolder (Cascade delete)
            modelBuilder.Entity<Folder>()
                .HasMany(f => f.ChildrenFolders)
                .WithOne(f => f.ParentFolder)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Folder -> User (Restrict delete)
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany(u => u.Folders)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
