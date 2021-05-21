using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNow.AspNetCore.ActionResults
{
    /// <summary>
    /// 自定义内容的ActionResult,用于跳过全局设置的{code,msg,data格式}
    /// </summary>
    public class CustomContentResult : IActionResult
    {
        public string Content { get; set; }

        public string ContentType { get; set; }

        public int? StatusCode { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ContentResult>>().ExecuteAsync(context, new ContentResult
            {
                Content = Content,
                ContentType = ContentType,
                StatusCode = StatusCode
            });
        }
    }
}
