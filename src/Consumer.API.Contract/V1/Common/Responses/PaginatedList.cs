using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common.Responses;

public record PaginatedList<TEntity> where TEntity : class
{
    public PaginatedList()
    {
    }
    
    [SetsRequiredMembers]
    public PaginatedList(
        int pageIndex, 
        int pageSize, 
        long total, 
        IEnumerable<TEntity> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Total = total;
        Data = data;
    }

    public required int PageIndex { get; init; }
    public required int PageSize { get; init; }
    public required long Total { get;  init; }
    public required IEnumerable<TEntity> Data { get; init; }
}