using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.ViewModel;
using Microsoft.Extensions.Logging;

namespace ChuXinEdu.CMS.Server.BLLService
{
    public class ChuXinWorkFlow : IChuXinWorkFlow
    {
        private readonly ILogger<ChuXinWorkFlow> _logger;
        public ChuXinWorkFlow(ILogger<ChuXinWorkFlow> logger)
        {
            _logger = logger;
        }

        #region 登陆模块
        public string LoginVerify(string loginCode, string pwd)
        {
            string result = "";
            using (BaseContext context = new BaseContext())
            {
                var sysUser = context.SysUser.Where(u => u.LoginCode == loginCode
                                                && u.Pwd == pwd)
                                            .FirstOrDefault();
                if (sysUser == null)
                {
                    result = "1101";
                }
                else
                {
                    result = "1200";
                    sysUser.LastLoginTime = DateTime.Now;
                    context.SaveChanges();
                }
            }
            return result;
        }

        public string LogOut(string loginCode)
        {
            string result = "";
            using (BaseContext context = new BaseContext())
            {
                var sysUser = context.SysUser.Where(u => u.LoginCode == loginCode)
                                            .FirstOrDefault();
                if (sysUser == null)
                {
                    result = "1401";
                }
                else
                {
                    result = "1200";
                    sysUser.LastLoginTime = DateTime.Now;
                    sysUser.Token = "";
                    context.SaveChanges();
                }
            }
            return result;
        }

        public string SaveUserLoginInfo(string loginCode, string ip, string signToken)
        {
            string result = "1200";
            try
            {
                string strExpireMinu = CustomConfig.GetSetting("UserExpireTime");
                if (string.IsNullOrEmpty(strExpireMinu))
                {
                    strExpireMinu = "60";
                }
                using (BaseContext context = new BaseContext())
                {
                    int expireMinu = Int32.Parse(strExpireMinu);
                    var sysUser = context.SysUser.Where(u => u.LoginCode == loginCode).First();
                    sysUser.Token = signToken;
                    sysUser.TokenExpireTime = DateTime.Now.AddMinutes(expireMinu);

                    context.SysLoginHistory.Add(new SysLoginHistory{
                        LoginCode = loginCode,
                        LoginIp = ip,
                        LoginTime = DateTime.Now
                    });
                    
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = "1500";
                _logger.LogError(ex, "保存token出错");
            }
            return result;
        }

        #endregion 登陆模块
        // 排课
        public string BatchStudentsCourseArrange(CA_C_STUDENTS_MAIN caInfo)
        {
            string result = "1200";
            _logger.LogInformation("批量排课开始");
            try
            {
                string statusCode = "09";
                string statusName = "待上课";
                string courseType = caInfo.CourseType;

                using (BaseContext context = new BaseContext())
                {
                    foreach (var student in caInfo.StudentList)
                    {
                        // 1. 向student_course_arrange表中插入数据
                        context.StudentCourseArrange.Add(new StudentCourseArrange
                        {
                            StudentCoursePackageId = student.StudentCoursePackageId,
                            ArrangeTemplateCode = caInfo.TemplateCode,
                            Classroom = caInfo.RoomCode,
                            CoursePeriod = caInfo.PeriodName,
                            CourseWeekDay = caInfo.DayCode,
                            StudentCode = student.StudentCode,
                            StudentName = student.StudentName,
                            PackageCode = student.PackageCode,
                            CourseTotalCount = student.CourseCount,
                            CourseRestCount = student.CourseCount,
                            CourseType = courseType
                        });
                        _logger.LogInformation("为{0}[{1}]向student_course_arrange中插入 1 条记录. [CourseTotalCount: {2}, CourseRestCount: {3}]", student.StudentName, student.StudentCode, student.CourseCount, student.CourseCount);

                        // 2. 向student_course_list表中插入数据（每节课一条记录）
                        DateTime firstCourseDate = student.StartDate.Date;
                        List<SysHoliday> holidays = context.SysHoliday.Where(h => h.HolidayDate >= firstCourseDate).ToList();
                        int coverHolidayCount = 0;
                        int courseCount = student.CourseCount;
                        for (int i = 0; i < courseCount; i++)
                        {
                            DateTime courseDate = firstCourseDate.AddDays((i + coverHolidayCount) * 7);

                            // 统一放假的日期不排课
                            while (!DateHelper.IsAvailableCourseDate(holidays, courseDate))
                            {
                                coverHolidayCount++;
                                courseDate = firstCourseDate.AddDays((i + coverHolidayCount) * 7);
                            }

                            context.StudentCourseList.Add(new StudentCourseList
                            {
                                StudentCoursePackageId = student.StudentCoursePackageId,
                                ArrangeTemplateCode = caInfo.TemplateCode,
                                Classroom = caInfo.RoomCode,
                                CoursePeriod = caInfo.PeriodName,
                                CourseWeekDay = caInfo.DayCode,
                                CourseDate = courseDate,
                                StudentCode = student.StudentCode,
                                StudentName = student.StudentName,
                                PackageCode = student.PackageCode,
                                CourseCategoryCode = student.CourseCategoryCode,
                                CourseCategoryName = student.CourseCategoryName,
                                CourseFolderCode = student.CourseFolderCode,
                                CourseFolderName = student.CourseFolderName,
                                CourseType = courseType,
                                AttendanceStatusCode = statusCode,
                                AttendanceStatusName = statusName
                            });
                        }

                        _logger.LogInformation("为{0}[{1}]向student_course_list中插入 {2} 条记录", student.StudentName, student.StudentCode, courseCount);
                        if (courseType == "正式")
                        {
                            // 3.1 更新student_course_package表中的flex_course_count字段 (如果一个学生续费相同套餐，那么应该等)
                            var scp = context.StudentCoursePackage.Where(s => s.Id == student.StudentCoursePackageId)
                                                                   .FirstOrDefault();
                            scp.FlexCourseCount = scp.FlexCourseCount - courseCount;
                            _logger.LogInformation("{0}[{1}]student_course_package中未排课数变更为：{2}", student.StudentName, student.StudentCode, scp.FlexCourseCount);
                        }
                        else // 试听
                        {
                            // 3.2 更新student_temp 试听学生表
                            var st = context.StudentTemp.Where(s => s.StudentCode == student.StudentCode
                                                                    && s.StudentTempStatus == "00").FirstOrDefault();
                            if (st != null)
                            {
                                st.StudentTempStatus = "01"; // 修改状态为已安排试听
                                st.TrialFolderCode = student.CourseFolderCode;
                                st.TrialFolderName = student.CourseFolderName;
                            }
                            else
                            {
                                // 3.3 正式学生试听其他课程。
                                var realStudent = context.Student.Where(s => s.StudentCode == student.StudentCode
                                                                           && s.TrialOtherCourse == "是")
                                                                .FirstOrDefault();
                                if (realStudent != null)
                                {
                                    realStudent.TrialOtherCourse = "否"; // 已经排课，将试听标志修改为否
                                }
                            }
                        }
                        // 4. 提交事务
                        context.SaveChanges();
                    }
                    _logger.LogInformation("批量排课结束");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "排课错误, 批量排课撤销");
                result = "1500";
            }

            return result;
        }

        // 说明：试听学生没有请假入口，只有正式学生可以请假
        public string SingleQingJia(int studentCourseId)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    // 1. 更新学生课程表
                    var studentCourse = context.StudentCourseList.First(s => s.StudentCourseId == studentCourseId && s.AttendanceStatusCode == "09");

                    studentCourse.AttendanceStatusCode = "00";
                    studentCourse.AttendanceStatusName = "个人请假";

                    // 2. 更新学生排课表 课程 减 1
                    int studentCoursePackageId = studentCourse.StudentCoursePackageId;
                    string studentCode = studentCourse.StudentCode;
                    string templateCode = studentCourse.ArrangeTemplateCode;
                    string packageCode = studentCourse.PackageCode;
                    string dayCode = studentCourse.CourseWeekDay;
                    string coursePeriod = studentCourse.CoursePeriod;
                    string roomCode = studentCourse.Classroom;


                    _logger.LogInformation("个人请假开始，信息：[姓名：{0}， 时间：{1} {2}]", studentCourse.StudentName, studentCourse.CourseDate, coursePeriod);
                    var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                && s.StudentCode == studentCode
                                                                                && s.ArrangeTemplateCode == templateCode
                                                                                && s.PackageCode == packageCode
                                                                                && s.CourseWeekDay == dayCode
                                                                                && s.Classroom == roomCode
                                                                                && s.CoursePeriod == coursePeriod)
                                                                        .FirstOrDefault();
                    if (courseArrange == null)
                    {
                        _logger.LogWarning("没有找到当前课程的arrange信息！！！！！！");
                        return "1409";
                    }

                    _logger.LogInformation("课程arrange中的排课课程数为：{0}", courseArrange.CourseTotalCount);
                    // 如果学生当前时时间段没有排课，则删除student_course_arrange表中的记录
                    if (courseArrange.CourseTotalCount <= 1)
                    {
                        context.Remove(courseArrange);
                        _logger.LogInformation("已经删除当前课程所在arrange信息");
                    }
                    else
                    {
                        courseArrange.CourseTotalCount -= 1;
                        courseArrange.CourseRestCount -= 1;
                        _logger.LogInformation("课程arrange中的排课信息变更为：[CourseTotalCount: {0}, CourseRestCount: {1}]", courseArrange.CourseTotalCount, courseArrange.CourseRestCount);
                    }

                    // 3. 更新学生套餐表， 套餐内未排课课时数 加 1
                    var studentCoursePackage = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                            .FirstOrDefault();
                    studentCoursePackage.FlexCourseCount += 1;
                    _logger.LogInformation("学生套餐表package的待排课数变更为：{0}", studentCoursePackage.FlexCourseCount);

                    // 4. 提交事务
                    context.SaveChanges();
                    _logger.LogInformation("个人请假结束");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "请假失败！");
                result = "1500";
            }
            return result;
        }

        // 说明：试听学生没有请假入口，只有正式学生可以请假
        public string RestoreSingleQingJia(int studentCourseId)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    // 1. 获取当前课程的相关信息
                    var studentCourse = context.StudentCourseList.First(s => s.StudentCourseId == studentCourseId);

                    int studentCoursePackageId = studentCourse.StudentCoursePackageId;
                    string studentCode = studentCourse.StudentCode;
                    string dayCode = studentCourse.CourseWeekDay;
                    string coursePeriod = studentCourse.CoursePeriod;
                    DateTime theDay = studentCourse.CourseDate;

                    // 2. 判断当前时间段是否重新安排了当前学生的课程（当全部删除当前学生的课程，再重新添加课程时，会出现这种情况）
                    var studentCourseOther = context.StudentCourseList.Where(s => s.StudentCode == studentCode
                                                                                    && s.CourseWeekDay == dayCode
                                                                                    && s.CoursePeriod == coursePeriod
                                                                                    && s.CourseDate == theDay
                                                                                    && s.AttendanceStatusCode == "09")
                                                                        .FirstOrDefault();
                    if (studentCourseOther != null)
                    {
                        // 3.1 当前时间重新安排了课程，则直接删除当前请假的课程
                        context.Remove(studentCourse);
                        _logger.LogInformation("当前时间重新安排了课程[{0} {1} {2}]，直接删除当前请假的课程!!", studentCourse.StudentName, theDay, coursePeriod);
                    }
                    else
                    {
                        // 3.2 没有重新安排课程
                        // 4. 判断是否重新安排完成了所有课程                        
                        string packageCode = studentCourse.PackageCode;
                        var studentCoursePackage = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                            .First();
                        if (studentCoursePackage.FlexCourseCount == 0)
                        {
                            // 4.1 重新安排完成了所有课程，则直接删除当前请假的课程
                            context.Remove(studentCourse);
                            _logger.LogInformation("{0}的当前套餐已经重新全部安排完成package.flexcoursecoun=1, 所以删除当前请假课程[ {1} {2}]!!", studentCourse.StudentName, theDay, coursePeriod);

                        }
                        else
                        {
                            // 4.2. 更新学生套餐表， 套餐内未排课课时数 -1
                            studentCoursePackage.FlexCourseCount -= 1;

                            // 5. 修改请假为 待上课
                            studentCourse.AttendanceStatusCode = "09";
                            studentCourse.AttendanceStatusName = "待上课";

                            // 6. 更新学生排课表student_course_arrange 课程 +1
                            string templateCode = studentCourse.ArrangeTemplateCode;
                            string roomCode = studentCourse.Classroom;
                            var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                    && s.StudentCode == studentCode
                                                                                    && s.ArrangeTemplateCode == templateCode
                                                                                    && s.PackageCode == packageCode
                                                                                    && s.CourseWeekDay == dayCode
                                                                                    && s.Classroom == roomCode
                                                                                    && s.CoursePeriod == coursePeriod)
                                                                            .FirstOrDefault();
                            if (courseArrange != null)
                            {
                                // 6.1 有记录
                                courseArrange.CourseTotalCount += 1;
                                courseArrange.CourseRestCount += 1;
                            }
                            else
                            {
                                // 6.2 没有记录 插入数据 （说明: 当学生在该时间段的所有课程全部请假时，出现这种情况）
                                context.StudentCourseArrange.Add(new StudentCourseArrange
                                {
                                    StudentCoursePackageId = studentCoursePackageId,
                                    ArrangeTemplateCode = templateCode,
                                    Classroom = roomCode,
                                    CoursePeriod = coursePeriod,
                                    CourseWeekDay = dayCode,
                                    StudentCode = studentCode,
                                    StudentName = studentCourse.StudentName,
                                    PackageCode = packageCode,
                                    CourseTotalCount = 1,
                                    CourseRestCount = 1,
                                    CourseType = "正式"
                                });
                            }
                        }
                    }

                    // 7. 提交事务
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "撤销请假失败！");
                result = "1500";
            }
            return result;
        }

        public string SingleRemoveCourse(int studentCourseId)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {

                    // 1. 删除学生课程表内排课记录
                    var studentCourse = context.StudentCourseList.FirstOrDefault(s => s.StudentCourseId == studentCourseId);

                    // 2. 处理学生排课表  student_course_arrange
                    int studentCoursePackageId = studentCourse.StudentCoursePackageId;
                    string studentCode = studentCourse.StudentCode;
                    string templateCode = studentCourse.ArrangeTemplateCode;
                    string packageCode = studentCourse.PackageCode;
                    string dayCode = studentCourse.CourseWeekDay;
                    string coursePeriod = studentCourse.CoursePeriod;
                    string roomCode = studentCourse.Classroom;

                    string logFullTime = studentCourse.CourseDate + " [" + coursePeriod + "]";
                    _logger.LogInformation("删除单节排课课程开始：{0}[{1}] 时间：{2} ", studentCourse.StudentName, studentCourse.StudentCode, logFullTime);
                    if (studentCourse.CourseType == "试听")
                    {
                        // 2.1 删除排课表
                        var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                                    && s.ArrangeTemplateCode == templateCode
                                                                                    && s.PackageCode == packageCode
                                                                                    && s.CourseWeekDay == dayCode
                                                                                    && s.Classroom == roomCode
                                                                                    && s.CoursePeriod == coursePeriod
                                                                                    && s.CourseType == "试听")
                                                                            .FirstOrDefault();
                        context.Remove(courseArrange);

                        // 2.1.1 更新试听学生表
                        var st = context.StudentTemp.Where(s => s.StudentCode == studentCode
                                                                && s.StudentTempStatus == "01").FirstOrDefault();
                        if (st != null)
                        {
                            st.StudentTempStatus = "00";  // 修改状态为待安排试听
                            st.TrialFolderCode = "";
                            st.TrialFolderName = "";
                        }
                        else
                        {
                            // 2.1.2 正式学生试听其他课程。
                            var realStudent = context.Student.Where(s => s.StudentCode == studentCode
                                                                       && s.TrialOtherCourse == "否")
                                                            .FirstOrDefault();
                            if (realStudent != null)
                            {
                                realStudent.TrialOtherCourse = "是";
                            }
                        }
                    }
                    else //正式
                    {
                        // 2.2 更新排课表
                        var courseArrange = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                    && s.ArrangeTemplateCode == templateCode
                                                                                    && s.PackageCode == packageCode
                                                                                    && s.CourseWeekDay == dayCode
                                                                                    && s.Classroom == roomCode
                                                                                    && s.CoursePeriod == coursePeriod
                                                                                    && s.CourseType == "正式")
                                                                            .FirstOrDefault();
                        if (courseArrange != null)
                        {
                            // 如果学生这个时间段只排了当前一节课，则删除student_course_arrange表中的记录
                            _logger.LogInformation("时间：{0}， {1}[{2}]安排的课程数为：{3}", logFullTime, studentCourse.StudentName, studentCode, courseArrange.CourseTotalCount);
                            if (courseArrange.CourseTotalCount <= 1)
                            {
                                context.Remove(courseArrange);
                                _logger.LogInformation("时间：{0}，{1}[{2}]只安排了一节课,删除student_course_arrange表中的记录！", logFullTime, studentCourse.StudentName, studentCode);
                            }
                            else
                            {
                                courseArrange.CourseTotalCount -= 1;
                                courseArrange.CourseRestCount -= 1;
                                _logger.LogInformation("时间：{0}，{1}[{2}]student_course_arrange变更为：[CourseTotalCount :{3}, CourseRestCount: {4}]", logFullTime, studentCourse.StudentName, studentCode, courseArrange.CourseTotalCount, courseArrange.CourseRestCount);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("找不到StudentCourseArrange中对应当前课程的排课记录！！！！！！！！！！！");
                            return "1409";
                        }


                        // 3. 更新学生套餐表， 套餐内未排课课时数 加 1
                        var studentCoursePackage = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                                .First();
                        studentCoursePackage.FlexCourseCount += 1;
                        _logger.LogInformation("{1}[{2}]课程套餐表studentCoursePackage剩余未排课记录变更为[FlexCourseCount: {3}]", studentCourse.StudentName, studentCode, studentCoursePackage.FlexCourseCount);
                    }


                    context.StudentCourseList.Remove(studentCourse);
                    // 4. 提交事务
                    context.SaveChanges();
                    _logger.LogInformation("删除单节排课记录结束：{0}[{1}] 时间：{2} ", studentCourse.StudentName, studentCourse.StudentCode, logFullTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除单节排课记录错误");
                result = "1500";
            }
            return result;
        }

        public string AddHoliday(SysHoliday holiday)
        {
            string result = "1200";
            try
            {
                _logger.LogInformation("添加假期开始");
                using (BaseContext context = new BaseContext())
                {
                    // 1. 向sys_holiday表中插入数据
                    context.SysHoliday.Add(holiday);

                    // 2. 更新student_course_list表 修改相关课程状态为统一放假
                    List<StudentCourseList> studentcourseList = context.StudentCourseList.Where(s => s.CourseDate == holiday.HolidayDate
                                                                                                    && s.AttendanceStatusCode == "09")
                                                                                        .ToList();
                    foreach (var course in studentcourseList)
                    {
                        course.AttendanceStatusCode = "03";
                        course.AttendanceStatusName = "统一放假";

                        _logger.LogInformation("假期影响学生：{0} [{1} {2}]", course.StudentName, course.CourseDate, course.CoursePeriod);
                        // 3. 试听与正式分开处理
                        string studentCode = course.StudentCode;
                        if (course.CourseType == "试听")
                        {
                            // 3.1 移除 student_course_arrange中的记录
                            var sca = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode
                                                                                && s.CourseType == "试听")
                                                                    .First();
                            context.Remove(sca);

                            // 3.2 更新 student_temp中的记录 修改状态为待排课
                            var st = context.StudentTemp.Where(s => s.StudentCode == studentCode
                                                                    && s.StudentTempStatus == "01").FirstOrDefault();
                            if (st != null)
                            {
                                st.StudentTempStatus = "00";  //修改状态为 待安排试听
                                st.TrialFolderCode = "";
                                st.TrialFolderName = "";
                            }
                            else
                            {
                                // 3.3 正式学生试听其他课程。
                                var realStudent = context.Student.Where(s => s.StudentCode == studentCode
                                                                           && s.TrialOtherCourse == "否")
                                                                .FirstOrDefault();
                                if (realStudent != null)
                                {
                                    realStudent.TrialOtherCourse = "是";
                                }
                            }
                        }
                        else //正式
                        {
                            int studentCoursePackageId = course.StudentCoursePackageId;
                            string dayCode = course.CourseWeekDay;
                            string periodName = course.CoursePeriod;

                            // 3.1 更新student_course_arrange表 (由于一个时间段内的学生是唯一的 跟模板和套餐没有关系，所以根据学生编码，星期几，时间段过滤即可)
                            StudentCourseArrange sca = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                                && s.StudentCode == studentCode
                                                                                                && s.CourseWeekDay == dayCode
                                                                                                && s.CoursePeriod == periodName)
                                                                                    .FirstOrDefault();
                            // 如果学生当前时时间段没有排课，则删除student_course_arrange表中的记录
                            if (sca.CourseTotalCount <= 1)
                            {
                                context.Remove(sca);
                            }
                            else
                            {
                                sca.CourseTotalCount -= 1;
                                sca.CourseRestCount -= 1;
                            }

                            // 3.2 更新student_course_package表 修改剩余选课课时数目
                            string packageCode = course.PackageCode;
                            StudentCoursePackage scp = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                                    .First();
                            scp.FlexCourseCount += 1;
                        }
                    }

                    // 5. 提交事物
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加假期失败！");
                result = "1500";
            }

            return result;
        }

        public string RemoveHoliday(string strDay)
        {
            // 说明： 在一个时间段，一个学生只能排课一次，否则涉及放假逻辑需要修改
            string result = "1200";
            try
            {
                DateTime theDay = DateTime.Parse(strDay);
                using (BaseContext context = new BaseContext())
                {
                    // 1. 删除sys_holiday表中的假期记录
                    var sysHoliday = context.SysHoliday.Where(h => h.HolidayDate == theDay)
                                                        .FirstOrDefault();
                    context.Remove(sysHoliday);

                    // 2. 更新student_course_list表， 修改状态为待上课
                    var scl = context.StudentCourseList.Where(s => s.CourseDate == theDay
                                                                   && s.AttendanceStatusCode == "03")
                                                        .ToList();
                    foreach (var course in scl)
                    {
                        course.AttendanceStatusCode = "09";
                        course.AttendanceStatusName = "待上课";

                        // 3. 判断是试听或者正式
                        if (course.CourseType == "试听")
                        {
                            // 判断该学生是否在其他时间段安排了试听
                            var sca = context.StudentCourseArrange.Where(s => s.StudentCode == course.StudentCode
                                                                                && s.CourseType == "试听")
                                                                    .FirstOrDefault();
                            if (sca == null)
                            {
                                // 3.1 向排课表student_course_arrange中插入一条数据
                                context.StudentCourseArrange.Add(new StudentCourseArrange
                                {
                                    StudentCoursePackageId = 0,
                                    ArrangeTemplateCode = course.ArrangeTemplateCode,
                                    Classroom = course.Classroom,
                                    CoursePeriod = course.CoursePeriod,
                                    CourseWeekDay = course.CourseWeekDay,
                                    StudentCode = course.StudentCode,
                                    StudentName = course.StudentName,
                                    PackageCode = course.PackageCode,
                                    CourseTotalCount = 1,
                                    CourseRestCount = 1,
                                    CourseType = "试听"
                                });
                            }
                            else
                            {
                                context.Remove(course);
                            }

                            // 3.2 更新试听学生表 student_temp
                            var st = context.StudentTemp.Where(s => s.StudentCode == course.StudentCode
                                                                    && s.StudentTempStatus == "00").FirstOrDefault();
                            if (st != null)
                            {
                                st.StudentTempStatus = "01";
                                st.TrialFolderCode = course.CourseFolderCode;
                                st.TrialFolderName = course.CourseFolderName;
                            }
                            else
                            {
                                // 3.3 正式学生试听其他课程。
                                var realStudent = context.Student.Where(s => s.StudentCode == course.StudentCode
                                                                           && s.TrialOtherCourse == "是")
                                                                .FirstOrDefault();
                                if (realStudent != null)
                                {
                                    realStudent.TrialOtherCourse = "否";
                                }
                            }
                        }
                        else  // 正式
                        {
                            // 3.1 判断是否重新排完了所有课程
                            int studentCoursePackageId = course.StudentCoursePackageId;
                            string packageCode = course.PackageCode;
                            string studentCode = course.StudentCode;
                            StudentCoursePackage scp = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                                    .First();
                            if (scp.FlexCourseCount == 0)
                            {
                                // 3.1.1 重新安排完了；删除当前课程
                                context.Remove(course);
                                continue;
                            }
                            else
                            {
                                // 3.1.2 更新student_course_package 可供排课的课时数 -1
                                scp.FlexCourseCount -= 1;
                            }

                            // 3.2. 更新student_course_arrange表 某时间段内排课数+1，剩余待上课数目+1
                            string dayCode = course.CourseWeekDay;
                            string periodName = course.CoursePeriod;

                            var sca = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                && s.StudentCode == studentCode
                                                                                && s.CourseWeekDay == dayCode
                                                                                && s.CoursePeriod == periodName)
                                                                    .FirstOrDefault();
                            if (sca != null)
                            {
                                sca.CourseTotalCount += 1;
                                sca.CourseRestCount += 1;
                            }
                            else
                            {
                                // 插入数据至student_course_arrange表
                                context.StudentCourseArrange.Add(new StudentCourseArrange
                                {
                                    StudentCoursePackageId = studentCoursePackageId,
                                    ArrangeTemplateCode = course.ArrangeTemplateCode,
                                    Classroom = course.Classroom,
                                    CoursePeriod = course.CoursePeriod,
                                    CourseWeekDay = course.CourseWeekDay,
                                    StudentCode = course.StudentCode,
                                    StudentName = course.StudentName,
                                    PackageCode = course.PackageCode,
                                    CourseTotalCount = 1,
                                    CourseRestCount = 1,
                                    CourseType = "正式"
                                });
                            }
                        }
                    }
                    // 4. 提交事务
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除假期失败");
                result = "1500";
            }
            return result;
        }

        public string SignInSingle(CL_U_SIGN_IN course)
        {
            string result = "1200";
            try
            {
                int courseId = course.CourseListId;
                using (BaseContext context = new BaseContext())
                {
                    _logger.LogInformation("单节销课开始");
                    string studentCode = course.StudentCode;
                    var scl = context.StudentCourseList.Where(s => s.StudentCourseId == courseId && s.AttendanceStatusCode == "09").FirstOrDefault();
                    if (scl == null)
                    {
                        _logger.LogInformation("没有找到当前待销课课程!!!!!!!!!! return， courseId: {0}", courseId);
                        return "1409";
                    }
                    // 1. 更新课程记录表的状态
                    scl.AttendanceStatusCode = "01";
                    scl.AttendanceStatusName = "上课销课";
                    scl.TeacherCode = course.TeacherCode;
                    scl.TeacherName = course.TeacherName;
                    scl.CourseSubject = course.Title;
                    scl.CourseFolderCode = course.CourseFolderCode;
                    scl.CourseFolderName = course.CourseFolderName;

                    // 2. 判断是否为试听
                    if (scl.CourseType == "试听")
                    {
                        // 2.1.1 更新试听学员表状态
                        var st = context.StudentTemp.Where(s => s.StudentCode == studentCode
                                                                && s.StudentTempStatus == "01").FirstOrDefault();
                        if (st != null)
                        {
                            st.StudentTempStatus = "02"; // 试听结束
                        }

                        // 2.1.2 删除课程安排表试听信息
                        var sca = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode && s.CourseType == "试听").First();
                        context.Remove(sca);
                    }
                    else  //正式
                    {
                        // 2.2.1 课程安排表信息                        
                        int studentCoursePackageId = scl.StudentCoursePackageId;
                        string templateCode = scl.ArrangeTemplateCode;
                        string packageCode = scl.PackageCode;
                        string dayCode = scl.CourseWeekDay;
                        string periodName = scl.CoursePeriod;
                        var sca = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                            && s.ArrangeTemplateCode == templateCode
                                                                            && s.StudentCode == studentCode
                                                                            && s.PackageCode == packageCode
                                                                            && s.CourseWeekDay == dayCode
                                                                            && s.CoursePeriod == periodName)
                                                                .FirstOrDefault();
                        if (sca == null)
                        {
                            _logger.LogWarning("没有找到当前课程的arrange信息！！！！！  return. ");
                            return "1409";
                        }
                        _logger.LogInformation("当前课程所在arrange剩余课程数为：{0}", sca.CourseRestCount);
                        if (sca.CourseRestCount <= 1)
                        {
                            context.Remove(sca);
                            _logger.LogInformation("删除当前课程所在arrange");
                        }
                        else
                        {
                            sca.CourseRestCount -= 1;
                            _logger.LogInformation("变更当前课程所在arrange：[CourseRestCount: {0}]", sca.CourseRestCount);
                        }

                        // 2.2.2 更新学生套餐表
                        var scp = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                .First();

                        _logger.LogInformation("当前课程所在package 剩余课程数为：{0}", scp.RestCourseCount);
                        if (scp.RestCourseCount == 1)
                        {
                            // 2.2.3 如果是套餐的最后一节课， 判断该学生是否还有其他没有完成的套餐
                            var otherNormalScpCount = context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                                   && s.ScpStatus == "00"
                                                                                   && s.Id != studentCoursePackageId)
                                                                                .Count();
                            if (otherNormalScpCount == 0)
                            {
                                // 2.2.4 没有其他上课的套餐， 修改学生状态为 ‘03 结束未续费’
                                var student = context.Student.Where(s => s.StudentCode == studentCode).First();
                                student.StudentStatus = "03";
                                _logger.LogInformation("当前是最后一节课，没有其他上课的套餐，修改学生[{0}]状态为 ‘03 结束未续费", scp.StudentName);
                            }

                            // 2.2.5 修改当前学生套餐状态为 01已完成
                            scp.ScpStatus = "01";
                        }
                        scp.RestCourseCount -= 1;
                        _logger.LogInformation("变更当前课程所在package 剩余课程数为：{0}", scp.RestCourseCount);
                    }

                    // 3. 更新作品表 
                    if (course.FileUIds != null)
                    {
                        DateTime finishDay = scl.CourseDate;
                        List<string> uids = course.FileUIds;
                        foreach (string uid in uids)
                        {
                            var artWork = context.StudentArtwork.Where(s => s.StudentCourseId == courseId
                                                                            && s.TempUId == uid
                                                                            && s.ArtworkStatus == "00")
                                                                .First();
                            artWork.ArtworkTitle = course.Title;
                            artWork.ArtworkCostCourseCount = course.CostCount;
                            artWork.FinishDate = finishDay;
                            artWork.ArtworkStatus = "01";
                        }
                    }

                    // 4. 提交事务
                    context.SaveChanges();
                    _logger.LogInformation("单节课程签到结束。");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "单节销课错误! 课程ID：{0}", course.CourseListId);
                result = "1500";
            }
            return result;
        }

        public string SignInBatch(List<CL_U_SIGN_IN> courseList)
        {
            string result = "1200";

            _logger.LogInformation("批量销课开始");
            foreach (var item in courseList)
            {
                using (BaseContext context = new BaseContext())
                {
                    try
                    {
                        string studentCode = item.StudentCode;
                        var scl = context.StudentCourseList.Where(s => s.StudentCourseId == item.CourseListId && s.AttendanceStatusCode == "09").FirstOrDefault();

                        if (scl == null)
                        {
                            _logger.LogWarning("找不到课程Id：{0}", item.CourseListId);
                            continue;
                        }

                        string logFullTime = scl.CourseDate + " [" + scl.CoursePeriod + "]";
                        _logger.LogInformation("开始销课，课程Id：{0}。{1},{2}", item.CourseListId, logFullTime, scl.StudentName);
                        // 1. 更新课程记录表的状态
                        scl.AttendanceStatusCode = "01";
                        scl.AttendanceStatusName = "上课销课";
                        scl.TeacherCode = item.TeacherCode;
                        scl.TeacherName = item.TeacherName;
                        scl.CourseSubject = item.Title;
                        scl.CourseFolderCode = item.CourseFolderCode;
                        scl.CourseFolderName = item.CourseFolderName;

                        // 2. 判断是否为试听
                        if (scl.CourseType == "试听")
                        {
                            // 2.1.1 更新试听学员表状态
                            var st = context.StudentTemp.Where(s => s.StudentCode == studentCode
                                                                    && s.StudentTempStatus == "01").FirstOrDefault();
                            if (st != null)
                            {
                                st.StudentTempStatus = "02"; // 试听结束
                            }

                            // 2.1.2 删除课程安排表试听信息
                            var sca = context.StudentCourseArrange.Where(s => s.StudentCode == studentCode && s.CourseType == "试听").First();
                            context.Remove(sca);
                        }
                        else  //正式
                        {
                            // 2.2.1 课程安排表信息
                            int studentCoursePackageId = scl.StudentCoursePackageId;
                            string templateCode = scl.ArrangeTemplateCode;
                            string packageCode = scl.PackageCode;
                            string dayCode = scl.CourseWeekDay;
                            string periodName = scl.CoursePeriod;
                            var sca = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId
                                                                                && s.ArrangeTemplateCode == templateCode
                                                                                && s.StudentCode == studentCode
                                                                                && s.PackageCode == packageCode
                                                                                && s.CourseWeekDay == dayCode
                                                                                && s.CoursePeriod == periodName)
                                                                    .FirstOrDefault();
                            if (sca == null)
                            {
                                _logger.LogWarning("找不到当前课程所在arrange, 课程ID:{0} 的课程不进行销课！！！", item.CourseListId);
                                continue;
                            }
                            _logger.LogInformation("当前课程所在arrange 的待上课课时数为：{0}", sca.CourseRestCount);
                            if (sca.CourseRestCount <= 1)
                            {
                                context.Remove(sca);
                                _logger.LogInformation("删除当前课程所在arrange");
                            }
                            else
                            {
                                sca.CourseRestCount -= 1;
                                _logger.LogInformation("修改当前课程所在arrange 的待上课课时数为：{0}", sca.CourseRestCount);
                            }

                            // 2.2.2 更新学生套餐表
                            var scp = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId)
                                                                .First();
                            if (scp.RestCourseCount == 1)
                            {
                                // 2.2.3 如果是套餐的最后一节课， 判断该学生是否还有其他没有完成的套餐
                                var otherNormalScpCount = context.StudentCoursePackage.Where(s => s.StudentCode == studentCode
                                                                                       && s.ScpStatus == "00"
                                                                                       && s.Id != studentCoursePackageId)
                                                                                    .Count();
                                if (otherNormalScpCount == 0)
                                {
                                    // 2.2.4 没有其他上课的套餐， 修改学生状态为 ‘03 结束未续费’
                                    var student = context.Student.Where(s => s.StudentCode == studentCode).First();
                                    student.StudentStatus = "03";
                                    _logger.LogInformation("当前是最后一节课，没有其他上课的套餐，修改学生[{0}]状态为 ‘03 结束未续费", scp.StudentName);
                                }

                                // 2.2.5 修改当前学生套餐状态为 01已完成 
                                scp.ScpStatus = "01";
                            }
                            scp.RestCourseCount -= 1;
                            _logger.LogInformation("变更当前课程所在package 剩余课程数为：{0}", scp.RestCourseCount);
                        }
                        // 3. 提交事务  需要放到foreach 代码块中，防止某次循环中断(比如：需要获取arrange的数据，但是arrange在上次循环应该被删除，但是由于没提交，所以没删除) 造成数据错乱。
                        context.SaveChanges();
                        _logger.LogInformation("销课结束，课程Id：{0}。{1},{2}", item.CourseListId, logFullTime, scl.StudentName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "销课出错，课程ID：{0}", item.CourseListId);
                        result = "1500"; // 部分错误
                    }
                }
                _logger.LogInformation("批量销课结束");
            }
            return result;
        }

        public string SupplementHistoryCourse(List<StudentCourseList> courseList)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    int i = 0;
                    int studentCoursePackageId = courseList[0].StudentCoursePackageId;
                    // 1. 向课程记录表中插入数据
                    foreach (var course in courseList)
                    {
                        context.StudentCourseList.Add(course);
                        i++;
                    }
                    // 2. 获取当前课程套餐
                    var package = context.StudentCoursePackage.Where(s => s.Id == studentCoursePackageId && s.FlexCourseCount > 0).FirstOrDefault();
                    if (package != null)
                    {
                        if (package.FlexCourseCount - i == 0)
                        {
                            // 2.1 套餐内可以排课的课程数目为 0，进一步判断是否所有课程都已结束
                            if (package.RestCourseCount - i == 0)
                            {
                                // 2.2 套餐内课程全部上完，标记当前课程套餐为结束
                                package.FlexCourseCount = 0;
                                package.RestCourseCount = 0;
                                package.ScpStatus = "01";
                            }
                            else if (package.RestCourseCount - i > 0)
                            {
                                package.FlexCourseCount -= i;
                                package.RestCourseCount -= i;
                            }
                            else
                            {
                                _logger.LogWarning("课程套餐数据异常！！套餐ID: {0}，剩余restCourseCount: {1}， 本次提交补录课程数目：{2}", studentCoursePackageId, package.RestCourseCount, i.ToString());
                                return "1409";
                            }
                        }
                        else if (package.FlexCourseCount - i > 0)
                        {
                            package.FlexCourseCount -= i;
                            package.RestCourseCount -= i;
                        }
                        else
                        {
                            _logger.LogWarning("课程套餐数据异常！！套餐ID: {0}，剩余FlexCourseCount: {1}， 本次提交补录课程数目：{2}", studentCoursePackageId, package.FlexCourseCount, i.ToString());
                            return "1409";
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "补录课程信息失败");
                result = "1500";
            }
            return result;
        }
        public string SupplementArtWork(CL_U_SIGN_IN course)
        {
            string result = "1200";
            try
            {
                int courseId = course.CourseListId;
                using (BaseContext context = new BaseContext())
                {
                    // 1. 更新作品表 
                    if (course.FileUIds != null)
                    {
                        List<string> uids = course.FileUIds;
                        foreach (string uid in uids)
                        {
                            var artWork = context.StudentArtwork.Where(s => s.StudentCourseId == courseId
                                                                            && s.TempUId == uid
                                                                            && s.ArtworkStatus == "00")
                                                                .First();
                            artWork.ArtworkTitle = course.Title;
                            artWork.FinishDate = course.CourseDate;
                            artWork.ArtworkCostCourseCount = course.CostCount;
                            artWork.ArtworkStatus = "01";
                        }

                        // 2. 提交事务
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新作品失败");
                result = "1500";
            }
            return result;
        }

        public string SupplementArtWork(string[] uids)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    foreach (string uid in uids)
                    {
                        var artWork = context.StudentArtwork.Where(s => s.TempUId == uid
                                                                        && s.ArtworkStatus == "00")
                                                            .First();
                        artWork.FinishDate = DateTime.Now;
                        artWork.ArtworkStatus = "01";
                    }

                    // 2. 提交事务
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量上传作品失败！");
                result = "1500";
            }
            return result;
        }

        public int UploadArtWork(StudentArtwork artwork)
        {
            int result = -2;
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.StudentArtwork.Add(artwork);
                    context.SaveChanges();
                    result = artwork.ArtworkId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上传作品失败");
            }
            return result;
        }

        public string UploadAvatar(string code, string path, string type)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    if (type == "student")
                    {
                        var st = context.Student.Where(s => s.StudentCode == code).First();
                        st.StudentAvatarPath = path;
                        context.SaveChanges();
                    }
                    else if (type == "teacher")
                    {
                        var teacher = context.Teacher.Where(t => t.TeacherCode == code).First();
                        teacher.TeacherAvatarPath = path;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上传头像");
                result = "1500";
            }
            return result;
        }

        public string RemoveTempArtWork(int courseId, string uid, string rootPath)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var artWork = context.StudentArtwork.Where(s => s.TempUId == uid
                                                                    && s.StudentCourseId == courseId
                                                                    && s.ArtworkStatus == "00")
                                                        .FirstOrDefault();
                    if (artWork != null)
                    {
                        string savePath = rootPath + artWork.DocumentPath;
                        if (System.IO.File.Exists(savePath))
                        {
                            System.IO.File.Delete(savePath);
                        }
                        context.Remove(artWork);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除临时作品");
                result = "1500";
            }
            return result;
        }

        public string RemoveArtWorkById(int id, string rootPath)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var artWork = context.StudentArtwork.Where(s => s.ArtworkId == id).FirstOrDefault();
                    if (artWork != null)
                    {
                        string savePath = rootPath + artWork.DocumentPath;
                        if (System.IO.File.Exists(savePath))
                        {
                            System.IO.File.Delete(savePath);
                        }
                        context.Remove(artWork);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除作品失败");
                result = "1500";
            }
            return result;
        }

        public string AddStudentCoursePackage(StudentCoursePackage scp)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.StudentCoursePackage.Add(scp);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加学生套餐失败");
                result = "1500";
            }
            return result;
        }

        public string RemoveStudentCoursePackage(int studentCoursePackageId)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var courseList = context.StudentCourseList.Where(c => c.StudentCoursePackageId == studentCoursePackageId
                                                        && c.AttendanceStatusCode == "01")
                                            .ToList();
                    if (courseList.Count > 0)
                    {
                        // 已经上课 不能删除
                        result = "1600";
                    }
                    else
                    {
                        var package = context.StudentCoursePackage.Where(p => p.Id == studentCoursePackageId).FirstOrDefault();
                        if (package != null)
                        {
                            context.StudentCoursePackage.Remove(package);
                        }

                        var sca = context.StudentCourseArrange.Where(s => s.StudentCoursePackageId == studentCoursePackageId).FirstOrDefault();
                        if (sca != null)
                        {
                            context.StudentCourseArrange.Remove(sca);
                        }

                        var scl = context.StudentCourseList.Where(s => s.StudentCoursePackageId == studentCoursePackageId).FirstOrDefault();
                        if (scl != null)
                        {
                            context.StudentCourseList.Remove(scl);
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除学生套餐失败");
                result = "1500";
            }
            return result;
        }
        public string UpdateStudentCoursePackage(int id, StudentCoursePackage package)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var scp = context.StudentCoursePackage.Where(s => s.Id == id).First();

                    scp.IsDiscount = package.IsDiscount;
                    scp.IsPayed = package.IsPayed;
                    scp.ActualPrice = package.ActualPrice;
                    scp.PayeeCode = package.PayeeCode;
                    scp.PayeeName = package.PayeeName;
                    scp.PayPatternCode = package.PayPatternCode;
                    scp.PayPatternName = package.PayPatternName;
                    scp.PayDate = package.PayDate;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新学生套餐失败");
                result = "1500";
            }
            return result;
        }

        public string AddStudentBaseInfo(Student student)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.Student.Add(student);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增学生失败");
                result = "1500";
            }
            return result;
        }

        public string AddTempStudent(StudentTemp student)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.StudentTemp.Add(student);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增临时学生失败");
                result = "1500";
            }
            return result;
        }

        public string UpdateStudentBaseInfo(string studentCode, Student student)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.Student.Where(st => st.StudentCode == studentCode).First();

                    s.StudentSex = student.StudentSex;
                    s.StudentBirthday = student.StudentBirthday;
                    s.StudentIdentityCardNum = student.StudentIdentityCardNum;
                    s.StudentPhone = student.StudentPhone;
                    s.StudentAddress = student.StudentAddress;
                    s.StudentRemark = student.StudentRemark;
                    s.StudentRegisterDate = student.StudentRegisterDate;
                    s.StudentStatus = student.StudentStatus;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新学生基本信息失败");
                result = "1500";
            }
            return result;
        }

        public string SetStudentFeeBack(string studentCode, List<StudentCoursePackage> packageList)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var student = context.Student.Where(st => st.StudentCode == studentCode).FirstOrDefault();
                    if (student != null)
                    {
                        student.StudentStatus = "02"; // 中途退费
                    }

                    foreach (var package in packageList)
                    {
                        var scp = context.StudentCoursePackage.Where(s => s.Id == package.Id
                                                                            && s.ScpStatus == "00")
                                                            .FirstOrDefault();
                        if (scp != null)
                        {
                            scp.ScpStatus = "02";
                            scp.FeeBackAmount = package.FeeBackAmount;
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "学生退费失败");
                result = "1500";
            }
            return result;
        }

        public string UpdateStudentTrialOtherCourse(string studentCode, string curVal)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.Student.Where(st => st.StudentCode == studentCode).FirstOrDefault();
                    if (s != null)
                    {
                        s.TrialOtherCourse = curVal;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新学生开启新试听失败");
                result = "1500";
            }
            return result;
        }

        public string UpdateTempStudent(int id, StudentTemp student)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.StudentTemp.Where(st => st.Id == id).First();

                    s.StudentName = student.StudentName;
                    s.StudentSex = student.StudentSex;
                    s.StudentBirthday = student.StudentBirthday;
                    s.StudentIdentityCardNum = student.StudentIdentityCardNum;
                    s.StudentPhone = student.StudentPhone;
                    s.StudentAddress = student.StudentAddress;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新临时学生信息失败");
                result = "1500";
            }
            return result;
        }

        public string TempStudentTrialFail(int id)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.StudentTemp.Where(st => st.Id == id).First();

                    s.Result = "失败";
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新学生试听结果失败");
                result = "1500";
            }
            return result;
        }

        public string TempStudentTrialSuccess(int id)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.StudentTemp.Where(st => st.Id == id).First();

                    s.Result = "成功";

                    context.Student.Add(new Student
                    {
                        StudentCode = s.StudentCode,
                        StudentName = s.StudentName,
                        StudentSex = s.StudentSex,
                        StudentBirthday = s.StudentBirthday,
                        StudentIdentityCardNum = s.StudentIdentityCardNum,
                        StudentPhone = s.StudentPhone,
                        StudentPropagateType = s.StudentPropagateType,
                        StudentPropagateTxt = s.StudentPropagateTxt,
                        StudentAddress = s.StudentAddress,
                        StudentAvatarPath = s.StudentAvatarPath,
                        StudentRegisterDate = DateTime.Now,
                        TrialOtherCourse = "否",
                        StudentStatus = "01"
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新学生试听结果失败");
                result = "1500";
            }
            return result;
        }

        public string RemoveTempStudent(int id)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var s = context.StudentTemp.Where(st => st.Id == id).First();
                    context.StudentTemp.Remove(s);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除临时学生失败");
                result = "1500";
            }
            return result;
        }

        #region sys course package
        public string AddSysCoursePackage(SysCoursePackage newPackage)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.SysCoursePackage.Add(newPackage);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加系统套餐失败");
                result = "1500";
            }
            return result;
        }

        public string UpdateSysCoursePackage(int id, SysCoursePackage package)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var scp = context.SysCoursePackage.Where(s => s.Id == id).First();
                    scp.PackageName = package.PackageName;
                    scp.PackageCourseCategoryCode = package.PackageCourseCategoryCode;
                    scp.PackageCourseCategoryName = package.PackageCourseCategoryName;
                    scp.PackageCourseCount = package.PackageCourseCount;
                    scp.PackagePrice = package.PackagePrice;
                    scp.PackageEnabled = package.PackageEnabled;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新系统套餐失败");
                result = "1500";
            }
            return result;
        }

        public string RemoveSysCoursePackage(int id)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var scp = context.SysCoursePackage.Where(s => s.Id == id).First();
                    context.SysCoursePackage.Remove(scp);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除系统套餐失败");
                result = "1500";
            }
            return result;
        }
        #endregion

        #region teacher
        public string AddTeacher(Teacher teacher)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.Teacher.Add(teacher);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加教师失败");
                result = "1500";
            }
            return result;
        }

        public string UpdateTeacher(int id, Teacher teacher)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var th = context.Teacher.Where(t => t.Id == id).FirstOrDefault();
                    th.TeacherSex = teacher.TeacherSex;
                    th.TeacherBirthday = teacher.TeacherBirthday;
                    th.TeacherIdentityCardNum = teacher.TeacherIdentityCardNum;
                    th.TeacherPhone = teacher.TeacherPhone;
                    th.TeacherAddress = teacher.TeacherAddress;
                    th.TeacherRegisterDate = teacher.TeacherRegisterDate;
                    th.TeacherRemark = teacher.TeacherRemark;
                    th.TeacherStatus = teacher.TeacherStatus;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新教师失败");
                result = "1500";
            }
            return result;
        }

        #endregion

        public string AddNewRecommend(StudentRecommend srd)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    context.StudentRecommend.Add(srd);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加学生介绍信息失败");
                result = "1500";
            }
            return result;
        }

        public string RemoveStudentRecommend(int id)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var srd = context.StudentRecommend.Where(s => s.Id == id).FirstOrDefault();
                    if (srd != null)
                    {
                        context.StudentRecommend.Remove(srd);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除学生介绍信息失败");
                result = "1500";
            }
            return result;
        }

        public string AddTeacherRole(string roleCode, List<string> teacherCodes)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var role = context.SysDictionary.Where(d => d.TypeCode == "roles"
                                                                && d.ItemCode == roleCode
                                                                && d.ItemEnabled == "Y")
                                                    .First();
                    string roleDesc = role.ItemDesc;
                    foreach (var teacherCode in teacherCodes)
                    {
                        TeacherRole tr = new TeacherRole
                        {
                            TeacherCode = teacherCode,
                            RoleCode = roleCode,
                            RoleLevel = roleDesc
                        };
                        context.TeacherRole.Add(tr);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加角色失败");
                result = "1500";
            }
            return result;
        }

        public string RemoveTeacherRole(string roleCode, List<string> teacherCodes)
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    foreach (var teacherCode in teacherCodes)
                    {
                        var tr = context.TeacherRole.Where(r => r.TeacherCode == teacherCode && r.RoleCode == roleCode).FirstOrDefault();
                        if (tr != null)
                        {
                            context.TeacherRole.Remove(tr);
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除角色失败");
                result = "1500";
            }
            return result;
        }
    }
}