using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<SysDictionary>> GetAll()
        {
            //  /api/dic            
            SysDictionary[] dic = null;
             using (var context = new BaseContext())
             {
                dic =  context.SysDictionary.ToList().ToArray();
             }
            return dic;
        }
    }   
}