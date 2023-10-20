namespace ThesisApp.Entities;

public class Faculty
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ICollection<Department> Departments { get; set; }
    public ICollection<User> Students { get; set; }
}