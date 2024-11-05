namespace CMS_Project.Models;

public class ContentType
{
    public int Id { get; set; }
    public string Type { get; set; }
    
    public ICollection<Document> Documents { get; set; }

    public ContentType()
    {
        Documents = new List<Document>();
    }
}