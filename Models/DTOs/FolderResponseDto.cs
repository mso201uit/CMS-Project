namespace CMS_Project.Models.DTOs
{
    public class FolderResponseDto
    {
        public string Name { get; set; }             
        public DateTime CreatedDate { get; set; }    
        public int? ParentFolderId { get; set; }     
        public List<FolderResponseDto> ChildrenFolders { get; set; } = new();
    }
}
