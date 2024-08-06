namespace SE170311.Lab3.Payload.Response;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

public class BasicResponse
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }

}