using Microsoft.AspNetCore.Mvc;

namespace utils.apiFormatResponse;


public class ApiFormatResponse<T>
{
  public int Status { get; set; }
  public string Message { get; set; }
  public T? Data { get; set; }
  public ApiFormatResponse(int status, string message, T? data = default)
  {
    Status = status;
    Message = message;
    Data = data;
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
}