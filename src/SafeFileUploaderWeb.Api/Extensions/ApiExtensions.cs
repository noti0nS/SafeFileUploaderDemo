using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Api.Extensions;

internal static class ApiExtensions
{
    public static IResult ToHttpResult<T>(this ApiResponse<T> response) 
    {
        if (response.IsSuccess) return TypedResults.Ok(response);
        return TypedResults.Json(response, statusCode: (int)response.Code);
    }
}