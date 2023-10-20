namespace ThesisApp.Entities;
public class Department
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int FacultyId { get; set; }
    public Faculty Faculty { get; set; }
    public ICollection<User> Students { get; set; }
}