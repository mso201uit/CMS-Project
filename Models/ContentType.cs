using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models
{
    public class ContentType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Type { get; set; } = String.Empty;
        
    }
}