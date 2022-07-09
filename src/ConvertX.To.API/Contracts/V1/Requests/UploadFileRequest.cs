namespace ConvertX.To.API.Contracts.V1.Requests;

public class UploadFileRequest
{
    public string SessionId { get; set; }
    public string FileId { get; set; }
    public string ConvertTo { get; set; }
}