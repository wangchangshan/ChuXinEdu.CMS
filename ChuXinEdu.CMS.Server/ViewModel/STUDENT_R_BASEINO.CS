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
        public int TotalCourseCount { get; set; }

        public int RestCourseCount { get; set; }

        public decimal TotalTuition { get; set; }

        public Student StudentInfo{ get; set; }

        public virtual IEnumerable<StudentCoursePackage> CoursePackageList { get; set; }
    }
}
