using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_course_arrange_template_detail")]
    public class SysCourseArrangeTemplateDetail
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("arrange_template_code")]
        public string ArrangeTemplateCode { get; set; }

        [Column("course_period")]
        public string CoursePeriod { get; set; }

        [Column("CourseWeekDay")]
        public string CourseWeekDay { get; set; }
    }
}