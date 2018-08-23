using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_status_history")]
    public class StudentStatusHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("last_status")]
        public string LastStatus { get; set; }

        [Column("current_status")]
        public string CurrentStatus { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}