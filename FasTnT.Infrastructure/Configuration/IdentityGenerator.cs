namespace FasTnT.Infrastructure.Configuration
{
    public class IdentityGenerator
    {
        private int _lastValue;

        public int NextValue => _lastValue += 1;
    }
}
