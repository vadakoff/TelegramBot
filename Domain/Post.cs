namespace Domain;

public class Post
{
    public int? PostId { set; get; }
    public string? Slug { set; get; }
    public string? Title { set; get; }
    public string? Year { set; get; }
    public string? Developer { set; get; }
    public string? Version { set; get; }
    public string? Language { set; get; }
    public string? Tags { set; get; }
    public long? Size { set; get; }
    public bool? Flag { set; get; }
}