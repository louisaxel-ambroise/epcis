using FasTnT.Domain.Model.Queries;

namespace FasTnT.Domain.Model.Users;

public class UserDefaultQueryParameter : QueryParameter
{
    public User User { get; set; }
}
