using System;
using System.Data;
using System.Linq;
using System.Text;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 身份验证过滤器
    public class OuterAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext fileterContext)
        {
            string name = fileterContext.HttpContext.Request.Headers["name"] + "";
            
            using (BaseContext context = new BaseContext())
                {
                    var outerUser = context.SysOutUser.Where(u => u.UserCode == name).FirstOrDefault();
                    if(outerUser != null)
                    {  
                        
                        outerUser.LastRequestTime = DateTime.Now;
                        context.SaveChanges();
                    }
                    else 
                    {
                        fileterContext.Result = new JsonResult(new { code = "1401" });
                    }
                }   
        }
    }
}