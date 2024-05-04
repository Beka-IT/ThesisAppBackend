namespace ThesisApp.Models;

public class GetThesisResponse
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string DescriptionKg { get; set; }
    public string DescriptionTr { get; set; }
    public string CuratorFirstname { get; set; }
    public string CuratorLastname { get; set; }
    public string CuratorPatronomyc { get; set; }
    public bool IsChosen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<StudentResponse> ChosenBy { get; set; }
}