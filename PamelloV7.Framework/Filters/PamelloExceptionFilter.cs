using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Filters
{
    public class PamelloExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }
        public void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception is PamelloException x) {
                context.Result = new BadRequestObjectResult(x.Message);
                context.ExceptionHandled = true;
            }
        }
    }
}
