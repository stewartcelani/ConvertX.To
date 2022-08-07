using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ConvertX.To.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // for PdfSharp to work with .NET Core

        // TODO: Add MediatR
    }
}