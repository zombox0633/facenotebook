using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace utils.apiFormatResponse;


public class ApiFormatResponse<T>
{
  [JsonPropertyOrder(1)]
  public int Status { get; set; }
  
  [JsonPropertyOrder(2)]
  public string Path { get; set; } = string.Empty;
  
  [JsonPropertyOrder(3)]
  public DateTime Timestamp { get; set; }
  
  [JsonPropertyOrder(4)]
  public string Message { get; set; }
  
  [JsonPropertyOrder(5)]
  public T? Data { get; set; }

  public ApiFormatResponse(int status, string message, T? data = default, string path = "")
  {
    Status = status;
    Message = message;
    Data = data;
    Timestamp = DateTime.UtcNow;
    Path = path;
  }
}

public class ApiFormatErrorResponse : ApiFormatResponse<object?>
{
  public ApiFormatErrorResponse(int status, string message, string path = "") 
    : base(status, message, null, path)
  {
  }
}

public static class apiFormatResponse
{
  public static ActionResult<ApiFormatResponse<T>> Success<T>(T data, int statusCode = 200, string message = "OK")
  {
    return new ObjectResult(new ApiFormatResponse<T>(statusCode, message, data))
    {
      StatusCode = statusCode
    };
  }

  public static ActionResult<ApiFormatResponse<T>> Error<T>(string message, int statusCode = 400, IEnumerable<string>? validationErrors = null, string path = "")
  {
    var fullMessage = message;
    if (validationErrors != null && validationErrors.Any())
    {
      fullMessage = $"{message}: {string.Join(", ", validationErrors)}";
    }

    return new ObjectResult(new ApiFormatResponse<T>(statusCode, fullMessage, default, path))
    {
      StatusCode = statusCode
    };
  }

  public static ActionResult<ApiFormatResponse<T>> ValidationError<T>(ModelStateDictionary modelState,  string path = "")
  {
    var errors = modelState
      .Where(x => x.Value?.Errors.Count > 0)
      .SelectMany(x => x.Value!.Errors)
      .Select(x => x.ErrorMessage);

    return Error<T>("Invalid input provided", 400, errors, path);
  }
}