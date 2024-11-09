using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; 

namespace CMS_Project.Models.DTOs
{
    public class UserResponseDto
    {
        [JsonPropertyName("userId")]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        
        public ICollection<FolderDto> Folders { get; set; } = new List<FolderDto>();
    }
}