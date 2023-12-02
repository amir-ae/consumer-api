namespace Consumer.API.Contract.V1.Common.Responses;

public record PaginatedList<TEntity>(int PageIndex, 
    int PageSize, 
    long Total, 
    IList<TEntity> Data) where TEntity : class;