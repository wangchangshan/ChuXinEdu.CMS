using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_back_fee")]
    public class StudentBackFee
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

        [Column("course_rest_count")]
        public int CourseRestCount { get; set; }

        [Column("back_fee_amount")]
        public decimal BackFeeAmount { get; set; }

        [Column("refund_person_code")]
        public string RefundPersonCode { get; set; }

        [Column("refund_person_name")]
        public string RefundPersonName { get; set; }

        [Column("refund_date")]
        public DateTime RefundDate { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }        
    }
}