namespace ThesisApp.Models;

public class GetRequestResponse
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string DescriptionKg { get; set; }
    public string DescriptionTr { get; set; }
    public long CuratorId { get; set; }
    public string CuratorFirstname { get; set; }
    public string CuratorLastname { get; set; }
    public string CuratorPatronomyc { get; set; }
    public long StudentId { get; set; }
    public string StudentFirstname { get; set; }
    public string StudentLastname { get; set; }
    public string StudentPatronomyc { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool IsMyTheme { get; set; }
}