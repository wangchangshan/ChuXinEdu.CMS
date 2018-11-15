using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 格式化返回值
    public class MyReturnValueFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // if (context.Result is ObjectResult)
            // {
            //     var objectResult = context.Result as ObjectResult;
            //     if(objectResult.Value is String)
            //     {
            //         context.Result = new JsonResult(new { code = objectResult.Value.ToString(), data = "" });
            //     }
            //     else if(objectResult.Value is JsonResult)
            //     {
            //         var jsonResult = objectResult.Value as JsonResult;
            //         context.Result = new JsonResult(new { code = "1200", data = jsonResult.Value });
            //     }
            // }
            // else if (context.Result is EmptyResult)
            // {
            //     //context.Result = new ObjectResult(new { code = 404, sub_msg = "未找到资源", msg = "" });
            // }
            // else if (context.Result is ContentResult)
            // {
            //     //context.Result = new ObjectResult(new { code = 200, msg = "", result = (context.Result as ContentResult).Content });
            // }
            // else if (context.Result is StatusCodeResult)
            // {
            //     // context.Result = new ObjectResult(new { code = (context.Result as StatusCodeResult).StatusCode, sub_msg = "", msg = "" });
            // }
        }
    }
}