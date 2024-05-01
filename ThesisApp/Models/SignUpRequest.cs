namespace ThesisApp.Models
{
    public class SignUpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronomyc { get; set; }
        public int FacultyId { get; set; }
        public int DepartmentId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
