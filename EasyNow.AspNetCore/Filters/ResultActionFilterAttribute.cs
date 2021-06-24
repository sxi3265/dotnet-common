using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyNow.AspNetCore.Filters
{
    public class ResultActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                context.Result = new ObjectResult(new { code = 0, msg = string.Empty, data = objectResult.Value });
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new { code = 0, msg = string.Empty });
            }
            else if (context.Result is ContentResult contentResult)
            {
                context.Result = new ObjectResult(new { code = 0, msg = string.Empty, data = contentResult.Content });
            }
            else if (context.Result is StatusCodeResult)
            {
                context.Result = new ObjectResult(new { code = 0,  msg = string.Empty });
            }
            else if (context.Result is JsonResult jsonResult)
            {
                context.Result = new ObjectResult(new { code = 0, msg = string.Empty, data = jsonResult.Value });
            }
        }
    }
}
