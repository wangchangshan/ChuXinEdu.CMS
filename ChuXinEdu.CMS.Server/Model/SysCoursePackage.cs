using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_course_package")]
    public class SysCoursePackage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("package_code")]
        public string PackageCode { get; set; }

        [Column("package_name")]
        public string PackageName { get; set; }

        [Column("package_course_category_code")]
        public string PackageCourseCategoryCode { get; set; }

        [Column("package_course_category_name")]
        public string PackageCourseCategoryName { get; set; }

        [Column("package_course_count")]
        public int PackageCourseCount { get; set; }

        [Column("package_price")]
        public decimal StudentIdentityCardNum { get; set; }

        [Column("package_enabled")]
        public string PackageEnabled { get; set; }

        [Column("package_create_time")]
        public DateTime PackageCreateTime { get; set; }
    }
}