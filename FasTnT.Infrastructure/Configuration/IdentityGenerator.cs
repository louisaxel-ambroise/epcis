namespace FasTnT.Infrastructure.Configuration
{
    public class IdentityGenerator
    {
        private int _lastValue = 0;

        public int NextValue => _lastValue += 1;
    }
}
