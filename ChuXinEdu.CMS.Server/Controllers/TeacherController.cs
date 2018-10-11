using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;

        public TeacherController(IChuXinQuery chuxinQuery)
        {
            _chuxinQuery = chuxinQuery;    
        }

        /// <summary>
        /// [教师] 获取教师键值对 list GET api/teacher/getfinancer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<DIC_R_KEY_VALUE> GetFinancer()
        {
            return _chuxinQuery.GetTeacherToCharge();
        }
    }   
}