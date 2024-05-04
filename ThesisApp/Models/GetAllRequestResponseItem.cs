namespace ThesisApp.Models;

public class GetAllRequestResponseItem
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string StudentFirstname { get; set; }
    public string StudentLastname { get; set; }
    public string StudentPatronomyc { get; set; }
    public DateTime CreatedAt { get; set; }
}