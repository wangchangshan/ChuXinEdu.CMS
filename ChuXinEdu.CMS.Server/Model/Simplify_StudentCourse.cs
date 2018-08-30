using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    public class Simplify_StudentCourse
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("course_category_code")]
        public string Code { get; set; }

        [Column("course_category_name")]
        public string Name { get; set; }

    }
}