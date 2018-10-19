using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_course_package")]
    public class StudentCoursePackage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("package_code")]
        public string PackageCode { get; set; }

        [Column("package_name")]
        public string PackageName { get; set; }

        [Column("course_category_code")]
        public string CourseCategoryCode { get; set; }

        [Column("course_category_name")]
        public string CourseCategoryName { get; set; }

        [Column("course_folder_code")]
        public string CourseFolderCode { get; set; }

        [Column("course_folder_name")]
        public string CourseFolderName { get; set; }

        [Column("package_course_count")]
        public int PackageCourseCount { get; set; }       

        [Column("actual_course_count")]
        public int ActualCourseCount { get; set; }   

        [Column("flex_course_count")]
        public int FlexCourseCount { get; set; }

        [Column("package_price")]
        public decimal PackagePrice { get; set; }

        [Column("actual_price")]
        public decimal ActualPrice { get; set; }

        [Column("is_discount")]
        public string IsDiscount { get; set; }

        [Column("is_payed")]
        public string IsPayed { get; set; }

        [Column("payee_code")]
        public string PayeeCode { get; set; }

        [Column("payee_name")]
        public string PayeeName { get; set; }

        [Column("pay_pattern_code")]
        public string PayPatternCode { get; set; }

        [Column("pay_pattern_name")]
        public string PayPatternName { get; set; }

        [Column("pay_date")]
        public DateTime PayDate { get; set; }        

        [Column("rest_course_count")]
        public int RestCourseCount { get; set; }

        [Column("scp_status")]
        public string ScpStatus { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}