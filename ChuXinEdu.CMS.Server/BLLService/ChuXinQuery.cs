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
        #region teacher
        public IEnumerable<DIC_R_KEY_VALUE> GetTeacherToCharge()
        {
            using (BaseContext context = new BaseContext())
            {
                var teachers = context.DIC_R_KEY_VALUE.FromSql($@"select teacher_code as item_code, teacher_name as item_name 
                                                                  from teacher 
                                                                  where teacher_status = '01'
                                                                  order by teacher_name")
                                                        .ToList();

                return teachers;
            }
        }
        #endregion

        #region student
        public IEnumerable<Student> GetStudentList(int pageIndex, int pageSize, QUERY_STUDENT query, out int totalCount)
        {
            IEnumerable<Student> students = null;
            using (BaseContext context = new BaseContext())
            {
                if (!String.IsNullOrEmpty(query.studentName))
                {
                    totalCount = context.Student.Where(s => EF.Functions.Like(s.StudentName, "%" + query.studentName + "%")).Count();
                    students = context.Student.Where(s => EF.Functions.Like(s.StudentName, "%" + query.studentName + "%")).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    totalCount = context.Student.Count();
                    students = context.Student.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                }

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

        public IEnumerable<StudentTemp> GetTempStudentList(int pageIndex, int pageSize)
        {
            using (BaseContext context = new BaseContext())
            {
                var tempStudents = context.StudentTemp.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

                return tempStudents;
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
                return context.Simplify_StudentCourse.FromSql(@"select id, student_code,course_category_code,course_category_name from student_course_package")
                                                        .ToList();
            }
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
                return context.StudentCoursePackage.Where(s => s.StudentCode == studentCode).ToList();
            }
        }

        public IEnumerable<StudentCourseList> GetStudentCourseList(string studentCode)
        {
            using (BaseContext context = new BaseContext())
            {
                return context.StudentCourseList.Where(s => s.StudentCode == studentCode
                                                            && (s.AttendanceStatusCode == "01" || s.AttendanceStatusCode == "02"))
                                                .OrderBy(s => s.CoursePeriod)
                                                .OrderBy(s => s.CourseDate)
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
	                                                            and DATE_FORMAT(student_birthday,'%m-%d') >= DATE_FORMAT(now(),'%m-%d')
                                                            order by DATE_FORMAT(student_birthday,'%m-%d')");

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

        public IEnumerable<CA_R_PERIOD_STUDENTS> GetAllPeriodStudents(string templateCode, string roomCode)
        {
            using (BaseContext context = new BaseContext())
            {
                // is_this_week 为是否本周有课
                var studentList = context.CA_R_PERIOD_STUDENTS.FromSql($@"select sca.id,sca.course_period,sca.course_week_day,sca.student_code,sca.student_name,sca.package_code,
                                                                      scp.course_category_code,scp.course_category_name,scp.course_folder_code,scp.course_folder_name,
                                                                      sca.course_total_count,sca.course_rest_count,sca.course_type,
                                                                      uf_IsThisWeekHasCourse(sca.student_code,sca.course_week_day,sca.course_period,sca.classroom) as is_this_week
                                                                from student_course_arrange sca
                                                                left join student_course_package scp on sca.student_course_package_id = scp.id 
                                                                where sca.arrange_template_code = {templateCode} and sca.classroom= {roomCode} and sca.course_rest_count > 0
                                                                order by scp.course_folder_code")
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
                var studentList = context.CA_R_PERIOD_STUDENTS.FromSql($@"select sca.id,sca.course_period,sca.course_week_day,sca.student_code,sca.student_name,sca.package_code,
                                                                      scp.course_category_code,scp.course_category_name,scp.course_folder_code,scp.course_folder_name, 
                                                                      sca.course_total_count,sca.course_rest_count,sca.course_type,
                                                                      uf_IsThisWeekHasCourse(sca.student_code,{dayCode},{periodName},{roomCode}) as is_this_week
                                                                from student_course_arrange sca
                                                                left join student_course_package scp on sca.student_course_package_id = scp.id 
                                                                where sca.arrange_template_code = {templateCode} and sca.classroom = {roomCode} and sca.course_week_day = {dayCode} and sca.course_period = {periodName} 
                                                                      and sca.course_rest_count > 0
                                                                order by scp.course_folder_code")
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
                                                                where scp.flex_course_count > 0 and not exists(
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
                return context.Simplify_StudentCourseList.FromSql($@"select student_course_id,course_category_name,course_date,attendance_status_code,attendance_status_name,course_type 
                                                                    from student_course_list
                                                                    where student_code={studentCode} and course_week_day={dayCode} and course_period={coursePeriod} 
                                                                          and (attendance_status_code = '00' or attendance_status_code = '03' or attendance_status_code = '09')
                                                                    order by course_date;")
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

        public DataTable GetCourseToFinishList()
        {
            // 暂时没有考虑 是否已经报了新的套餐
            DataTable dt = ADOContext.GetDataTable(@"select student_code, student_name, package_name, rest_course_count from student_course_package where rest_course_count <= 5");

            return dt;
        }
        #endregion

        #region  作品
        public IEnumerable<StudentArtwork> GetArkworkByCourse(int courseId)
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
                                            .OrderBy(s => s.FinishDate)
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

        public IEnumerable<SysCoursePackage> GetSysCoursePackageList()
        {
            using (BaseContext context = new BaseContext())
            {
                return context.SysCoursePackage.OrderBy(s => s.PackageCourseCategoryCode).ToList();
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

        #region teacher
        public IEnumerable<Teacher> GetTeacherList()
        {
            using (BaseContext context = new BaseContext())
            {
                return context.Teacher.ToList();
            }
        }
        #endregion 

        public string GetAvatarTruePath(int studentId)
        {
            string path = string.Empty;
            using (BaseContext context = new BaseContext())
            {
                var st = context.Student.Where(s => s.Id == studentId).FirstOrDefault();
                if (st != null)
                {
                    path = st.StudentAvatarPath;
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