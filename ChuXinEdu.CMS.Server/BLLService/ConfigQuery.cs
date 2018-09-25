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
    public class ConfigQuery : IConfigQuery
    {
        public IEnumerable<DIC_R_KEY_VALUE> GetDicByCode(string typeCode)
        {
            using (BaseContext context = new BaseContext())
			{
				var dicList =  context.DIC_R_KEY_VALUE.FromSql($@"select id, item_code, item_name 
                                                                  from sys_dictionary 
                                                                  where type_code= {typeCode} and item_enabled = 'Y' 
                                                                  order by item_sort_weight")
                                                        .ToList();
                return dicList;
			}
        }
    }
}