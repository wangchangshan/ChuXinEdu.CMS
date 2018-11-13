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

            string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQAB";
            string privateKey = "MIIEpAIBAAKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQABAoIBAQCThWCuLTmWKp190BR3jtKZqq0eQXNS+jmNfxKOoTpoiauDUdrsUDlduh7i1x8zinrTDYWVRSPYUk3fpyI0rTkdPFudRrQdB5dUUiRB9bifs9XpaqIXfdXYI+VGbDoYye2g/QctBocuH+ZTnuh1KT08KSMjO1hEMg52xcCTH+f7HXC4n6O8nhcle8OrRDcpBTqvq4fTecDtbx9ocOEyUNJl4UtUYoLsweM/t2EPwkEZtL+VGvGqLcXxklN0bvF3GRflBXn3gHkVvSO6Q9Nfi4yaQDQkOTzRsmB2naoLh12H3oHZbIGORoZVNYkZtl05liViqEcjhd9oXlxDJ/I+SsFBAoGBAOyk9geODIsJKg8Fn05L/NW8L9+HT/1Yi7/9f1ZbxYF/GPjoUkm/ubdAhzuHLdnqQNjLoRHB6eA6MMgE7GKerITuw4n9guFyygBKxGskmq1Uj0b2BRE4V5j+v+Yi/ZcenuFwW2IA2HQ1bV23GDCGQCSbPyjwsimucgFXJOpBaazJAoGBAN+87Ti56nTsmWzobP1c3uF9/MJRZh7obEmsUCJmeLu/ARNwHnbbELlE2BHmYPWn4zR0yHMQ4dyXEhaSfwq+rwO3CQ7PPKTaUGgaOFmwlb7iP0QNzGHxZcgSEgOhEhEVyZeuXJ0xovXyKytzFjlg10sGUOFxrU09KLtIfJNoniQFAoGADdReJHbiuMViVdpoQvUh44PJ8HBB9nZURHYWGP/n/PdmiDVbib/QpKr6vDncLh4IiCLuWNFST65W53mSLvlC78ncV5YfLfm9YGL+M5zCBVspvGXoSedXBzPsFxD4bPp3IomkbYfLHo94wr9OhJdz7C++czAN2W2+b+Gd4KrN+DECgYBTYM2qsSYdkrqOaoOLW9u3fsL+C2WaKRWJ0ww99aASn1igTM0dj//Ie05mRUtm4Lz8JHU65FS3ZxkXjlqHv43rPX/DpBk+ehky4mqTigoUC652BwpeiZ5bk9hgv9rCwTLSMulSr1fCfg/2bjofAebZj63+heWGfj86pAXAzWD5nQKBgQCnTuHTyhUnfsLFUbxNJlHNSLvOzP1WURCAUYbXVWlfnD/2HIEBTvHHLnBxA4libQJnDLnzcLFlfUxZxa5K+fSfv5m/4ED1tbOwebACiWMUTI2sE26TfsGltJkAsp4FJu7gCznJP24rmgDDHE5RDCgU5zrhYa1dd04Z4Ikcd/0vMw==";
            var rsa = new RSAHelper(RSAType.RSA2,Encoding.UTF8, privateKey, publicKey);

            //_logger.LogInformation("解密前为：{0}", pwd);
            pwd = rsa.Decrypt(pwd);
            //_logger.LogInformation("解密后为：{0}", pwd);
            result = _chuxinWorkFlow.LoginVerify(loginCode, pwd);

            // 当前用户名签名
            string signToken = "";
            if(result == "200")
            {
                signToken = rsa.Sign(loginCode);
            }

            dynamic obj = new {
                result = result,
                token = signToken
            };

            string reslutJson = JsonConvert.SerializeObject(obj);            
            return reslutJson;
        }

    }   
}