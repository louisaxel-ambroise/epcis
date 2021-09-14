namespace FasTnT.Domain.Model
{
    public class UserDefaultQueryParameter
    {
        public User User { get; set; }
        public string Name { get; set; }
        public string[] Values { get; set; }
    }
}
