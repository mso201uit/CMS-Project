using System;
using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models.DTOs
{
    public class FolderDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        //[Required] if this is required, then the user needs to get a root folder to put everything else under, when their account get created.
        public int ParentId { get; set; }
         
        [Required]
        public int UserId { get; set; }
    }
}