using EasyNow.Dto.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyNow.AspNetCore.Filters
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var environment = context.HttpContext.RequestServices.GetService<IHostEnvironment>();

            // 标记该异常已被处理
            context.ExceptionHandled = true;
            if (context.Exception is MessageException exception)
            {
                if (environment.IsDevelopment())
                {
                    context.Result = new ObjectResult(new
                        { code = -1, msg = exception.Message, ex = exception,data=exception.Data });
                }
                else
                {
                    context.Result = new ObjectResult(new
                        { code = -1, msg = exception.Message, data = exception.Data });
                }
            }
            else
            {
                var logger = context.HttpContext.RequestServices.GetService<ILoggerProvider>().CreateLogger(context.ActionDescriptor.DisplayName);
                logger.LogError(context.Exception, string.Empty);
                context.Result = environment.IsDevelopment() ? new ObjectResult(new { code = -1, msg = "请求发生错误,请稍后再试.", ex = context.Exception }) : new ObjectResult(new { code = -1, msg = "请求发生错误,请稍后再试." });
            }
        }
    }
}
