using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("teacher_role")]
    public class TeacherRole
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("role_code")]
        public string RoleCode { get; set; }

        [Column("role_level")]
        public string RoleLevel { get; set; }
    }
}