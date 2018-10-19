using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //course read 学生课程信息总览
    public class STUDENT_R_COURSE_OVERVIEW
    {
        [Key]
        [Column("course_category_code")]
        public string CourseCategoryCode { get; set; }

        [Column("course_category_name")]
        public string CourseCategoryName { get; set; }

        [Column("total_course_count")]
        public int TotalCourseCount { get; set; }

        [Column("total_rest_course_count")]
        public int TotalRestCourseCount { get; set; }

        [Column("total_tuition")]
        public decimal TotalTuition { get; set; }
    }
}
