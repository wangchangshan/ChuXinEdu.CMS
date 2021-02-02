using System;
using System.Linq;
using ChuXinEdu.CMS.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 身份验证过滤器
    public class WxAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext fileterContext)
        {
            string sKey = fileterContext.HttpContext.Request.Headers["skey"] + "";

            using (BaseContext context = new BaseContext())
            {
                var wxUser = context.SysWxUser.Where(u => u.SessionKey == sKey).FirstOrDefault();
                if (wxUser != null)
                {

                    wxUser.LastRequestTime = DateTime.Now;
                    context.SaveChanges();
                    if (wxUser.InnerPersonName == "马朝")
                    {
                        System.Threading.Thread.Sleep(2500);
                    }
                }
                else
                {
                    fileterContext.Result = new JsonResult(new { code = "1401" });
                }
            }
        }
    }
}