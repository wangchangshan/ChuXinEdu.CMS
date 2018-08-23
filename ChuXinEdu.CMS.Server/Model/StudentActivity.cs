using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_activity")]
    public class StudentActivity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("activity_id")]
        public int ActivityId { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }
    }
}