namespace CMS_Project.Models.DTOs
{
    public class DocumentResponseDto
    {
        public FolderDto Folder { get; set; } = null!;
        public DocumentDetailDto Document { get; set; } = null!;
    }
}