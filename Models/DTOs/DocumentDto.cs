using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models.DTOs
{
    public class DocumentDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FolderId { get; set; }
    }
}