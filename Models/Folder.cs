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
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
