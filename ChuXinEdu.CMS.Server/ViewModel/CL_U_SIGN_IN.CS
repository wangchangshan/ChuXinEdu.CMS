using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    // course list 学生签到
    public class CL_U_SIGN_IN
    {
        public int CourseListId{ get; set; }

        public DateTime CourseDate{ get; set; }

        public string StudentCode{ get; set; }

        public string TeacherCode { get; set; }

        public string TeacherName { get; set; }

        public List<string> FileUIds { get; set;}

        public int CostCount { get; set; } 

        public string Title { get; set; }
    }
}