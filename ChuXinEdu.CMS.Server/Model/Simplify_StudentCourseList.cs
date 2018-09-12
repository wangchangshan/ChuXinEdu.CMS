using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    public class Simplify_StudentCourseList
    {
        [Key]
        [Column("student_course_id")]
        public int StudentCourseId { get; set; }

        [Column("course_date")]
        public DateTime CourseDate { get; set; }

        [Column("course_category_name")]
        public string CourseCategoryName { get; set; }        
        
        [Column("attendance_status_code")]
        public string AttendanceStatusCode { get; set; }

        [Column("attendance_status_name")]
        public string AttendanceStatusName { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; }
    }
}