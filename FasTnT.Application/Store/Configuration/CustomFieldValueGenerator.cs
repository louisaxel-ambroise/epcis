using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Diagnostics.CodeAnalysis;

namespace FasTnT.Infrastructure.Configuration
{
    internal class CustomFieldValueGenerator : ValueGenerator<int>
    {
        public override bool GeneratesTemporaryValues => false;

        public override int Next([NotNull] EntityEntry entry) => entry.Context.GetService<IdentityGenerator>().NextValue;
    }
}
