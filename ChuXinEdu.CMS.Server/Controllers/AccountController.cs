using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.BLL;
using Microsoft.Extensions.Logging;
using System.Text;
using ChuXinEdu.CMS.Server.Filters;
using ChuXinEdu.CMS.Server.Utils;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;
        private readonly ILogger<AccountController> _logger;

        private readonly string publicKey = CustomConfig.GetSetting("LoginPublicKey");
        private readonly string privateKey = CustomConfig.GetSetting("LoginPrivateKey");

        public AccountController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, ILogger<AccountController> logger)
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
            string loginCode = loginForm.username.ToString().ToUpper();
            string pwd = loginForm.password.ToString();

            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);

            try
            {
                pwd = rsa.Decrypt(pwd);
            }
            catch (Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致登陆错误，访问来自IP：{0}", ip);
            }
            if (result == "1700")
            {
                return new JsonResult(new { code = result });
            }

            string signToken = "";
            string teacherCode = "";
            result = _chuxinWorkFlow.LoginVerify(loginCode, pwd, ip, out signToken, out teacherCode);

            string roles = string.Empty;
            // 当前用户名签名
            if (result == "1200")
            {
                if (teacherCode == "0")
                {
                    roles = "1007";
                    if (loginCode == "CSWANG")
                    {
                        roles = "1007,99999"; // super
                    }
                }
                else
                {
                    roles = _chuxinQuery.GetRoles(teacherCode);
                }
                signToken = Guid.NewGuid().ToString("N");
                result = _chuxinWorkFlow.SaveUserLoginInfo(loginCode, ip, signToken);
            }
            else if (result == "1201") // 同一局域网内的登陆
            {
                if (teacherCode == "0")
                {
                    roles = "1007";
                }
                else
                {
                    roles = _chuxinQuery.GetRoles(teacherCode);
                }
            }

            return new JsonResult(new { code = result, data = signToken, roles = roles });
        }

        // POST api/account/checkpwd
        [HttpPost]
        [MyAuthenFilter]
        public ActionResult<string> CheckPwd([FromBody] dynamic loginForm)
        {
            string result = string.Empty;
            string loginCode = loginForm.username.ToString();
            string pwd = loginForm.password.ToString();
            string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);

            try
            {
                pwd = rsa.Decrypt(pwd);
            }
            catch (Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致登陆错误，访问来自IP：{0}", ip);
            }
            if (result == "1700")
            {
                return new JsonResult(new { code = result });
            }

            result = _chuxinWorkFlow.PwdVerify(loginCode, pwd);
            return new JsonResult(new { code = result });
        }

        // POST api/account/changepwd
        [HttpPost]
        [MyAuthenFilter]
        public ActionResult<string> ChangePwd([FromBody] dynamic pwdForm)
        {
            string result = string.Empty;
            string loginCode = pwdForm.username.ToString();
            string newPwd = pwdForm.newpwd.ToString();

            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);

            try
            {
                newPwd = rsa.Decrypt(newPwd);
            }
            catch (Exception ex)
            {
                result = "1700";
                _logger.LogError(ex, "公钥秘钥不匹配导致修改密码错误，访问来自IP：{0}", HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            }
            if (result == "1700")
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

            return new JsonResult(new { code = result });
        }
    }
}