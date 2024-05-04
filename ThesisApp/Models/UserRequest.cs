using ThesisApp.Enums;

namespace ThesisApp.Models;

public class UserRequest
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Patronomyc { get; set; }
    public int FacultyId { get; set; }
    public int DepartmentId { get; set; }
    public bool IsVerified { get; set; } = false;
    public string PhoneNumber { get; set; }
    public UserType Role { get; set; }
    public string Token { get; set; }
    public DateTime? Deadline { get; set; }
    public long? ChosenThesisId { get; set; }
}