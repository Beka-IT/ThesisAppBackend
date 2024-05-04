namespace ThesisApp.Entities;

public class Notification
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public long? SentToId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}