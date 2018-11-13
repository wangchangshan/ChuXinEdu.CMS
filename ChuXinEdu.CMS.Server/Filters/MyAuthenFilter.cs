using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    public class MyAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["token"] + "";   
            if(string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new { code = "401" });
            }     
        }
    }
}