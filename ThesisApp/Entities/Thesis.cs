namespace ThesisApp.Entities;

public class Thesis
{
    public long Id { get; set; }
    public string TitleKg { get; set; }
    public string TitleTr { get; set; }
    public string DescriptionKg { get; set; }
    public string DescriptionTr { get; set; }
    public long CuratorId { get; set; }
    public int DepartmentId { get; set; }
    public bool IsChosen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}