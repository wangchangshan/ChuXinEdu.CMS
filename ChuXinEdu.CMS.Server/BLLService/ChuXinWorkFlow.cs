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
                result = "500";
            }

            return result;
        }
    }
}