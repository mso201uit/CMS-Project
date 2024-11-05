namespace CMS_Project.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    
    // Keys
    public int UserId { get; set; }
    public User User { get; set; }

    public int ContentTypeId { get; set; }
    public ContentType ContentType { get; set; }

    public int? FolderId { get; set; }
    public Folder Folder { get; set; }

    public Document()
    {
        Created = DateTime.UtcNow;
    }
}