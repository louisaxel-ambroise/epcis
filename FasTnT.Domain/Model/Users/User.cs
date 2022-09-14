namespace FasTnT.Domain.Model.Users;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] Salt { get; set; }
    public string SecuredKey { get; set; }
    public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;
    public bool CanCapture { get; set; }
    public bool CanQuery { get; set; }
    public List<UserDefaultQueryParameter> DefaultQueryParameters { get; set; } = new();
}
