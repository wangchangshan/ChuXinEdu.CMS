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
    public class ChuXinWorkFlow : IChuXinWorkFlow
    {
        public string BatchStudentsCourseArrange(CA_C_STUDENTS_MAIN caInfo)
        {
            string result = "200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    foreach (var student in caInfo.StudentList)
                    {
                        // 1. 向student_course_arrange表中插入数据
                        context.StudentCourseArrange.Add(new StudentCourseArrange
                        {
                            ArrangeTemplateCode = caInfo.TemplateCode,
                            Classroom = caInfo.RoomCode,
                            CoursePeriod = caInfo.PeriodName,
                            CourseWeekDay = caInfo.DayCode,
                            StudentCode = student.StudentCode,
                            StudentName = student.StudentName,
                            PackageCode = student.PackageCode,
                            CourseCategoryCode = student.CourseCategoryCode,
                            CourseCategoryName = student.CourseCategoryName,
                            CourseTotalCount = student.CourseCount,
                            CourseRestCount = student.CourseCount,
                            CourseType = "正式"
                        });


                        // 2. 向student_course_list表中插入数据（每节课一条记录）
                        DateTime firstCourseDate = student.StartDate.Date;
                        int courseCount = student.CourseCount;
                        for (int i = 0; i < courseCount; i++)
                        {
                            context.StudentCourseList.Add(new StudentCourseList
                            {
                                ArrangeTemplateCode = caInfo.TemplateCode,
                                Classroom = caInfo.RoomCode,
                                CoursePeriod = caInfo.PeriodName,
                                CourseWeekDay = caInfo.DayCode,
                                CourseDate = firstCourseDate.AddDays(i * 7),
                                StudentCode = student.StudentCode,
                                StudentName = student.StudentName,
                                PackageCode = student.PackageCode,
                                CourseCategoryCode = student.CourseCategoryCode,
                                CourseCategoryName = student.CourseCategoryName,
                                CourseFolderCode = "",
                                CourseFolderName = "",
                                CourseType = "正式",
                                AttendanceStatusCode = "09",
                                AttendanceStatusName = "待上课"
                            });
                        }

                        // 3. 更新student_course_package表中的flex_course_count字段
                        var scp = context.StudentCoursePackage.Where(s => s.StudentCode == student.StudentCode && s.PackageCode == student.PackageCode)
                                                                .FirstOrDefault();
                        scp.FlexCourseCount = scp.FlexCourseCount - student.CourseCount;

                        // 4. 提交事务
                        context.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                result = "500";
            }

            return result;
        }

        public string SingleQingJia(int studentCourseId)
        {
            string result = "200";
            try
            {
                using (BaseContext context = new BaseContext())
                {

                    // 1. 更新学生课程表
                    var studentCourse = context.StudentCourseList.FirstOrDefault(s => s.StudentCourseId == studentCourseId);
                    studentCourse.AttendanceStatusCode = "00";
                    studentCourse.AttendanceStatusName = "个人请假";

                    // 2. 更新学生排课表 课程 减 1
                    string studentCode = studentCourse.StudentCode;
                    string templateCode = studentCourse.ArrangeTemplateCode;
                    string packageCode = studentCourse.PackageCode;
                    string dayCode = studentCourse.CourseWeekDay;
                    string coursePeriod = studentCourse.CoursePeriod;
                    string roomCode = studentCourse.Classroom;

                    var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                                && s.ArrangeTemplateCode == templateCode
                                                                                && s.PackageCode == packageCode
                                                                                && s.CourseWeekDay == dayCode
                                                                                && s.Classroom == roomCode
                                                                                && s.CoursePeriod == coursePeriod)
                                                                        .FirstOrDefault();
                    courseArrange.CourseTotalCount -= 1;
                    courseArrange.CourseRestCount -= 1;

                    // 3. 更新学生套餐表， 套餐内未排课课时数 加 1
                    var studentCoursePackage = context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                                        && s.PackageCode == packageCode)
                                                                            .FirstOrDefault();
                    studentCoursePackage.FlexCourseCount += 1;

                    // 4. 提交事务
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                result = "500";
            }
            return result;
        }

        public string SingleRemoveCourse(int studentCourseId)
        {
            string result = "200";
            try
            {
                using (BaseContext context = new BaseContext())
                {

                    // 1. 删除学生课程表内排课记录
                    var studentCourse = context.StudentCourseList.FirstOrDefault(s => s.StudentCourseId == studentCourseId);
                    context.StudentCourseList.Remove(studentCourse);

                    // 2. 更新学生排课表 课程 减 1
                    string studentCode = studentCourse.StudentCode;
                    string templateCode = studentCourse.ArrangeTemplateCode;
                    string packageCode = studentCourse.PackageCode;
                    string dayCode = studentCourse.CourseWeekDay;
                    string coursePeriod = studentCourse.CoursePeriod;
                    string roomCode = studentCourse.Classroom;

                    var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                                && s.ArrangeTemplateCode == templateCode
                                                                                && s.PackageCode == packageCode
                                                                                && s.CourseWeekDay == dayCode
                                                                                && s.Classroom == roomCode
                                                                                && s.CoursePeriod == coursePeriod)
                                                                        .FirstOrDefault();
                    courseArrange.CourseTotalCount -= 1;
                    courseArrange.CourseRestCount -= 1;

                    // 3. 更新学生套餐表， 套餐内未排课课时数 加 1
                    var studentCoursePackage = context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                                        && s.PackageCode == packageCode)
                                                                            .FirstOrDefault();
                    studentCoursePackage.FlexCourseCount += 1;

                    // 4. 提交事务
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                result = "500";
            }
            return result;
        }

        public string AddHoliday(SysHoliday holiday)
        {
            string result = "200";
            try
            {
                using(BaseContext context = new BaseContext())
                {
                    // 1. 向sys_holiday表中插入数据
                    context.SysHoliday.Add(holiday);

                    // 2. 更新student_course_list表 修改相关课程状态为统一放假
                    List<StudentCourseList> studentcourseList = context.StudentCourseList.Where(s => s.CourseDate == holiday.HolidayDate
                                                                                                    && s.AttendanceStatusCode == "09")
                                                                                        .ToList();
                    foreach(var course in studentcourseList)
                    {
                        course.AttendanceStatusCode = "03";
                        course.AttendanceStatusName = "统一放假";

                        string studentCode = course.StudentCode;
                        string dayCode = course.CourseWeekDay;
                        string periodName = course.CoursePeriod;

                        // 3. 更新student_course_arrange表 (由于一个时间段内的学生是唯一的 跟模板和套餐没有关系，所以根据学生编码，星期几，时间段过滤即可)
                        StudentCourseArrange sca = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                                            && s.CourseWeekDay == dayCode
                                                                                            && s.CoursePeriod == periodName)
                                                                                .FirstOrDefault();
                        sca.CourseTotalCount -= 1;
                        sca.CourseRestCount -=1;

                        // 4. 更新student_course_package表 修改剩余选课课时数目
                        string packageCode = course.PackageCode;
                        StudentCoursePackage scp = context.StudentCoursePackage.Where( s => s.StudentCode == studentCode
                                                                                            && s.PackageCode == packageCode)
                                                                                .FirstOrDefault();
                        scp.FlexCourseCount += 1;

                        // 5. 提交事物
                        //context.SaveChanges();
                    }
                    
                    // 5. 提交事物
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                result = "500";
            }
            
            return result;
        }

        public string RemoveHoliday(string strDay)
        {
            string result = "200";
            try
            {
                DateTime theDay = DateTime.Parse(strDay);
                using(BaseContext context = new BaseContext())
                {
                    // 1. 删除sys_holiday表中的假期记录
                    var sysHoliday = context.SysHoliday.Where( h => h.HolidayDate == theDay)
                                                        .FirstOrDefault();
                    context.Remove(sysHoliday);

                    // 2.更新student_course_list表， 修改状态为待上课
                    var scl = context.StudentCourseList.Where( s => s.CourseDate == theDay).ToList();
                    foreach( var course in scl)
                    {
                        course.AttendanceStatusCode = "09";
                        course.AttendanceStatusName = "待上课";

                        // 3. 更新student_course_arrange表 某时间段内排课数+1，剩余待上课数目+1
                        string studentCode = course.StudentCode;
                        string dayCode = course.CourseWeekDay;
                        string periodName = course.CoursePeriod;

                        var sca = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                         && s.CourseWeekDay == dayCode
                                                                        && s.CoursePeriod == periodName)
                                                                .FirstOrDefault();
                        sca.CourseTotalCount += 1;
                        sca.CourseRestCount += 1;

                        // 4. 更新student_course_package 可供排课的课时数 -1
                        string packageCode = course.PackageCode;
                        StudentCoursePackage scp = context.StudentCoursePackage.Where( s => s.StudentCode == studentCode
                                                                                            && s.PackageCode == packageCode)
                                                                                .FirstOrDefault();
                        scp.FlexCourseCount -= 1;
                        
                    }
                }
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                result = "500";
            }
            return result;
        }
    }
}