namespace SheetsProcessorApi.DTOs;


public class ApiResponse<T>
{
    public bool Success {get; set;}
    public string? Message {get; set;}
    public T? Data {get; set;}

    public List<string>? Errors {get; set;}

    private ApiResponse(){}

    public static ApiResponse<T> Ok(T data, string message = "Operação realizada com sucesso")
    {
        
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> Fail(T? data, string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data,
            Errors = errors ?? new List<string>()
        };
    }

}
