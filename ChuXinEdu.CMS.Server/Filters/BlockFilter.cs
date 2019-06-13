using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 全部拦截过滤器 用于过时的接口
    public class BlockFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new JsonResult(new { code = "0000" });
            return;
        }
    }
}