using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net.Http;
using System.Text;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;
        private readonly ILogger<TeacherController> _logger;

        private readonly string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQAB";
        private readonly string privateKey = "MIIEpAIBAAKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQABAoIBAQCThWCuLTmWKp190BR3jtKZqq0eQXNS+jmNfxKOoTpoiauDUdrsUDlduh7i1x8zinrTDYWVRSPYUk3fpyI0rTkdPFudRrQdB5dUUiRB9bifs9XpaqIXfdXYI+VGbDoYye2g/QctBocuH+ZTnuh1KT08KSMjO1hEMg52xcCTH+f7HXC4n6O8nhcle8OrRDcpBTqvq4fTecDtbx9ocOEyUNJl4UtUYoLsweM/t2EPwkEZtL+VGvGqLcXxklN0bvF3GRflBXn3gHkVvSO6Q9Nfi4yaQDQkOTzRsmB2naoLh12H3oHZbIGORoZVNYkZtl05liViqEcjhd9oXlxDJ/I+SsFBAoGBAOyk9geODIsJKg8Fn05L/NW8L9+HT/1Yi7/9f1ZbxYF/GPjoUkm/ubdAhzuHLdnqQNjLoRHB6eA6MMgE7GKerITuw4n9guFyygBKxGskmq1Uj0b2BRE4V5j+v+Yi/ZcenuFwW2IA2HQ1bV23GDCGQCSbPyjwsimucgFXJOpBaazJAoGBAN+87Ti56nTsmWzobP1c3uF9/MJRZh7obEmsUCJmeLu/ARNwHnbbELlE2BHmYPWn4zR0yHMQ4dyXEhaSfwq+rwO3CQ7PPKTaUGgaOFmwlb7iP0QNzGHxZcgSEgOhEhEVyZeuXJ0xovXyKytzFjlg10sGUOFxrU09KLtIfJNoniQFAoGADdReJHbiuMViVdpoQvUh44PJ8HBB9nZURHYWGP/n/PdmiDVbib/QpKr6vDncLh4IiCLuWNFST65W53mSLvlC78ncV5YfLfm9YGL+M5zCBVspvGXoSedXBzPsFxD4bPp3IomkbYfLHo94wr9OhJdz7C++czAN2W2+b+Gd4KrN+DECgYBTYM2qsSYdkrqOaoOLW9u3fsL+C2WaKRWJ0ww99aASn1igTM0dj//Ie05mRUtm4Lz8JHU65FS3ZxkXjlqHv43rPX/DpBk+ehky4mqTigoUC652BwpeiZ5bk9hgv9rCwTLSMulSr1fCfg/2bjofAebZj63+heWGfj86pAXAzWD5nQKBgQCnTuHTyhUnfsLFUbxNJlHNSLvOzP1WURCAUYbXVWlfnD/2HIEBTvHHLnBxA4libQJnDLnzcLFlfUxZxa5K+fSfv5m/4ED1tbOwebACiWMUTI2sE26TfsGltJkAsp4FJu7gCznJP24rmgDDHE5RDCgU5zrhYa1dd04Z4Ikcd/0vMw==";

        public AccountController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, ILogger<TeacherController> logger)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _logger = logger;
        }


        // POST api/account/login
        [HttpPost]
        public ActionResult<string> Login([FromBody] dynamic loginForm)
        {
            string result = string.Empty;
            string loginCode = loginForm.username.ToString();
            string pwd = loginForm.password.ToString();
            
            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); 
            var rsa = new RSAHelper(RSAType.RSA2,Encoding.UTF8, privateKey, publicKey);

            try
            {
                pwd = rsa.Decrypt(pwd);
            }
            catch(Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致登陆错误，访问来自IP：{0}", ip);
            }
            if(result == "1700")
            {
                return new JsonResult(new { code = result });
            }
            
            string signToken = ""; 
            string teacherCode = "";
            result = _chuxinWorkFlow.LoginVerify(loginCode, pwd, ip, out signToken, out teacherCode);

            string roles = string.Empty;
            // 当前用户名签名
            if(result == "1200")
            {
                if(teacherCode == "0")
                {
                    roles = "1007";
                }
                else 
                {
                    roles = _chuxinQuery.GetRoles(teacherCode);
                }
                signToken = Guid.NewGuid().ToString("N");
                result = _chuxinWorkFlow.SaveUserLoginInfo(loginCode, ip, signToken);
            }
            
            return new JsonResult(new { code = result, data = signToken, roles = roles });
        }

        // POST api/account/checkpwd
        [HttpPost]
        public ActionResult<string> CheckPwd([FromBody] dynamic loginForm)
        {
            string result = string.Empty;
            string loginCode = loginForm.username.ToString();
            string pwd = loginForm.password.ToString();
            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); 

            var rsa = new RSAHelper(RSAType.RSA2,Encoding.UTF8, privateKey, publicKey);

            try
            {
                pwd = rsa.Decrypt(pwd);
            }
            catch(Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致登陆错误，访问来自IP：{0}", ip);
            }
            if(result == "1700")
            {
                return new JsonResult(new { code = result });
            }
            
            result = _chuxinWorkFlow.PwdVerify(loginCode, pwd);
            return new JsonResult(new { code = result });
        }

          // POST api/account/login
        [HttpPost]
        public ActionResult<string> ChangePwd([FromBody] dynamic pwdForm)
        {
            string result = string.Empty;
            string loginCode = pwdForm.username.ToString();
            string newPwd = pwdForm.newpwd.ToString();

            var rsa = new RSAHelper(RSAType.RSA2,Encoding.UTF8, privateKey, publicKey);

            try
            {
                newPwd = rsa.Decrypt(newPwd);
            }
            catch(Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致修改密码错误，访问来自IP：{0}", HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            }
            if(result == "1700")
            {
                return new JsonResult(new { code = result });
            }
             
            result = _chuxinWorkFlow.ChangePassword(loginCode, newPwd);            
            return new JsonResult(new { code = result });
        }
        
        // POST api/account/logout
        [HttpPost("{logincode}")]
        [MyAuthenFilter]
        public ActionResult<string> LogOut(string loginCode)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.LogOut(loginCode);
            
            return new JsonResult(new { code = result});
        }
    }   
}