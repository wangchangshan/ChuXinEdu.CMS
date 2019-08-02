using System;
using System.Data;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.BLL;
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                                                where course_type='试听' and attendance_status_code = '01' group by DATE_FORMAT(course_date,'%Y-%m'), course_category_code 
                                                order by DATE_FORMAT(course_date,'%Y-%m')");

            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取签到课程数据出错");
            }
            return dt;
        }

        public DataTable GetTotalIncome()
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select course_category_name as name,sum(actual_price-fee_back_amount) as value 
                                                from student_course_package 
                                                where is_payed='Y' 
                                                group by course_category_name");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取整体报名收入分布出错");
            }
            return dt;
        }

        public IDictionary<string, decimal> GetTotalActualIncome()
        {
            Dictionary<string, decimal> actualIncome = new Dictionary<string, decimal>();
            try
            {
                actualIncome.Add("国画", 0.00m);
                actualIncome.Add("西画", 0.00m);
                actualIncome.Add("硬笔", 0.00m);
                actualIncome.Add("软笔", 0.00m);

                DataTable dtPackage = ADOContext.GetDataTable(@"select id, actual_course_count,actual_price 
                                                from student_course_package 
                                                where is_payed='Y'");
                foreach (DataRow dr in dtPackage.Rows)
                {
                    string packageId = dr["id"].ToString();
                    decimal amount = Convert.ToDecimal(dr["actual_price"].ToString());
                    int courseCount = Convert.ToInt32(dr["actual_course_count"].ToString());
                    decimal unitPrice = amount / courseCount;

                    DataTable dtCourse = ADOContext.GetDataTable(@"select course_folder_name, count(1) as course_count 
                                                                    from student_course_list 
                                                                    where student_course_package_id =@1 and attendance_status_code in ('01', '02') 
                                                                    group by course_folder_name", packageId);
                    foreach (DataRow drCourse in dtCourse.Rows)
                    {
                        actualIncome[drCourse["course_folder_name"].ToString()] += Convert.ToInt32(drCourse["course_count"].ToString()) * unitPrice;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取实际销课收入分布出错");
            }
            return actualIncome;
        }

        public DataTable GetAllDistribution(string begin, string end)
        {
            DataTable dt = null;
            try
            {
                dt = ADOContext.GetDataTable(@"select course_folder_code, count(1) as course_count, DATE_FORMAT(course_date,'%Y-%m') as ym 
                                                from student_course_list 
                                                where course_type='正式' and DATE_FORMAT(course_date,'%Y-%m') >= @1 and DATE_FORMAT(course_date,'%Y-%m') <= @2
                                                    and attendance_status_code in ('01', '02') 
                                                group by course_folder_code,DATE_FORMAT(course_date,'%Y-%m')"
                                            , begin, end);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取销课课时分布数据出错");
            }
            return dt;
        }
    }
}