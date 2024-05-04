namespace ThesisApp.Models;

public class UpdateThesisRequest
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string DescriptionKg { get; set; }
    public string DescriptionTr { get; set; }
}