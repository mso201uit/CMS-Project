using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS_Project.Models.DTOs
{
    public class FolderDto
    {
        public int? Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    
        public DateTime? CreatedDate { get; set; }
        
        public int? ParentFolderId { get; set; }
        
        public List<FolderDto> ChildrenFolders { get; set; } = new List<FolderDto>();
    }
}