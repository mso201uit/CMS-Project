﻿using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models
{
    public class User
    {   
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        
        public ICollection<Document> Documents { get; set; }  = new List<Document>();
        
    }
}
