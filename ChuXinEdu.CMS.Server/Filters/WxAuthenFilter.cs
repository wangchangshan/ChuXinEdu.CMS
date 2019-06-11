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
    public class WxAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext fileterContext)
        {
            string sKey = fileterContext.HttpContext.Request.Headers["skey"] + "";
            
            using (BaseContext context = new BaseContext())
                {
                    var wxUser = context.SysWxUser.Where(u => u.SessionKey == sKey).FirstOrDefault();
                    if(wxUser != null)
                    {  
                        
                        wxUser.LastRequestTime = DateTime.Now;
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