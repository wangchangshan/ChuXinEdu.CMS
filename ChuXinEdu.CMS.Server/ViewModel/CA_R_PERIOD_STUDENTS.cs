using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    // 排课主界面 展示时间段内学生列表
    public class CA_R_PERIOD_STUDENTS
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

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

        [Column("course_category_code")]
        public string CourseCategoryCode { get; set; }

        [Column("course_category_name")]
        public string CourseCategoryName { get; set; }

        [Column("course_folder_code")]
        public string CourseFolderCode { get; set; }

        [Column("course_folder_name")]
        public string CourseFolderName { get; set; }

        [Column("course_total_count")]
        public int CourseTotalCount { get; set; }

        [Column("course_rest_count")]
        public int CourseRestCount { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; }

        [Column("is_this_week")]
        public string IsThisWeek { get; set; }
    }
}