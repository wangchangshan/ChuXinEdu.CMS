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
        IEnumerable<DIC_R_KEY_VALUE> GetTeacherToCharge();

        IEnumerable<DIC_R_KEY_VALUE> GetAllActiveStudents();

        string GetRoles(string teacherCode);

        IEnumerable<SysDictionary> GetAllRoles();

        IEnumerable<SysMenu> GetSysMenus();

        IEnumerable<SysCourseArrangeTemplate> GetSysArrangeTemplates();

        IEnumerable<SysCourseArrangeTemplateDetail> GetArrangeTemplateDetail(string templateCode);

        IEnumerable<Student> GetStudentList(int pageIndex, int pageSize, QUERY_STUDENT query, out int totalCount);

        IEnumerable<Student> GetStudentList2Export(QUERY_STUDENT query);

        DataTable GetStudentForRecommend(string studentName);

        IEnumerable<StudentRecommend> GetRecommendStudentList(string studentCode);

        IEnumerable<StudentTemp> GetTempStudentList(int pageIndex, int pageSize, QUERY_STUDENT_TEMP query, out int totalCount);

        Student GetStudentByCode(string studentCode);

        DataTable GetStudentAuxiliaryInfo(string studentCode);
        
        IEnumerable<StudentCoursePackage> GetStudentCoursePackage(string studentCode);

        IEnumerable<DIC_R_KEY_VALUE> GetStudentPackageKV(string studentCode);

        IEnumerable<StudentCoursePackage> GetNoFinishPackage(string studentCode);

        IEnumerable<StudentCourseList> GetStudentCourseList(string studentCode);

        IEnumerable<StudentCourseList> GetStudentCourseList(int pageIndex, int pageSize, QUERY_STUDENT_COURSE_LIST query,out int totalCount);

        IEnumerable<StudentCourseList> GetStudentDayOffList(string studentCode);

        IEnumerable<StudentCourseComment> GetCourseComments(string studentCode);

        DataTable GetScpSimplify();

        DataTable GetStudentPayRank();

        //获取已经上课的数目
        //int GetStudentSignInCourseCount(string studentCode);


        //  获取每天上课时间段
        IEnumerable<SysCourseArrangeTemplateDetail> GetCourseArrangePeriod(string templateCode);

         // 获取学生选课信息（包含每个时间段）
        IEnumerable<CA_R_PERIOD_STUDENTS> GetAllPeriodStudents(string templateCode, string roomCode);

        // 获取时间段内排课信息
        IEnumerable<CA_R_PERIOD_STUDENTS> GetPeriodStudents(string templateCode, string roomCode, string dayCode, string periodName);

        List<StudentCourseList> GetCoursesToday();

        List<StudentCourseList> GetCoursesByday(DateTime theDay);

        // 获取待选课学生列表
        IEnumerable<StudentCoursePackage> GetStudentToSelectCourse(string dayCode, string periodName);

        // 获取待试听学生列表
        IEnumerable<StudentTemp> GetTempStudentToSelectCourse();

        // 获取学生排课列表
        IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod);

        DataTable GetBirthdayIn7Days();
 
        // 获取假期列表
        IEnumerable<SysHoliday> GetHolidayList();

        // 获取待签到课程列表
        IEnumerable<StudentCourseList> GetCoursesToSignIn();

        // 获取待签到课程数目
        int GetCoursesToSignInCount();

        // 获取课程套餐将要结束（5 节课）的学生列表
        DataTable GetCourseToFinishList();

        IEnumerable<StudentArtwork> GetArkworkByCourse(int courseId);

        IEnumerable<StudentArtwork> GetArkworkByStudent(string studentCode);

        string GetArtWorkTruePath(int artworkId);

        string GetAvatarTruePath(int id, string type);

        SysCoursePackage GetSysCoursePackage(string packageCode);

        IEnumerable<SysCoursePackage> GetSysCoursePackageList(QUERY_SYS_PACKAGE query);

        List<SysActivity> GetActivityList(QUERY_SYS_ACTIVITY query, out int totalCount);

        SysActivity GetActivityById(int activityId);

        List<StudentActivity> GetStudentByActivity(int activityId);

        bool isPackageUsed(int packageId);

        IEnumerable<Teacher> GetTeacherList(QUERY_TEACHER query);

        DataTable GetAllCourseRoleTeachers();

        DataTable GetTeacherListWithRole(string roleCode);

        Teacher GetTeacher(string teacherCode);

        string getTeacherCodeByName(string teacherName);

        IEnumerable<StudentCourseList> GetTeacherCourseList(string teacherCode, int pageIndex, int pageSize, QUERY_TEACHER_COURSE query, out int totalCount);

        IEnumerable<StudentCourseList> GetTeacherCourseList2Export(string teacherCode, QUERY_TEACHER_COURSE query);

        SysWxUser GetWxUserByOpenId(string openId);

        IEnumerable<WxPicture> GetWxPicture(string picTypeCode);

        IEnumerable<WxPicture> GetWxHomePicture();

        bool IsStudentExist(string studentCode, string studentName);

        string GetTeacherCodeByWxKey(string wxKey);

        string GetWeiXinPicTruePath(int id);

        // 这是一个测试
        StudentDescTest GetStudentDescTest(string studentCode);

    }
}