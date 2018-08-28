using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class StudentDescTest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("student_status")]
        public string StudentStatus { get; set; }

        [Column("student_status_desc")]
        public string StudentStatusDesc { get; set; }
    }
}