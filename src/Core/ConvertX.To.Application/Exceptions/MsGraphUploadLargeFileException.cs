using Microsoft.Graph;

namespace ConvertX.To.Application.Exceptions;

public class MsGraphUploadLargeFileException : ConvertXToExceptionBase
{
    public new static string Message => "Error uploading large file to Microsoft Graph.";
    public UploadResult<DriveItem> UploadResult { get;  }

    public MsGraphUploadLargeFileException(UploadResult<DriveItem> uploadResult) : base(Message)
    {
        UploadResult = uploadResult ?? throw new NullReferenceException(nameof(uploadResult));
    }
    
}