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

        // [排课模块] 删除学生排课 单节
        string SingleRemoveCourse(int studentCourseId);
    }
}