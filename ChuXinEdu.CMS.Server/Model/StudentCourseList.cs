using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_course_list")]
    public class StudentCourseList
    {
        [Key]
        [Column("student_course_id")]
        public int StudentCourseId { get; set; }

        [Column("arrange_template_code")]
        public string ArrangeTemplateCode { get; set; }

        [Column("classroom")]
        public string Classroom { get; set; }

        [Column("course_period")]
        public string CoursePeriod { get; set; }

        [Column("course_week_day")]
        public string CourseWeekDay { get; set; }

        [Column("course_date")]
        public DateTime CourseDate { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("teacher_name")]
        public string TeacherName { get; set; }

        [Column("package_code")]
        public string PackageCode { get; set; }

        [Column("course_category")]
        public string CourseCategory { get; set; }

        [Column("course_folder")]
        public string CourseFolder { get; set; }

        [Column("course_subject")]
        public string CourseSubject { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; }
        
        [Column("activity_id")]
        public int ActivityId { get; set; }
        
        [Column("attendance_status")]
        public string AttendanceStatus { get; set; }
    }
}