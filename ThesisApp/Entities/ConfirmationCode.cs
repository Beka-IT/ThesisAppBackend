namespace ThesisApp.Entities;


public class ConfirmationCode
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int Code { get; set; }
}