using ConvertX.To.API.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;



namespace ConvertX.To.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errorsInModelState = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)).ToArray();

            var errorResponse = new ErrorResponse();

            foreach (var error in errorsInModelState)
            foreach (var subError in error.Value)
            {
                var errorModel = new ErrorResponseModel
                {
                    Error = error.Key,
                    Message = subError
                };
                errorResponse.Errors.Add(errorModel);
            }

            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }
        
        await next();
    }
}