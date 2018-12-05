using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_out_user")]
    public class SysOutUser
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_code")]
        public string UserCode { get; set; }

        [Column("user_key")]
        public string UserKey { get; set; }

        [Column("last_request_time")]
        public DateTime? LastRequestTime { get; set; }
    }
}