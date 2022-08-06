namespace ConvertX.To.API.Contracts.V1.Responses;

public class ErrorResponseModel
{
    public ErrorResponseModel()
    {
    }

    public ErrorResponseModel(string error, string message)
    {
        Error = error;
        Message = message;
    }

    public string Error { get; set; }
    public string Message { get; set; }
}