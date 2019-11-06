using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class WxPictureController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public WxPictureController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取微信小程序用到的宣传图片 GET api/wxpicture/getwxpicture
        /// </summary>
        /// <returns></returns>
        [HttpGet("{wxPicCode}")]
        public IEnumerable<WxPicture> GetWxPicture(string wxPicCode)
        {
            IEnumerable<WxPicture> wxPics = _chuxinQuery.GetWxPicture(wxPicCode);

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var pic in wxPics)
            {
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=ad-wx";
            }

            return wxPics;
        }
    }
}