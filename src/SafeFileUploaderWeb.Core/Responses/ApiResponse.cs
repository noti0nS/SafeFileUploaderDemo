using System.Net;
using System.Text.Json.Serialization;

namespace SafeFileUploaderWeb.Core.Responses;

public class ApiResponse<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }
    
    [JsonIgnore]
    public bool IsSuccess => string.IsNullOrWhiteSpace(Message);

    [JsonIgnore]
    public HttpStatusCode Code { get; set; }
    
    [JsonConstructor]
    public ApiResponse()
    {
        Code = HttpStatusCode.OK;
    }

    private ApiResponse(T? data, string? message, HttpStatusCode code)
    {
        Data = data;
        Message = message;
        Code = code;
    }

    public static ApiResponse<T> Success(T data) => new(data, null, HttpStatusCode.OK);

    public static ApiResponse<T> Fail(string message, HttpStatusCode code = HttpStatusCode.InternalServerError) 
        => new(default, message, code);
}
