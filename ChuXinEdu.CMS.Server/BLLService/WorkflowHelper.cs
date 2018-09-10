using System;
using System.Data;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.BLLService
{
    public class WorkflowHelper
    {
        // 判断当前日期是否可以排课，如果是统一放假则返回false
        public static bool IsAvailableCourseDate(List<SysHoliday> holidays, DateTime theDay)
        {
            bool result = true;
            foreach (var item in holidays)
            {
                if(item.HolidayDate.ToShortDateString() == theDay.ToShortDateString())
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}