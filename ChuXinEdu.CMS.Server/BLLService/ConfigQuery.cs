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
				var dicList =  context.DIC_R_KEY_VALUE.FromSql($@"select item_code, item_name 
                                                                  from sys_dictionary 
                                                                  where type_code= {typeCode} and item_enabled = 'Y' 
                                                                  order by item_sort_weight")
                                                        .ToList();
                return dicList;
			}
        }

        public IEnumerable<DIC_R_KEY_VALUE> GetSysCoursePackage(string categoryCode)
        {
            using (BaseContext context = new BaseContext())
			{
				var dicList =  context.DIC_R_KEY_VALUE.FromSql($@"select package_code as item_code, package_name as item_name 
                                                                  from sys_course_package  
                                                                  where package_course_category_code = {categoryCode} and package_enabled = '是' ")
                                                        .ToList();
                return dicList;
			}
        }

        public IEnumerable<SysDictionary> GetDictionarys()
        {
            using (BaseContext context = new BaseContext())
            {
                var dics = context.SysDictionary.ToList();
                return dics;
            }
        }

        public IEnumerable<StudentCourseArrange> GetArrangeDirty()
        {
            using (BaseContext context = new BaseContext())
            {
                var arrange = context.StudentCourseArrange.Where(a => a.CourseRestCount <= 0).ToList();
                return arrange;
            }
        }

        public IEnumerable<StudentCoursePackage> GetPackageDirty()
        {
            using (BaseContext context = new BaseContext())
            {
                var packages = context.StudentCoursePackage.Where(p => p.RestCourseCount == 0 && p.ScpStatus == "00").ToList();
                return packages;
            }
        }
    }
}