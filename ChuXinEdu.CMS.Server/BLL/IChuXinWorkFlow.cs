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
    public interface IChuXinWorkFlow
    {
        // [排课模块] 批量学生排课
        string BatchStudentsCourseArrange(CA_C_STUDENTS_MAIN caInfo);

        // [排课模块] 学生个人请假
        string SingleQingJia(int studentCourseId);

        // [排课模块] 撤销个人请假
        string RestoreSingleQingJia(int studentCourseId);

        // [排课模块] 删除学生排课 单节
        string SingleRemoveCourse(int studentCourseId);

        // [排课模块] 添加放假安排
        string AddHoliday(SysHoliday holiday);

        // [排课模块] 删除放假日期
        string RemoveHoliday(string strDay);

        // [课程签到]
        string SignInSingle(CL_U_SIGN_IN course);

        // [课程签到]
        string SignInBatch(List<CL_U_SIGN_IN> courseList);

        // [作品补充上传]
        string SupplementArtWork(CL_U_SIGN_IN course);

        // [上传作品]
        int UploadArtWork(StudentArtwork artwork);

        string UploadAvatar(string studentCode, string path);

        // [删除临时作品]
        string RemoveTempArtWork(int courseId, string uid, string rootPath);

         // [添加学生选课套餐]
        string AddStudentCoursePackage(StudentCoursePackage scp);

        // [删除学生选课套餐]
        string RemoveStudentCoursePackage(int studentCoursePackageId);

        // [更新学生选课套餐]  
        string UpdateStudentCoursePackage(int id, StudentCoursePackage package);

        // [更新学生]  
        string UpdateStudentBaseInfo(string studentCode, Student student);
    }
}