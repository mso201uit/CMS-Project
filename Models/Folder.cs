﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        // Navigasjons-egenskaper
        public int? ParentId { get; set; }
        public Folder? ParentFolder { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Folder> ChildrenFolders { get; set; } = new List<Folder>();
    }
}
