using ThesisApp.Enums;

namespace ThesisApp.Entities;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Patronomyc { get; set; }
    public int FacultyId { get; set; }
    public int DepartmentId { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public UserType Role { get; set; }
    //public long? SelectedThesisId { get; set; } 
    //public Thesis? SelectedThesis { get; set; } = null!; 
    public long? RecommendedThesisId { get; set; } 
    public Thesis? RecommendedThesis { get; set; } = null!; 
}

