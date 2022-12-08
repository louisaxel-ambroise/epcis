using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Configuration;

public interface IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder);
}
