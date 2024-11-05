using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Folder> Folders { get; set; }

        public User()
        {
            Documents = new List<Document>();
            Folders = new List<Folder>();
            CreatedDate = DateTime.UtcNow;
        }
    }
}
