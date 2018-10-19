using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //course read baseinfo  查看学生基础信息
    public class STUDENT_R_BASEINO
    {
        public Student StudentInfo{ get; set; }

        public virtual IEnumerable<STUDENT_R_COURSE_OVERVIEW> CourseOverview { get; set; }

        public virtual IEnumerable<StudentCoursePackage> CoursePackageList { get; set; }
    }
}
