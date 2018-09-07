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
                                                                .First();
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
                                                                        .First();
                    courseArrange.CourseTotalCount -= 1;
                    courseArrange.CourseRestCount -= 1;

                    // 3. 更新学生套餐表， 套餐内未排课课时数 加 1
                    var studentCoursePackage = context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                                        && s.PackageCode == packageCode)
                                                                            .First();
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
    }
}