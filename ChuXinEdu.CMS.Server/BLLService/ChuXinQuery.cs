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
        #region student
        public IEnumerable<Student> GetStudentList(int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                IEnumerable<Student> students = context.Student.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                return context.Student.ToList();
            }
        }

        // 所有学生的课程大类
        public IEnumerable<Simplify_StudentCourse> GetAllStudentsCourse()
        {
            using (BaseContext context = new BaseContext())
            {
            //     return context.Simplify_StudentCourse.FromSql(@"select scp.id,scp.student_code,scp.package_course_category as course_category_code, d.item_name as course_category_name 
            //                             from student_course_package scp 
            //                             left join sys_dictionary d on scp.package_course_category = d.item_code and d.type_code='course_category'").ToList();
                return context.Simplify_StudentCourse.FromSql(@"select id,student_code,course_category_code,course_category_name from student_course_package")
                                                        .ToList();
            }
        }

        public IEnumerable<Student> GetStudentsByName(string studentName)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Student.Where(s => EF.Functions.Like(s.StudentName, "%" + studentName + "%")).ToList();
            }
        }

        public Student GetStudentBaseByCode(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Student.Where(s => s.StudentCode == studentCode).FirstOrDefault();
            }
        }
        #endregion


        #region courseArrange
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

        public IEnumerable<StudentCourseArrange> GetStudentCourseArrage(string templateCode, string roomCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseArrange.Where(s => s.ArrangeTemplateCode == templateCode 
                                                                && s.Classroom == roomCode 
                                                                && s.CourseRestCount > 0)
                                                    .OrderBy(s => s.CourseCategoryCode)
                                                    .ToList();
            }
        }

        public IEnumerable<StudentCoursePackage> GetStudentToSelectCourse(string dayCode, string periodName)
        {
            // 不显示当前时间段内已经排过课的学生 与模板和教室无关
            using (BaseContext context = new BaseContext())
            {   
                return context.StudentCoursePackage.FromSql($@"select scp.* from student_course_package  scp 
                                                                where not exists(
                                                                    select 1 from student_course_arrange sca 
                                                                    where sca.student_code = scp.student_code 
                                                                    and sca.course_week_day = {dayCode} and sca.course_period = {periodName} 
                                                                    and sca.course_rest_count > 0 
                                                                )")
                                                    .ToList();
            }
        }

        // 获取学生排课列表
        public IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod)
        {
             using (BaseContext context = new BaseContext())
            {
                return context.Simplify_StudentCourseList.FromSql($@"select student_course_id,course_category_name,course_date,attendance_status_code,attendance_status_name 
                                                                    from student_course_list
                                                                    where student_code={studentCode} and course_week_day={dayCode} and course_period={coursePeriod}
                                                                        and attendance_status_code = '09'
                                                                    order by course_date;")
                                                                .ToList();
            }
        }

        
        // 获取时间段内排课信息
        public IEnumerable<StudentCourseArrange> GetArrangedByPeriod(string templateCode, string roomCode, string dayCode, string periodName)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseArrange.Where(s => s.ArrangeTemplateCode == templateCode
                                                                && s.Classroom == roomCode
                                                                && s.CourseWeekDay == dayCode
                                                                && s.CoursePeriod == periodName
                                                                && s.CourseRestCount > 0 )
                                                    .OrderBy(s => s.CourseCategoryCode)
                                                    .ToList();
            }
        }

        // 获取假期列表
        public IEnumerable<SysHoliday> GetHolidayList()
        {
            using (BaseContext context = new BaseContext())
            {
                DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                return context.SysHoliday.Where( h => h.HolidayDate >= firstDay)
                                        .OrderBy(h => h.HolidayDate)
                                        .ToList();
            }
        }
        #endregion











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