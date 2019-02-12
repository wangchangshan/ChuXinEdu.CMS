using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_course_comment")]
    public class StudentCourseComment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }
        
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("course_date")]
        public DateTime CourseDate { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("teacher_name")]
        public string TeacherName { get; set; }

        [Column("create_time")]
        public DateTime? CreateTime { get; set; }
    }
}