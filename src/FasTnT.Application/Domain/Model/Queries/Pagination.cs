namespace FasTnT.Application.Domain.Model.Queries;

public record Pagination(int PerPage, int StartFrom)
{
    public static Pagination Max => new(int.MaxValue, 0);
}
