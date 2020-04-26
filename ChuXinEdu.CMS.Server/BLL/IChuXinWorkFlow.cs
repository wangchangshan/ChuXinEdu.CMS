using System;
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
        // [登陆]
        string LoginVerify(string loginCode, string pwd, string ip, out string token, out string teacherCode);

        string PwdVerify(string loginCode, string pwd);

        // [修改密码]
        string ChangePassword(string loginCode, string newPwd);

        // [登出]
        string LogOut(string loginCode);

        // [登陆 签名]
        string SaveUserLoginInfo(string loginCode, string ip, string signToken);

        // [排课模块] 批量学生排课
        string BatchStudentsCourseArrange(CA_C_STUDENTS_MAIN caInfo);

        // [排课模块] 学生个人请假
        string SingleQingJia(int studentCourseId);

        // [排课模块] 撤销个人请假
        string RestoreSingleQingJia(int studentCourseId);

        // [排课模块] 删除学生排课 单节
        string SingleRemoveCourse(int studentCourseId);

        // [排课模板] 顺延所有排课
        string Postpone(string templateCode, DateTime startDay, DateTime endDay);

        // [排课模块] 添加放假安排
        string AddHoliday(SysHoliday holiday);

        // [排课模块] 删除放假日期
        string RemoveHoliday(string strDay);

        // [课程签到]
        string SignInSingle(CL_U_SIGN_IN course);

        // [课程签到]
        string SignInBatch(List<CL_U_SIGN_IN> courseList);

        // [课程补录]
        string SupplementHistoryCourse(List<StudentCourseList> courseList);

        // [作品补充上传]
        string SupplementArtWork(CL_U_SIGN_IN course);

        // [作品补充上传]
        string SupplementArtWork(string[] uids);

        // [上传作品]
        int UploadArtWork(StudentArtwork artwork);

        string UploadAvatar(string studentCode, string path, string type);

        // [上传微信小程序用到的图片]
        string UploadWxPicture(WxPicture picture);

        void SetWxPictureRateLevel(int id, int level);

        // [删除临时作品]
        string RemoveTempArtWork(int courseId, string uid, string rootPath);

        // [删除正式作品]
        string RemoveArtWorkById(int id, string rootPath);

        // [删除微信小程序图片]
        string RemoveWxPicture(int id, string rootPath);

        // [添加学生选课套餐]
        string AddStudentCoursePackage(StudentCoursePackage scp);

        // [添加新的推荐学员]
        string AddNewRecommend(StudentRecommend srd);

        // [删除学生推荐表中的数据]
        string RemoveStudentRecommend(int id);

        // [删除学生选课套餐]
        string RemoveStudentCoursePackage(int studentCoursePackageId);

        // [更新学生选课套餐]  
        string UpdateStudentCoursePackage(int id, StudentCoursePackage package);

        // [添加学生]  
        string AddStudentBaseInfo(Student student);

        // [添加临时试听学生]  
        string AddTempStudent(StudentTemp student);

        // [更新学生]  
        string UpdateStudentBaseInfo(string studentCode, Student student);

        // [正式学生打开新试听开关]
        string UpdateStudentTrialOtherCourse(string studentCode, string curVal);

        // [学生退费]
        string SetStudentFeeBack(string studentCode, List<StudentCoursePackage> packageList);

        // [学生套餐退费]
        string SetStudentPackageFeeBack(string studentCode, int packageId, List<StudentCoursePackage> packageList);

        // [更新试听学生]  
        string UpdateTempStudent(int id, StudentTemp student);

        // [删除试听学生]  
        string RemoveTempStudent(int id);

        // [试听学生 试听失败]  
        string TempStudentTrialFail(int id);

        // [试听学生 试听成功] 
        string TempStudentTrialSuccess(int id);

        // [添加系统套餐] 
        string AddSysCoursePackage(SysCoursePackage newPackage);

        // [更新系统套餐] 
        string UpdateSysCoursePackage(int id, SysCoursePackage package);

        // [删除系统套餐] 
        string RemoveSysCoursePackage(int id);

        // [添加课堂评语] 
        string AddCourseComment(StudentCourseComment newComment);

        // [更新课堂评语] 
        string UpdateCourseComment(int id, StudentCourseComment comment);

        // [删除课堂评语] 
        string RemoveCourseComment(int id);

        // [添加排课模板]
        string AddArrangeTemplate(string templateCode, string templateName, string templateEnabled, List<SysCourseArrangeTemplateDetail> details);

        // [更新排课模板]
        string UpdateArrangeTemplate(string templateCode, string templateName, string templateEnabled, List<SysCourseArrangeTemplateDetail> details);

        // [删除排课模板]
        string RemoveArrangeTemplate(string templateCode);

        // [添加活动]
        string AddNewActivity(SysActivity activity, out int id);

        // [添加活动]
        string UpdateActivity(int id, SysActivity activity);

        // [删除活动]
        string RemoveActivity(int id);

        // [保存活动学生]
        string SaveStudent2Activity(int id, List<StudentActivity> saList);

        // [添加教师]
        string AddTeacher(Teacher teacher);

        // [更新教师]
        string UpdateTeacher(int id, Teacher teacher);

        string UpdateTeacherWxkey(string teacherCode, string wxKey);

        // [为教师添加角色]
        string AddTeacherRole(string roleCode, List<string> teacherCodes);

        // [删除教师角色]
        string RemoveTeacherRole(string roleCode, List<string> teacherCodes);

        // [删除签到后的学生课程记录]
        string RemoveStudentCourse(int courseId);

        // [添加字典]
        string AddNewDic(List<SysDictionary> dicList);

        // [更新字典]
        string UpdateDic(List<SysDictionary> dicList);

        // [删除字典]
        string RemoveDic(string typeCode);

        // [插入微信登陆信息]
        string InsertWxLoginInfo(string openId, string sessionKey, string innerPCode, string innerPName, string wxKey, string type);

        string UpdateWxSKey(string openId, string sKey);

        // [脏数据处理]
        string ClearDirtyForPackage(int id);

        // [脏数据处理]
        string ClearDirtyForArrange(int id);
    }
}