namespace ThesisApp.Models;

public class CreateRequestToThesisRequest
{
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string DescriptionKg { get; set; }
    public string DescriptionTr { get; set; }
    public long CuratorId { get; set; }
    public bool IsMyTheme { get; set; }
}