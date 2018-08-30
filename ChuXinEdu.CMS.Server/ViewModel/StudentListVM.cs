using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class StudentListVM
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
        public DateTime StudentBirthday { get; set; }

        [Column("student_identity_card_num")]
        public string StudentIdentityCardNum { get; set; }

        [Column("student_phone")]
        public string StudentPhone { get; set; }

        [Column("student_register_date")]
        public DateTime StudentRegisterDate { get; set; }

        [Column("student_address")]
        public string StudentAddress { get; set; }        

        [Column("student_status")]
        public string StudentStatus { get; set; }

        public virtual IEnumerable<Simplify_StudentCourse> CourseCategory { get;set; }
    }
}