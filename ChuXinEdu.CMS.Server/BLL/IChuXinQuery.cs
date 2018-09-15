using System;
using System.Data;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.BLL
{
    /// <summary>
	/// This interface will define all the query function, so the data query from UI must call this interface
	/// </summary>
    public interface IChuXinQuery
    {
        IEnumerable<Student> GetStudentList(int pageIndex, int pageSize);

        IEnumerable<Student> GetStudentsByName(string studentName);

        Student GetStudentByCode(string studentCode);

        IEnumerable<StudentCoursePackage> GetStudentCoursePackage(string studentCode);

        IEnumerable<StudentCourseList> GetStudentCourseList(string studentCode);

        IEnumerable<Simplify_StudentCourse> GetAllStudentsCourse();

        //获取已经上课的数目
        int GetStudentSignInCourseCount(string studentCode);


        //  获取每天上课时间段
        IEnumerable<SysCourseArrangeTemplateDetail> GetCourseArrangePeriod(string templateCode);

         // 获取学生选课信息（包含每个时间段）
        IEnumerable<CA_R_PERIOD_STUDENTS> GetAllPeriodStudents(string templateCode, string roomCode);

        // 获取时间段内排课信息
        IEnumerable<CA_R_PERIOD_STUDENTS> GetPeriodStudents(string templateCode, string roomCode, string dayCode, string periodName);

        // 获取待选课学生列表
        IEnumerable<StudentCoursePackage> GetStudentToSelectCourse(string dayCode, string periodName);

        // 获取待试听学生列表
        IEnumerable<StudentTemp> GetTempStudentToSelectCourse();

        // 获取学生排课列表
        IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod);
 
        // 获取假期列表
        IEnumerable<SysHoliday> GetHolidayList();

        // 获取待签到课程列表
        IEnumerable<StudentCourseList> GetCoursesToSignIn();




        // 这是一个测试
        StudentDescTest GetStudentDescTest(string studentCode);

    }
}