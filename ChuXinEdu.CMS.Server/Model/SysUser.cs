using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_user")]
    public class SysUser
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("login_code")]
        public string LoginCode { get; set; }

        [Column("pwd")]
        public string Pwd { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("fail_count")]
        public int FailCount { get; set; }

        [Column("last_login_time")]
        public DateTime LastLoginTime { get; set; }
    }
}