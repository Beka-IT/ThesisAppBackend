namespace ThesisApp.Entities;

public class Deadline
{
    public long Id { get; set; }
    public DateTime EndDate { get; set; }
    public string AcademicYear { get; set; }
    public int DepartmentId { get; set; }
}