using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_recommend")]
    public class StudentRecommend
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("origin_student_code")]
        public string OriginStudentCode { get; set; }

        [Column("origin_student_name")]
        public string OriginStudentName { get; set; }

        [Column("new_student_code")]
        public string NewStudentCode { get; set; }

        [Column("new_student_name")]
        public string NewStudentName { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }
    }
}