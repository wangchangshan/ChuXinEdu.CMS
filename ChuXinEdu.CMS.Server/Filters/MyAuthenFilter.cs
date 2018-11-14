using System;
using System.Data;
using System.Linq;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChuXinEdu.CMS.Server.Filters
{
    // 身份验证过滤器
    public class MyAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext fileterContext)
        {
            string token = fileterContext.HttpContext.Request.Headers["token"] + "";
            string loginCode = fileterContext.HttpContext.Request.Headers["logincode"] + "";
            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginCode))
            {
                fileterContext.Result = new JsonResult(new { code = "1401" });
            }
            else
            {
                //为了防止 复制请求访问， 也可以根据Ip和logincode来生成 token, 在此处重新更加请求的IP和logincode 验证token签名
                //string ip = fileterContext.HttpContext.Connection.RemoteIpAddress.ToString(); // 反向代理的时候不能这么处理
                using (BaseContext context = new BaseContext())
                {
                    var sysUser = context.SysUser.Where(u => u.LoginCode == loginCode
                                                            && u.Token == token
                                                            && u.TokenExpireTime > DateTime.Now)
                                                .FirstOrDefault();
                    if(sysUser != null)
                    {  
                        string strExpireMinu = CustomConfig.GetSetting("UserExpireTime");
                        if (string.IsNullOrEmpty(strExpireMinu))
                        {
                            strExpireMinu = "60";
                        }
                        int expireMinu = Int32.Parse(strExpireMinu);
                        sysUser.TokenExpireTime = DateTime.Now.AddMinutes(expireMinu);
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
}