namespace FasTnT.Application.EfCore;

public record SqlProvider(string Name, string Assembly)
{
    public static SqlProvider SqlServer => new(nameof(SqlServer), typeof(Migrations.SqlServer.Marker).Assembly.GetName().Name);
    public static SqlProvider Postgres => new(nameof(Postgres), typeof(Migrations.Postgres.Marker).Assembly.GetName().Name);
    public static SqlProvider Sqlite => new(nameof(Sqlite), typeof(Migrations.Sqlite.Marker).Assembly.GetName().Name);

    public static implicit operator string(SqlProvider provider) => provider.Name;
}
