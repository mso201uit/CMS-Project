namespace CMS_Project.Models;

public class Folder
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Keys
    public int? ParentId { get; set; }
    public Folder Parent { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Folder> subFolders { get; set; }
    public ICollection<Document> documents { get; set; }

    public Folder()
    {
        subFolders = new List<Folder>();
        Documents = new List<Document>();
    }
}