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
    public class ChuXinQuery : IChuXinQuery
    {
        #region config

        public string GetRoles(string teacherCode)
        {
            string strRoles = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var roles = context.TeacherRole.Where(r => r.TeacherCode == teacherCode).ToList();
                foreach (TeacherRole role in roles)
                {
                    strRoles += role.RoleCode + ",";
                }
            }
            if (strRoles.Length > 1)
            {
                strRoles = strRoles.TrimEnd(',');
            }
            return strRoles;
        }

        public IEnumerable<SysDictionary> GetAllRoles()
        {
            using (BaseContext context = new BaseContext())
            {
                var roles = context.SysDictionary.Where(d => (d.TypeCode == "sys_roles" || d.TypeCode == "biz_roles")
                                                            && d.ItemEnabled == "Y")
                                                .OrderBy(d => d.ItemSortWeight)
                                                .OrderBy(d => d.TypeCode)
                                                .ToList();

                return roles;
            }
        }

        public IEnumerable<SysMenu> GetSysMenus()
        {
            using (BaseContext context = new BaseContext())
            {
                var menu = context.SysMenu.Where(m => m.IsEnable == "Y")
                                                .OrderBy(m => m.SortWeight)
                                                .ToList();

                return menu;
            }
        }

        public IEnumerable<SysCourseArrangeTemplate> GetSysArrangeTemplates()
        {
            using (BaseContext context = new BaseContext())
            {
                var templates = context.SysCourseArrangeTemplate.ToList();

                return templates;
            }
        }

        public IEnumerable<SysCourseArrangeTemplateDetail> GetArrangeTemplateDetail(string templateCode)
        {
            using (BaseContext context = new BaseContext())
            {
                var templates = context.SysCourseArrangeTemplateDetail.Where(t => t.ArrangeTemplateCode == templateCode)
                                                                    .OrderBy(t => t.CoursePeriod)
                                                                    .OrderBy(t => t.CourseWeekDay)
                                                                    .ToList();

                return templates;
            }
        }
        #endregion


        #region student
        public IEnumerable<Student> GetStudentList(int pageIndex, int pageSize, QUERY_STUDENT query, out int totalCount)
        {
            IEnumerable<Student> students = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<Student> temp = null;

                if (!String.IsNullOrEmpty(query.studentStatus))
                {
                    temp = context.Student.Where(s => s.StudentStatus == query.studentStatus);
                }
                if (!String.IsNullOrEmpty(query.studentCode))
                {
                    if (temp == null)
                    {
                        temp = context.Student;
                    }
                    temp = temp.Where(s => EF.Functions.Like(s.StudentCode, "%" + query.studentCode + "%"));
                }
                if (!String.IsNullOrEmpty(query.studentName.Trim()))
                {
                    if (temp == null)
                    {
                        temp = context.Student;
                    }
                    temp = temp.Where(s => EF.Functions.Like(s.StudentName, "%" + query.studentName.Trim() + "%"));
                }

                if (temp == null)
                {
                    temp = context.Student.Where(s => 1 == 1);
                }
                totalCount = temp.Count();
                students = temp.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                return students;
            }
        }

        public DataTable GetStudentList(int pageIndex, int pageSize, WX_QUERY_STUDENT query)
        {
            using (BaseContext context = new BaseContext())
            {
                int s = (pageIndex - 1) * pageSize;
                int e = pageSize;
                string sql = string.Empty;
                DataTable dt = null;
                if (!String.IsNullOrEmpty(query.studentName.Trim()))
                {
                    // for 测试审核
                    // sql = "select id, student_code, student_name, student_phone,student_avatar_path, '' as rest_course_info from student where student_status='01' and student_name='测试学员' and student_name like '%" + query.studentName.Trim() + "%' limit @1, @2";
                    sql = "select id, student_code, student_name, student_phone,student_avatar_path, '' as rest_course_info from student where student_status='01' and student_name like '%" + query.studentName.Trim() + "%' limit @1, @2";
                    dt = ADOContext.GetDataTable(sql, s, e);
                }
                else
                {
                    // sql = "select id, student_code, student_name, student_phone,student_avatar_path, '' as rest_course_info from student where student_status='01' and student_name='测试学员' limit @1, @2";
                    sql = "select id, student_code, student_name, student_phone,student_avatar_path, '' as rest_course_info from student where student_status='01' limit @1, @2";
                    dt = ADOContext.GetDataTable(sql, s, e);
                }

                return dt;
            }
        }

        public int GetActiveStudentCount()
        {
            int rtn = 0;
            DataTable dt = ADOContext.GetDataTable(@"select count(1) from student where student_status = '01'");
            if (dt.Rows.Count > 0)
            {
                rtn = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            return rtn;
        }

        public int getStudentRestCourseCount(string studentCode)
        {
            int courseCount = 0;
            DataTable dt = ADOContext.GetDataTable(@"select sum(rest_course_count) as course_count from student_course_package where scp_status='00' and student_code=@1", studentCode);
            courseCount = Convert.ToInt32(dt.Rows[0][0] ?? "0");

            return courseCount;
        }

        public IEnumerable<DIC_R_KEY_VALUE> GetAllStudents()
        {
            using (BaseContext context = new BaseContext())
            {
                var students = context.DIC_R_KEY_VALUE.FromSql($@"select s.student_code as item_code, s.student_name as item_name
                                                                    from student s
                                                                    order by s.student_name")
                                                        .ToList();

                return students;
            }
        }

        public IEnumerable<DIC_R_KEY_VALUE> GetAllActiveStudents()
        {
            using (BaseContext context = new BaseContext())
            {
                var students = context.DIC_R_KEY_VALUE.FromSql($@"select s.student_code as item_code, s.student_name as item_name
                                                                    from student s
                                                                    where s.student_status = '01'
                                                                    order by s.student_name")
                                                        .ToList();

                return students;
            }
        }

        public IEnumerable<Student> GetStudentList2Export(QUERY_STUDENT query)
        {
            IEnumerable<Student> students = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<Student> temp = null;

                if (!String.IsNullOrEmpty(query.studentStatus))
                {
                    temp = context.Student.Where(s => s.StudentStatus == query.studentStatus);
                }
                if (!String.IsNullOrEmpty(query.studentCode))
                {
                    if (temp == null)
                    {
                        temp = context.Student;
                    }
                    temp = temp.Where(s => EF.Functions.Like(s.StudentCode, "%" + query.studentCode + "%"));
                }
                if (!String.IsNullOrEmpty(query.studentName.Trim()))
                {
                    if (temp == null)
                    {
                        temp = context.Student;
                    }
                    temp = temp.Where(s => EF.Functions.Like(s.StudentName, "%" + query.studentName.Trim() + "%"));
                }

                if (temp == null)
                {
                    temp = context.Student.Where(s => 1 == 1);
                }
                students = temp.ToList();

                return students;
            }
        }

        public IEnumerable<StudentRecommend> GetRecommendStudentList(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentRecommend.Where(s => s.OriginStudentCode == studentCode).ToList();
            }
        }

        public DataTable GetStudentForRecommend(string studentName)
        {
            DataTable dtStudent = ADOContext.GetDataTable($@"select s.student_code, s.student_name,s.student_sex, s.student_register_date 
                                                            from student s
                                                            where s.student_name like '%{studentName}%' and not exists (
		                                                        select * from student_recommend sr where sr.new_student_code = s.student_code
	                                                        )");

            return dtStudent;
        }

        public DataTable GetStudentAuxiliaryInfo(string studentCode)
        {
            DataTable dtStudent = ADOContext.GetDataTable($@"select s.trial_other_course,sr.origin_student_code,sr.origin_student_name from student s 
                                                            left join student_recommend sr on s.student_code = sr.new_student_code
                                                            where s.student_code = '{studentCode}'");

            return dtStudent;
        }

        public IEnumerable<StudentTemp> GetTempStudentList(int pageIndex, int pageSize, QUERY_STUDENT_TEMP query, out int totalCount)
        {
            IEnumerable<StudentTemp> students = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<StudentTemp> temp = null;
                if (!string.IsNullOrEmpty(query.studentName.Trim()))
                {
                    temp = context.StudentTemp.Where(s => EF.Functions.Like(s.StudentName, "%" + query.studentName.Trim() + "%"));
                }
                if (query.studentTempStatus.Count() > 0)
                {
                    if (temp == null)
                    {
                        temp = context.StudentTemp;
                    }
                    temp = temp.Where(s => query.studentTempStatus.Contains(s.StudentTempStatus));
                }
                if (!string.IsNullOrEmpty(query.result.Trim()))
                {
                    if (temp == null)
                    {
                        temp = context.StudentTemp;
                    }
                    temp = temp.Where(s => s.Result == query.result.Trim());
                }

                if (temp == null)
                {
                    temp = context.StudentTemp.Where(s => 1 == 1);
                }

                totalCount = temp.Count();
                students = temp.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                return students;
            }
        }

        // 所有学生的课程大类
        public DataTable GetScpSimplify()
        {
            DataTable dt = ADOContext.GetDataTable(@"select distinct student_code,course_category_code,course_category_name from student_course_package");
            return dt;
        }

        public DataTable GetRestCourseCountByCategorty(string studentCode)
        {
            DataTable dt = ADOContext.GetDataTable($@"select course_category_name, sum(rest_course_count) as rest_course_count from student_course_package 
                                                        where student_code ='{studentCode}'  and scp_status = '00'
                                                        group by course_category_name");

            return dt;
        }

        public DataTable GetStudentPayRank()
        {
            DataTable dt = ADOContext.GetDataTable(@"select student_name, sum(actual_price - fee_back_amount) as amount from student_course_package group by student_name order by sum(actual_price - fee_back_amount) desc limit 0, 100");
            return dt;
        }

        public Student GetStudentByCode(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Student.Where(s => s.StudentCode == studentCode).First();
            }
        }

        public IEnumerable<StudentCoursePackage> GetStudentCoursePackage(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCoursePackage.Where(s => s.StudentCode == studentCode).OrderByDescending(s => s.Id).ToList();
            }
        }

        public IEnumerable<DIC_R_KEY_VALUE> GetStudentPackageKV(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                var dicList = context.DIC_R_KEY_VALUE.FromSql($@"select distinct course_category_code as item_code, course_category_name as item_name 
                                                                from student_course_package 
                                                                where student_code={studentCode}")
                                                        .ToList();
                return dicList;
            }
        }

        public IEnumerable<StudentCoursePackage> GetNoFinishPackage(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                && s.ScpStatus == "00"
                                                                && s.IsPayed == "Y")
                                                    .ToList();
            }
        }

        public List<StudentCourseList> GetCoursesByPackage(int scpId, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(s => s.StudentCoursePackageId == scpId
                                                            && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"))
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderBy(s => s.CourseDate)
                                                .Skip(pageSize * (pageIndex - 1))
                                                .Take(pageSize).ToList();
            }
        }

        public IEnumerable<StudentCourseList> GetStudentCourseList(string studentCode, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(s => s.StudentCode == studentCode
                                                            && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"))
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderByDescending(s => s.CourseDate).ToList()
                                                .Skip(pageSize * (pageIndex - 1))
                                                .Take(pageSize);
            }
        }

        public IEnumerable<StudentCourseList> GetStudentCourseList(int pageIndex, int pageSize, QUERY_STUDENT_COURSE_LIST query, out int totalCount)
        {
            IEnumerable<StudentCourseList> courseList = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<StudentCourseList> temp = null;

                if (!String.IsNullOrEmpty(query.studentCode))
                {
                    temp = context.StudentCourseList.Where(s => s.StudentCode == query.studentCode
                                                                && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"));
                }
                if (query.studentPackageId != null)
                {
                    if (temp == null)
                    {
                        temp = context.StudentCourseList;
                    }
                    temp = temp.Where(s => s.StudentCoursePackageId == query.studentPackageId);
                }

                if (temp == null)
                {
                    totalCount = 0;
                    courseList = temp;
                }
                else
                {
                    totalCount = temp.Count();
                    courseList = temp.OrderBy(s => s.CoursePeriod)
                                    .OrderBy(s => s.CourseDate).ToList()
                                    .Skip(pageSize * (pageIndex - 1))
                                    .Take(pageSize);
                }

                return courseList;
            }
        }

        public IEnumerable<StudentCourseList> GetStudentDayOffList(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(s => s.StudentCode == studentCode
                                                            && (s.AttendanceStatusCode == "00" || s.AttendanceStatusCode == "03"))
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderBy(s => s.CourseDate)
                                                .ToList();
            }
        }

        public IEnumerable<StudentCourseComment> GetCourseComments(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseComment.Where(s => s.StudentCode == studentCode)
                                                .OrderByDescending(s => s.CourseDate)
                                                .ToList();
            }
        }

        // public int GetStudentSignInCourseCount(string studentCode)
        // {
        //     int count = 0;
        //     using (BaseContext context = new BaseContext())
        //     {
        //         count = context.StudentCourseList.Where(s => s.StudentCode == studentCode
        //                                                     && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02")
        //                                                     && s.CourseType == "正式")
        //                                 .Count();
        //     }

        //     return count;
        // }

        public DataTable GetBirthdayIn7Days()
        {
            DataTable dtStudent = ADOContext.GetDataTable(@"select student_name, student_birthday from student 
                                                            where student_status = '01'
	                                                            and DATE_FORMAT(student_birthday,'%m-%d') <= DATE_FORMAT(date_add(now(), interval 7 day),'%m-%d')
	                                                            and DATE_FORMAT(student_birthday,'%m-%d') >= DATE_FORMAT(date_sub(now(), interval 3 day),'%m-%d') 
                                                            order by DATE_FORMAT(student_birthday,'%m-%d')");

            return dtStudent;
        }

        public DataTable GetBirthdayIn7Days(int pageIndex, int pageSize)
        {
            int start = (pageIndex - 1) * pageSize;
            int end = pageSize;
            // DataTable dtStudent = ADOContext.GetDataTable(@"select id, student_code, student_name, DATE_FORMAT(student_birthday,'%m-%d') as student_birthday, student_phone, student_avatar_path, 0 as rest_course_count from student 
            //                                                 where student_status = '01'
            //                                                     and student_name = '测试学员' 
            //                                                     and DATE_FORMAT(student_birthday,'%m-%d') <= DATE_FORMAT(date_add(now(), interval 7 day),'%m-%d')
            //                                                     and DATE_FORMAT(student_birthday,'%m-%d') >= DATE_FORMAT(date_sub(now(), interval 3 day),'%m-%d') 
            //                                                 order by DATE_FORMAT(student_birthday,'%m-%d') limit @1, @2", start, end);

            DataTable dtStudent = ADOContext.GetDataTable(@"select id, student_code, student_name, DATE_FORMAT(student_birthday,'%m-%d') as student_birthday, student_phone, student_avatar_path, 0 as rest_course_count from student 
                                                            where student_status = '01' 
	                                                            and DATE_FORMAT(student_birthday,'%m-%d') <= DATE_FORMAT(date_add(now(), interval 7 day),'%m-%d')
	                                                            and DATE_FORMAT(student_birthday,'%m-%d') >= DATE_FORMAT(date_sub(now(), interval 3 day),'%m-%d') 
                                                            order by DATE_FORMAT(student_birthday,'%m-%d') limit @1, @2", start, end);

            return dtStudent;
        }

        #endregion

        #region courseArrange 排课
        public IEnumerable<SysCourseArrangeTemplateDetail> GetCourseArrangePeriod(string templateCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysCourseArrangeTemplateDetail.Where(s => s.ArrangeTemplateCode == templateCode)
                                                            .OrderBy(s => s.CoursePeriod)
                                                            .OrderBy(s => s.CourseWeekDay)
                                                            .ToList();
            }
        }

        public List<StudentCourseList> GetCoursesByday(DateTime theDay)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(c => c.AttendanceStatusCode == "09" && c.CourseDate == theDay)
                                                .OrderBy(c => c.CourseFolderCode)
                                                .OrderBy(c => c.CoursePeriod)
                                                .ToList();
            }
        }

        public DataTable GetScheduleByDay(DateTime theDay)
        {
            string strDay = theDay.ToString("yyyy-MM-dd");
            DataTable dt = ADOContext.GetDataTable($@"select scl.course_period, sd.item_name as classroom_name,s.id,s.student_avatar_path,scl.course_week_day,scl.student_code,scl.student_name,scl.attendance_status_code,scl.attendance_status_name
                                                    from student_course_list scl 
                                                    left join student s on scl.student_code = s.student_code
                                                    left join sys_dictionary sd on scl.classroom = item_code
                                                    where sd.type_code='classroom'
                                                    and scl.course_date = @1
                                                    order by scl.course_period, classroom_name", strDay);

            return dt;
        }

        public List<StudentCourseList> GetStudentWeekCourse(string studentCode, DateTime weekLastDay)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(c => c.StudentCode == studentCode
                                                            && c.AttendanceStatusCode == "09"
                                                            && c.CourseDate >= DateTime.Today
                                                            && c.CourseDate <= weekLastDay)
                                                .OrderBy(c => c.CoursePeriod)
                                                .OrderBy(c => c.CourseDate)
                                                .ToList();
            }
        }

        public IEnumerable<CA_R_PERIOD_STUDENTS> GetAllPeriodStudents(string templateCode, string roomCode)
        {
            using (BaseContext context = new BaseContext())
            {
                // is_this_week 为是否本周有课
                var studentList = context.CA_R_PERIOD_STUDENTS.FromSql($@"select distinctrow sca.id,sca.course_period,sca.course_week_day,sca.student_code,sca.student_name,sca.package_code,
                                                                      scl.course_category_code,scl.course_category_name,scl.course_folder_code,scl.course_folder_name,
                                                                      sca.course_total_count,sca.course_rest_count,sca.course_type,
                                                                      uf_IsThisWeekHasCourse(sca.student_code,sca.course_week_day,sca.course_period,sca.classroom) as is_this_week
                                                                from student_course_arrange sca
                                                                left join student_course_package scp on sca.student_course_package_id = scp.id 
                                                                left join student_course_list scl on sca.arrange_guid = scl.arrange_guid 
                                                                where sca.arrange_template_code = {templateCode} and sca.classroom= {roomCode} 
                                                                and (scp.scp_status = '00' or scp.scp_status is null) 
                                                                and sca.course_rest_count > 0 
                                                                and scl.attendance_status_code = '09' 
                                                                order by scl.course_folder_code")
                                                            .ToList();
                foreach (var item in studentList)
                {
                    if (item.CourseType == "试听")
                    {
                        var cl = context.StudentCourseList.Where(s => s.CourseType == "试听" && s.StudentCode == item.StudentCode && s.AttendanceStatusCode == "09").First();
                        item.CourseFolderCode = cl.CourseFolderCode;
                        item.CourseFolderName = cl.CourseFolderName;
                    }
                }
                return studentList;
            }
        }

        // 获取时间段内排课信息
        public IEnumerable<CA_R_PERIOD_STUDENTS> GetPeriodStudents(string templateCode, string roomCode, string dayCode, string periodName)
        {
            using (BaseContext context = new BaseContext())
            {

                // is_this_week 为是否本周有课
                var studentList = context.CA_R_PERIOD_STUDENTS.FromSql($@"select distinctrow sca.id,sca.course_period,sca.course_week_day,sca.student_code,sca.student_name,sca.package_code,
                                                                      scl.course_category_code,scl.course_category_name,scl.course_folder_code,scl.course_folder_name, 
                                                                      sca.course_total_count,sca.course_rest_count,sca.course_type,
                                                                      uf_IsThisWeekHasCourse(sca.student_code,{dayCode},{periodName},{roomCode}) as is_this_week
                                                                from student_course_arrange sca
                                                                left join student_course_package scp on sca.student_course_package_id = scp.id 
                                                                left join student_course_list scl on sca.arrange_guid = scl.arrange_guid 
                                                                where sca.arrange_template_code = {templateCode} and sca.classroom = {roomCode} 
                                                                       and sca.course_week_day = {dayCode} and sca.course_period = {periodName} 
                                                                       and sca.course_rest_count > 0
                                                                       and (scp.scp_status = '00' or scp.scp_status is null) 
                                                                       and scl.attendance_status_code = '09' 
                                                                order by scl.course_folder_code")
                                                            .ToList();
                foreach (var item in studentList)
                {
                    if (item.CourseType == "试听")
                    {
                        var cl = context.StudentCourseList.Where(s => s.CourseType == "试听" && s.StudentCode == item.StudentCode && s.AttendanceStatusCode == "09").First();
                        item.CourseFolderCode = cl.CourseFolderCode;
                        item.CourseFolderName = cl.CourseFolderName;
                    }
                }
                return studentList;
            }
        }

        public IEnumerable<StudentCoursePackage> GetStudentToSelectCourse(string dayCode, string periodName)
        {
            // 不显示当前时间段内已经排过课的学生 与模板和教室无关
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCoursePackage.FromSql($@"select scp.* from student_course_package  scp 
                                                                where scp.scp_status = '00' and scp.flex_course_count > 0 and not exists(
                                                                    select 1 from student_course_arrange sca 
                                                                    where sca.student_code = scp.student_code 
                                                                    and sca.course_week_day = {dayCode} and sca.course_period = {periodName} 
                                                                    and sca.course_rest_count > 0 
                                                                )")
                                                    .ToList();
            }
        }

        public IEnumerable<StudentTemp> GetTempStudentToSelectCourse()
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentTemp.Where(s => s.StudentTempStatus == "00").ToList();
            }
        }

        // 获取学生排课列表
        public IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Simplify_StudentCourseList.FromSql($@"select student_course_id,course_category_name,course_folder_name,course_date,attendance_status_code,attendance_status_name,course_type 
                                                                    from student_course_list
                                                                    where student_code={studentCode} and course_week_day={dayCode} and course_period={coursePeriod} 
                                                                          and (((attendance_status_code = '00' or attendance_status_code = '03') and course_date >= DATE_FORMAT(now(),'%Y-%m-%d')) or attendance_status_code = '09')
                                                                    order by course_date")
                                                                .ToList();
            }
        }

        // 获取假期列表
        public IEnumerable<SysHoliday> GetHolidayList()
        {
            using (BaseContext context = new BaseContext())
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                return context.SysHoliday.Where(h => h.HolidayDate >= firstDay)
                                        .OrderBy(h => h.HolidayDate)
                                        .ToList();
            }
        }
        #endregion

        #region  待签到
        // 获取学生排课列表
        public IEnumerable<StudentCourseList> GetCoursesToSignIn()
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                                                            && s.CourseDate <= DateTime.Now.Date)
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderBy(s => s.CourseDate)
                                                .ToList();
            }
        }

        public IEnumerable<StudentCourseList> GetCoursesToSignIn(string classroomCode, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                // return context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                //                                             && s.StudentName == "测试学员"
                //                                             && s.Classroom == classroomCode
                //                                             && s.CourseDate <= DateTime.Now.Date)
                //                                 .OrderBy(s => s.CoursePeriod)
                //                                 .OrderBy(s => s.CourseDate)
                //                                 .Skip((pageIndex - 1) * pageSize)
                //                                 .Take(pageSize)
                //                                 .ToList();

                return context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                                                            && s.Classroom == classroomCode
                                                            && s.CourseDate <= DateTime.Now.Date)
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderBy(s => s.CourseDate)
                                                .Skip((pageIndex - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();
            }
        }

        public DataTable GetSignTimeCategory(string classroomCode)
        {
            DataTable dt = ADOContext.GetDataTable(@"select course_date, course_period  
                                                    from student_course_list
                                                    where classroom=@1 and attendance_status_code='09'
                                                    and course_date <= DATE_FORMAT(now(),'%y-%m-%d')
                                                    group by course_date, course_period  
                                                    order by course_date, course_period", classroomCode);

            return dt;
        }

        public int GetCoursesToSignInCount()
        {
            int courseCount = -1;
            using (BaseContext context = new BaseContext())
            {
                courseCount = context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                                                            && s.CourseDate <= DateTime.Now.Date)
                                                .Count();
            }
            return courseCount;
        }

        public int GetCoursesToSignInCount(string classroomCode)
        {
            int courseCount = -1;
            using (BaseContext context = new BaseContext())
            {
                // courseCount = context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                //                                             && s.Classroom == classroomCode
                //                                             && s.StudentName == "测试学员"
                //                                             && s.CourseDate <= DateTime.Now.Date)
                //                                 .Count();
                courseCount = context.StudentCourseList.Where(s => s.AttendanceStatusCode == "09"
                                                            && s.Classroom == classroomCode
                                                            && s.CourseDate <= DateTime.Now.Date)
                                                .Count();
            }
            return courseCount;
        }

        public int GetTodayCourseCount()
        {
            int courseCount = 0;
            using (BaseContext context = new BaseContext())
            {
                // courseCount = context.StudentCourseList.Where(s => s.CourseDate == DateTime.Today && s.StudentName == "测试学员").Count();
                courseCount = context.StudentCourseList.Where(s => s.CourseDate == DateTime.Today).Count();
            }
            return courseCount;
        }

        public DataTable GetExpirationStudents()
        {
            // 当前学生是正常在学的。如果当前套餐剩余0课时， 但是学生状态依然是正常，说明还有其他课程套餐。这种情况不提醒了。
            DataTable dt = ADOContext.GetDataTable(@"select scp.student_code, scp.student_name, scp.package_name, scp.rest_course_count 
                                                        from student_course_package scp
                                                        left join student s on scp.student_code = s.student_code
                                                        where s.student_status='01' and scp.scp_status = '00' and scp.rest_course_count <= 5");

            return dt;
        }

        public DataTable GetExpirationStudents(int pageIndex, int pageSize)
        {
            int s = (pageIndex - 1) * pageSize;
            int e = pageSize;
            // 当前学生是正常在学的。如果当前套餐剩余0课时， 但是学生状态依然是正常，说明还有其他课程套餐。这种情况不提醒了。
            // DataTable dt = ADOContext.GetDataTable(@"select s.id, scp.student_code, scp.student_name, scp.package_name, scp.rest_course_count, s.student_phone, s.student_avatar_path 
            //                                         from student_course_package scp
            //                                         left join student s on scp.student_code = s.student_code
            //                                         where s.student_status='01' and s.student_name='测试学员' and scp.scp_status = '00' and scp.rest_course_count <= 5
            //                                         limit @1,@2", s, e);
            DataTable dt = ADOContext.GetDataTable(@"select s.id, scp.student_code, scp.student_name, scp.package_name, scp.rest_course_count, s.student_phone, s.student_avatar_path 
                                                    from student_course_package scp
                                                    left join student s on scp.student_code = s.student_code
                                                    where s.student_status='01' and scp.scp_status = '00' and scp.rest_course_count <= 5
                                                    limit @1,@2", s, e);

            return dt;
        }
        #endregion

        #region  作品
        public IEnumerable<StudentArtwork> GetArtworkByCourse(int courseId)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentArtwork.Where(s => s.StudentCourseId == courseId
                                                            && s.ArtworkStatus == "01")
                                            .ToList();
            }
        }

        public IEnumerable<StudentArtwork> GetArkworkByStudent(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentArtwork.Where(s => s.StudentCode == studentCode
                                                            && s.ArtworkStatus == "01")
                                            .OrderByDescending(s => s.FinishDate)
                                            .ToList();
            }
        }

        public int GetStudentArkworkCount(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentArtwork.Where(s => s.StudentCode == studentCode
                                                            && s.ArtworkStatus == "01")
                                            .Count();
            }
        }

        public List<StudentArtwork> GetArkworkByStudent(string studentCode, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentArtwork.Where(s => s.StudentCode == studentCode
                                                            && s.ArtworkStatus == "01")
                                            .OrderByDescending(s => s.FinishDate)
                                            .Skip(pageSize * (pageIndex - 1))
                                            .Take(pageSize)
                                            .ToList();
            }
        }

        public string GetArtWorkTruePath(int artworkId)
        {
            string path = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var aw = context.StudentArtwork.Where(s => s.ArtworkId == artworkId).FirstOrDefault();
                if (aw != null)
                {
                    path = aw.DocumentPath;
                }
            }

            return path;
        }

        #endregion

        #region sys_course_package
        public SysCoursePackage GetSysCoursePackage(string packageCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysCoursePackage.Where(p => p.PackageCode == packageCode).First();
            }
        }

        public IEnumerable<SysCoursePackage> GetSysCoursePackageList(QUERY_SYS_PACKAGE query)
        {
            IEnumerable<SysCoursePackage> packages = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<SysCoursePackage> temp = null;

                if (!string.IsNullOrEmpty(query.packageEnabled) && query.packageEnabled.ToLower() != "all")
                {
                    temp = context.SysCoursePackage.Where(s => s.PackageEnabled == query.packageEnabled);
                }
                if (!string.IsNullOrEmpty(query.packageName.Trim()))
                {
                    if (temp == null)
                    {
                        temp = context.SysCoursePackage;
                    }
                    temp = temp.Where(s => EF.Functions.Like(s.PackageName, "%" + query.packageName.Trim() + "%"));
                }
                if (temp == null)
                {
                    packages = context.SysCoursePackage.OrderBy(s => s.PackageCourseCategoryCode).ToList();
                }
                else
                {
                    packages = temp.ToList();
                }
                return packages;
            }
        }

        public bool isPackageUsed(int packageId)
        {
            bool result = false;
            using (BaseContext context = new BaseContext())
            {
                var package = context.SysCoursePackage.Where(scp => scp.Id == packageId).First();
                string packageCode = package.PackageCode;
                int count = context.StudentCoursePackage.Where(scp => scp.PackageCode == packageCode).Count();
                if (count > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region activity
        public List<SysActivity> GetActivityList(QUERY_SYS_ACTIVITY query, out int totalCount)
        {
            List<SysActivity> activities = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<SysActivity> temp = null;

                if (!String.IsNullOrEmpty(query.activitySubject))
                {
                    temp = context.SysActivity.Where(s => EF.Functions.Like(s.ActivitySubject, "%" + query.activitySubject + "%"));
                }

                if (temp == null)
                {
                    temp = context.SysActivity.Where(s => 1 == 1);
                }
                totalCount = temp.Count();
                activities = temp.Skip(query.pageSize * (query.pageIndex - 1)).Take(query.pageSize).ToList();

                return activities;
            }
        }

        public SysActivity GetActivityById(int activityId)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysActivity.FirstOrDefault(s => s.ActivityId == activityId);
            }
        }

        public List<StudentActivity> GetStudentByActivity(int activityId)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentActivity.Where(s => s.ActivityId == activityId).ToList();
            }
        }

        #endregion

        #region teacher
        public IEnumerable<DIC_R_KEY_VALUE> GetTeacherToCharge()
        {
            using (BaseContext context = new BaseContext())
            {
                var teachers = context.DIC_R_KEY_VALUE.FromSql($@"select tr.teacher_code as item_code, t.teacher_name as item_name
                                                                    from teacher_role tr
                                                                    left join teacher t on tr.teacher_code = t.teacher_code
                                                                    where tr.role_code='1009' and t.teacher_status='01'
                                                                    order by t.teacher_name")
                                                        .ToList();

                return teachers;
            }
        }
        public IEnumerable<Teacher> GetTeacherList(QUERY_TEACHER query)
        {
            IEnumerable<Teacher> teachers = null;
            using (BaseContext context = new BaseContext())
            {
                IQueryable<Teacher> temp = null;

                if (!string.IsNullOrEmpty(query.teacherStatus))
                {
                    temp = context.Teacher.Where(t => t.TeacherStatus == query.teacherStatus);
                }
                if (!string.IsNullOrEmpty(query.teacherName.Trim()))
                {
                    if (temp == null)
                    {
                        temp = context.Teacher;
                    }
                    temp = temp.Where(t => EF.Functions.Like(t.TeacherName, "%" + query.teacherName.Trim() + "%"));
                }
                if (temp == null)
                {
                    teachers = context.Teacher.OrderBy(t => t.TeacherCode).ToList();
                }
                else
                {
                    teachers = temp.ToList();
                }
                return teachers;
            }
        }

        public DataTable GetAllCourseRoleTeachers()
        {
            DataTable dtTeacher = ADOContext.GetDataTable($@"select tr.role_code,tr.teacher_code, t.teacher_name,tr.role_level 
                                                            from teacher_role tr 
                                                            left join teacher t on tr.teacher_code = t.teacher_code 
                                                            where role_level='course' and t.teacher_status='01'");

            return dtTeacher;
        }

        public DataTable GetTeacherListWithRole(string roleCode)
        {
            DataTable dtTeacher = ADOContext.GetDataTable($@"select t.teacher_code,t.teacher_name,tr.role_code 
                                                            from teacher t 
                                                            left join teacher_role tr on t.teacher_code = tr.teacher_code and tr.role_code='{roleCode}'
                                                            where t.teacher_status='01'");

            return dtTeacher;
        }

        public Teacher GetTeacher(string teacherCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Teacher.Where(t => t.TeacherCode == teacherCode).FirstOrDefault();
            }
        }

        public string getTeacherCodeByName(string teacherName)
        {
            string teacherCode = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var user = context.SysUser.Where(s => s.LoginCode == teacherName).FirstOrDefault();
                if (user != null)
                {
                    teacherCode = user.TeacherCode;
                }
            }
            return teacherCode;
        }

        public IEnumerable<StudentCourseList> GetTeacherCourseList(string teacherCode, int pageIndex, int pageSize, QUERY_TEACHER_COURSE query, out int totalCount)
        {
            using (BaseContext context = new BaseContext())
            {
                var courseLists = context.StudentCourseList.Where(s => s.TeacherCode == teacherCode
                                                            && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"));
                if (query.startDate != null)
                {
                    courseLists = courseLists.Where(s => s.CourseDate >= query.startDate);
                }
                if (query.endDate != null)
                {
                    courseLists = courseLists.Where(s => s.CourseDate <= query.endDate);
                }
                courseLists = courseLists.OrderBy(s => s.CoursePeriod).OrderBy(s => s.CourseDate);
                totalCount = courseLists.Count();
                courseLists = courseLists.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

                return courseLists.ToList();
            }
        }

        public IEnumerable<StudentCourseList> GetTeacherCourseList2Export(string teacherCode, QUERY_TEACHER_COURSE query)
        {
            using (BaseContext context = new BaseContext())
            {
                var courseLists = context.StudentCourseList.Where(s => s.TeacherCode == teacherCode
                                                            && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"));
                if (query.startDate != null)
                {
                    courseLists = courseLists.Where(s => s.CourseDate >= query.startDate);
                }
                if (query.endDate != null)
                {
                    courseLists = courseLists.Where(s => s.CourseDate <= query.endDate);
                }

                return courseLists.OrderBy(s => s.CoursePeriod).OrderBy(s => s.CourseDate).ToList();
            }
        }
        #endregion 

        #region  weixin 小程序
        public SysWxUser GetWxUserByOpenId(string openId)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysWxUser.FirstOrDefault(s => s.OpenId == openId);
            }
        }

        public SysWxUser GetWxUserBySKey(string sKey)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysWxUser.FirstOrDefault(s => s.SessionKey == sKey);
            }
        }

        public bool IsStudentExist(string studentCode, string studentName)
        {
            bool flag = false;
            using (BaseContext context = new BaseContext())
            {
                var student = context.Student.FirstOrDefault(s => s.StudentCode == studentCode && s.StudentName == studentName);
                if (student != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public string GetTeacherCodeByWxKey(string wxKey, out string teacherName)
        {
            string result = string.Empty;
            teacherName = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var teacher = context.Teacher.FirstOrDefault(s => s.TeacherWxkey == wxKey);
                if (teacher != null)
                {
                    result = teacher.TeacherCode;
                    teacherName = teacher.TeacherName;
                }
            }
            return result;
        }

        public IEnumerable<WxPicture> GetWxPicture(string picTypeCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.WxPicture.Where(s => s.WxPictureType == picTypeCode).ToList();
            }
        }

        public IEnumerable<WxPicture> GetWxPicture(string picTypeCode, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.WxPicture.Where(s => s.WxPictureType == picTypeCode)
                                        .OrderByDescending(s => s.Id)
                                        .OrderByDescending(x => x.RateLevel)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();
            }
        }

        public DataTable GetWxPictureDT(string picTypeCode, int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                int s = (pageIndex - 1) * pageSize;
                int e = pageSize;

                string sql = @"select wp.id, wp.subject, wp.student_name as studentName, wp.student_age as studentAge, wp.picture_path as picturePath,wp.rate_level, s.id as studentId,s.student_avatar_path as avatarPath
                                from wx_picture wp 
                                left join student s on wp.student_code = s.student_code
                                where wp.wx_picture_type = @1 
                                order by wp.rate_level desc, wp.id desc limit @2, @3";
                DataTable dt = ADOContext.GetDataTable(sql, picTypeCode, s, e);

                return dt;
            }
        }

        public IEnumerable<WxPicture> GetWxHomePicture()
        {
            //只取机构环境01与学习日常00
            using (BaseContext context = new BaseContext())
            {
                return context.WxPicture.Where(s => s.WxPictureType == "00" || s.WxPictureType == "01").ToList();
            }
        }

        public string GetWeiXinPicTruePath(int id)
        {
            string path = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var st = context.WxPicture.Where(s => s.Id == id).FirstOrDefault();
                if (st != null)
                {
                    path = st.PicturePath;
                }
            }

            return path;
        }
        #endregion

        public string GetAvatarTruePath(int id, string type)
        {
            string path = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                if (type == "student")
                {
                    var st = context.Student.Where(s => s.Id == id).FirstOrDefault();
                    if (st != null)
                    {
                        path = st.StudentAvatarPath;
                    }
                }
                else if (type == "teacher")
                {
                    var teacher = context.Teacher.Where(s => s.Id == id).FirstOrDefault();
                    if (teacher != null)
                    {
                        path = teacher.TeacherAvatarPath;
                    }
                }
            }

            return path;
        }








        #region test
        public StudentDescTest GetStudentDescTest(string studentCode)
        {
            // ado
            DataTable aa = ADOContext.GetDataTable("select * from student");
            DataTable bb = ADOContext.GetDataTable("select * from student where student_code=@1", studentCode);

            // linq
            using (BaseContext context = new BaseContext())
            {
                var cc = context.Student.Where(s => s.StudentCode == studentCode).FirstOrDefault();
            }

            // raw sql
            using (BaseContext context = new BaseContext())
            {
                return context.StudentDescTest.FromSql($@"select s.id,s.student_name,s.student_status,d.item_name as student_status_desc 
                    from student s 
                    left join sys_dictionary d on  s.student_status = d.item_code and d.type_code = 'student_status' 
                    where s.student_code = {studentCode}
                ").FirstOrDefault();
            }
        }

        #endregion
    }
}