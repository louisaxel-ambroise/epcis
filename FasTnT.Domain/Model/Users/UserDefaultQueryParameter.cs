using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Model;

public class UserDefaultQueryParameter : QueryParameter
{
    public User User { get; set; }
}
