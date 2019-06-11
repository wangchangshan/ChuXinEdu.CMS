using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class WxOpenController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public WxOpenController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取微信小程序用到的宣传图片 GET api/wxopen/getwxhomepicture
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<WxPicture> GetWxHomePicture()
        {
            IEnumerable<WxPicture> wxPics = _chuxinQuery.GetWxHomePicture();

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var pic in wxPics)
            {
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=wx";
            }

            return wxPics;
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
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=wx";
            }

            return wxPics;
        }
    }
}