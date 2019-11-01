using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_wx_user")]
    public class SysWxUser
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("open_id")]
        public string OpenId { get; set; }

        [Column("session_key")]
        public string SessionKey { get; set; }

        [Column("inner_person_code")]
        public string InnerPersonCode { get; set; }

        [Column("inner_person_name")]
        public string InnerPersonName { get; set; }

        [Column("wx_key")]
        public string WxKey { get; set; }

        [Column("wx_user_type")]
        public string wxUserType { get; set; }

        [Column("access_token")]
        public string AccessToken { get; set; }

        [Column("expires_in")]
        public int? ExpiresIn { get; set; }

        [Column("last_request_time")]
        public DateTime? LastRequestTime { get; set; }
    }
}