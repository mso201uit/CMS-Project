using System;
using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models.DTOs
{
    public class FolderDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        //[Required] can't be used unless you give them start folder from the begining
        public int? ParentFolderId { get; set; }
         
        [Required]
        public int UserId { get; set; }
    }
}