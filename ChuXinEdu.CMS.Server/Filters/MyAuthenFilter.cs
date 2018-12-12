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
    public class MyAuthenFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            string token = filterContext.HttpContext.Request.Headers["token"] + "";
            string loginCode = filterContext.HttpContext.Request.Headers["logincode"] + "";
            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginCode))
            {
                filterContext.Result = new JsonResult(new { code = "1401" });
                return;
            }
            else
            {
                string ip = filterContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); 
                string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQAB";
                string privateKey = "MIIEpAIBAAKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQABAoIBAQCThWCuLTmWKp190BR3jtKZqq0eQXNS+jmNfxKOoTpoiauDUdrsUDlduh7i1x8zinrTDYWVRSPYUk3fpyI0rTkdPFudRrQdB5dUUiRB9bifs9XpaqIXfdXYI+VGbDoYye2g/QctBocuH+ZTnuh1KT08KSMjO1hEMg52xcCTH+f7HXC4n6O8nhcle8OrRDcpBTqvq4fTecDtbx9ocOEyUNJl4UtUYoLsweM/t2EPwkEZtL+VGvGqLcXxklN0bvF3GRflBXn3gHkVvSO6Q9Nfi4yaQDQkOTzRsmB2naoLh12H3oHZbIGORoZVNYkZtl05liViqEcjhd9oXlxDJ/I+SsFBAoGBAOyk9geODIsJKg8Fn05L/NW8L9+HT/1Yi7/9f1ZbxYF/GPjoUkm/ubdAhzuHLdnqQNjLoRHB6eA6MMgE7GKerITuw4n9guFyygBKxGskmq1Uj0b2BRE4V5j+v+Yi/ZcenuFwW2IA2HQ1bV23GDCGQCSbPyjwsimucgFXJOpBaazJAoGBAN+87Ti56nTsmWzobP1c3uF9/MJRZh7obEmsUCJmeLu/ARNwHnbbELlE2BHmYPWn4zR0yHMQ4dyXEhaSfwq+rwO3CQ7PPKTaUGgaOFmwlb7iP0QNzGHxZcgSEgOhEhEVyZeuXJ0xovXyKytzFjlg10sGUOFxrU09KLtIfJNoniQFAoGADdReJHbiuMViVdpoQvUh44PJ8HBB9nZURHYWGP/n/PdmiDVbib/QpKr6vDncLh4IiCLuWNFST65W53mSLvlC78ncV5YfLfm9YGL+M5zCBVspvGXoSedXBzPsFxD4bPp3IomkbYfLHo94wr9OhJdz7C++czAN2W2+b+Gd4KrN+DECgYBTYM2qsSYdkrqOaoOLW9u3fsL+C2WaKRWJ0ww99aASn1igTM0dj//Ie05mRUtm4Lz8JHU65FS3ZxkXjlqHv43rPX/DpBk+ehky4mqTigoUC652BwpeiZ5bk9hgv9rCwTLSMulSr1fCfg/2bjofAebZj63+heWGfj86pAXAzWD5nQKBgQCnTuHTyhUnfsLFUbxNJlHNSLvOzP1WURCAUYbXVWlfnD/2HIEBTvHHLnBxA4libQJnDLnzcLFlfUxZxa5K+fSfv5m/4ED1tbOwebACiWMUTI2sE26TfsGltJkAsp4FJu7gCznJP24rmgDDHE5RDCgU5zrhYa1dd04Z4Ikcd/0vMw==";
                var rsa = new RSAHelper(RSAType.RSA2,Encoding.UTF8, privateKey, publicKey);
                bool signVerify = rsa.Verify(loginCode + ip,token);
                if(!signVerify)
                {
                    filterContext.Result = new JsonResult(new { code = "1401" });
                    return;
                }

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
                        filterContext.Result = new JsonResult(new { code = "1401" });
                    }
                }
            }    
        }
    }
}