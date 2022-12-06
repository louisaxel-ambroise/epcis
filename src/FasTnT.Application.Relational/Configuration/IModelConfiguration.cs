using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Relational.Configuration;

public interface IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder);
}
