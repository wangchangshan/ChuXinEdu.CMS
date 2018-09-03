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
                return context.Simplify_StudentCourse.FromSql(@"select scp.id,scp.student_code,scp.package_course_category as course_category_code, d.item_name as course_category_name 
                                        from student_course_package scp 
                                        left join sys_dictionary d on scp.package_course_category = d.item_code and d.type_code='course_category'").ToList();
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
                return context.StudentCourseArrange.Where(s => s.ArrangeTemplateCode == templateCode && s.Classroom == roomCode && s.CourseRestCount > 0)
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