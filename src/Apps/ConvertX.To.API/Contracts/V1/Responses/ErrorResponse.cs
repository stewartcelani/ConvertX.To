namespace ConvertX.To.API.Contracts.V1.Responses;

public class ErrorResponse
{
    public List<ErrorResponseModel> Errors { get; set; } = new();
}