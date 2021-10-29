using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Diagnostics.CodeAnalysis;

namespace FasTnT.Infrastructure.Configuration
{
    public sealed class IncrementGenerator : ValueGenerator<int>
    {
        public override bool GeneratesTemporaryValues => false;
        public override int Next([NotNull] EntityEntry entry) => entry.Context.GetService<Identity>().NextValue;

        public sealed class Identity
        {
            private int _lastValue;

            public int NextValue => Interlocked.Increment(ref _lastValue);
        }
    }
}
