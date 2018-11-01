using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Context;
using Microsoft.Extensions.Logging;

namespace ChuXinEdu.CMS.Server.BLLService
{
    public class ChuXinStatistics : IChuXinStatistics
    {
        private readonly ILogger<ChuXinStatistics> _logger;
        public ChuXinStatistics(ILogger<ChuXinStatistics> logger)
        {
            _logger = logger;
        }
        public DataTable GetCourseDistribution()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(course_date,'%Y-%m') as ym, course_category_code as type, count(1) as amount 
                                                from student_course_list 
                                                where attendance_status_code = '01' and course_type='正式' group by DATE_FORMAT(course_date,'%Y-%m'), course_category_code 
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取签到课程数据出错");
            }
            return dt;
        }

        public DataTable GetStudentDistribution()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(course_date,'%Y-%m') as ym, count(distinct student_code) as amount 
                                                from student_course_list 
                                                where attendance_status_code = '01' and course_type='正式' group by DATE_FORMAT(course_date,'%Y-%m')
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取每月学生数目出错");
            }
            return dt;
        }

        public DataTable GetStudentDistribution_meishu()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(course_date,'%Y-%m') as ym, count(distinct student_code) as amount 
                                                from student_course_list 
                                                where course_category_code='meishu' and attendance_status_code = '01' and course_type='正式' group by DATE_FORMAT(course_date,'%Y-%m')
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取每月美术学生数据出错");
            }
            return dt;
        }

        public DataTable GetStudentDistribution_shufa()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(course_date,'%Y-%m') as ym, count(distinct student_code) as amount 
                                                from student_course_list 
                                                where course_category_code='shufa' and attendance_status_code = '01' and course_type='正式' group by DATE_FORMAT(course_date,'%Y-%m')
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取每月书法学生数据出错");
            }
            return dt;
        }

        public DataTable GetTrialStudentDistribution()
        {
            //如果根据student_temp表，则会出现 本月没有约试听 则下个月也没有统计到的情况
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(course_date,'%Y-%m') as ym,course_category_code as type,count(distinct student_code) as amount 
                                                from student_course_list 
                                                where course_type='正式' and attendance_status_code = '01' group by DATE_FORMAT(course_date,'%Y-%m'), course_category_code 
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取每月试听学生数据出错");
            }
            return dt;
        }
        public DataTable GetIncomeDistribution()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select DATE_FORMAT(pay_date,'%Y-%m')as ym,course_category_code as type,sum(actual_price) as amount 
                                                from student_course_package 
                                                group by DATE_FORMAT(pay_date,'%Y-%m'), course_category_code
                                                order by DATE_FORMAT(pay_date,'%Y-%m')");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "获取签到课程数据出错");
            }
            return dt;
        }
    }
}