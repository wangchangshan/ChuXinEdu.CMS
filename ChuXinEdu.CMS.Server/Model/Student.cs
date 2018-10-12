using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student")]
    public class Student
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("student_sex")]
        public string StudentSex { get; set; }

        [Column("student_birthday")]
        public DateTime? StudentBirthday { get; set; }

        [Column("student_identity_card_num")]
        public string StudentIdentityCardNum { get; set; }

        [Column("student_phone")]
        public string StudentPhone { get; set; }

        [Column("student_propagate_type")]
        public string StudentPropagateType { get; set; }

        [Column("student_propagate_txt")]
        public string StudentPropagateTxt { get; set; }

        [Column("student_register_date")]
        public DateTime StudentRegisterDate { get; set; }

        [Column("student_address")]
        public string StudentAddress { get; set; }

        [Column("student_avatar_path")]
        public string StudentAvatarPath { get; set; }

        [Column("trial_other_course")]
        public string TrialOtherCourse { get; set; }

        [Column("student_status")]
        public string StudentStatus { get; set; }

        [Column("student_remark")]
        public string StudentRemark { get; set; }
    }
}