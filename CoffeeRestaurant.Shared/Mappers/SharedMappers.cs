using CoffeeRestaurant.Shared.DTOs;
using CoffeeRestaurant.Shared.Common;

namespace CoffeeRestaurant.Shared.Mappers;

public static class SharedMappers
{
    // Generic mapping extensions
    public static List<TDto> ToDtoList<TEntity, TDto>(this IEnumerable<TEntity> entities, Func<TEntity, TDto> mapper)
    {
        return entities.Select(mapper).ToList();
    }

    // Pagination mappings
    public static PagedResultDto<TDto> ToPagedResult<TEntity, TDto>(
        this IEnumerable<TEntity> entities, 
        Func<TEntity, TDto> mapper, 
        int pageNumber, 
        int pageSize, 
        int totalCount)
    {
        return new PagedResultDto<TDto>
        {
            Items = entities.ToDtoList(mapper),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    // Response mappings
    public static ApiResponse<TDto> ToSuccessResponse<TDto>(this TDto data, string message = "Success")
    {
        return ApiResponse<TDto>.SuccessResult(data, message);
    }

    public static ApiResponse<TDto> ToErrorResponse<TDto>(this string error, List<string>? errors = null)
    {
        return ApiResponse<TDto>.ErrorResult(error, errors);
    }

    // Validation mappings
    public static ValidationErrorDto ToValidationError(this string propertyName, string errorMessage)
    {
        return new ValidationErrorDto
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage
        };
    }

    public static List<ValidationErrorDto> ToValidationErrors(this Dictionary<string, List<string>> validationErrors)
    {
        return validationErrors
            .SelectMany(kvp => kvp.Value.Select(error => kvp.Key.ToValidationError(error)))
            .ToList();
    }

    // Search and filter mappings
    public static SearchResultDto<TDto> ToSearchResult<TEntity, TDto>(
        this IEnumerable<TEntity> entities, 
        Func<TEntity, TDto> mapper, 
        string searchTerm, 
        int totalCount)
    {
        return new SearchResultDto<TDto>
        {
            Items = entities.ToDtoList(mapper),
            SearchTerm = searchTerm,
            TotalCount = totalCount
        };
    }
}

// Additional DTOs for shared functionality
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class ValidationErrorDto
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class SearchResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public int TotalCount { get; set; }
}
