using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_course_arrange")]
    public class StudentCourseArrange
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("student_course_package_id")]
        public int StudentCoursePackageId { get; set; }

        [Column("arrange_guid")]
        public string ArrangeGuid { get; set; }

        [Column("arrange_template_code")]
        public string ArrangeTemplateCode { get; set; }

        [Column("classroom")]
        public string Classroom { get; set; }

        [Column("course_period")]
        public string CoursePeriod { get; set; }

        [Column("course_week_day")]
        public string CourseWeekDay { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("package_code")]
        public string PackageCode { get; set; }

        [Column("course_total_count")]
        public int CourseTotalCount { get; set; }

        [Column("course_rest_count")]
        public int CourseRestCount { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; }
    }
}