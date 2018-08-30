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
        IEnumerable<Student> GetStudentList();

        IEnumerable<Student> GetStudentsByName(string studentName);

        Student GetStudentBaseByCode(string sutdentCode);

        IEnumerable<Simplify_StudentCourse> GetAllStudentsCourse();

        // 这是一个测试
        StudentDescTest GetStudentDescTest(string studentCode);

    }
}