using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.BLLService
{
    public class DicQuery : IDicQuery
    {
        public IEnumerable<SysDictionary> GetDic(string typeCode)
        {
            using (BaseContext context = new BaseContext())
			{
				return context.SysDictionary.ToList();
			}
        }
    }
}