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

        Student GetStudentBaseByCode(string sutdentCode);

        IEnumerable<Simplify_StudentCourse> GetAllStudentsCourse();


        //  获取每天上课时间段
        IEnumerable<SysCourseArrangeTemplateDetail> GetCourseArrangePeriod(string templateCode);

         // 获取学生选课信息（包含每个时间段）
        IEnumerable<StudentCourseArrange> GetStudentCourseArrage(string templateCode, string roomCode);

        // 获取时间段内排课信息
        IEnumerable<StudentCourseArrange> GetArrangedByPeriod(string templateCode, string roomCode, string dayCode, string periodName);

        // 获取待选课学生列表
        IEnumerable<StudentCoursePackage> GetStudentToSelectCourse(string dayCode, string periodName);

        // 获取待试听学生列表
        IEnumerable<StudentTemp> GetTempStudentToSelectCourse();

        // 获取学生排课列表
        IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod);
 
        // 获取假期列表
        IEnumerable<SysHoliday> GetHolidayList();




        // 这是一个测试
        StudentDescTest GetStudentDescTest(string studentCode);

    }
}