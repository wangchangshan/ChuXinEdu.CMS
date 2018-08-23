using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("teacher")]
    public class Teacher
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("teacher_name")]
        public string TeacherName { get; set; }

        [Column("teacher_sex")]
        public string TeacherSex { get; set; }

        [Column("teacher_birthday")]
        public DateTime TeacherBirthday { get; set; }

        [Column("teacher_identity_card_num")]
        public string TeacherIdentityCardNum { get; set; }

        [Column("teacher_phone")]
        public string TeacherPhone { get; set; }

        [Column("teacher_register_date")]
        public DateTime TeacherRegisterDate { get; set; }

        [Column("teacher_address")]
        public string TeacherAddress { get; set; }

        [Column("teacher_avatar_path")]
        public string TeacherAvatarPath { get; set; }

        [Column("teacher_status")]
        public string TeacherStatus { get; set; }

        [Column("teacher_remark")]
        public string TeacherRemark { get; set; }
    }
}