using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class CourseArrangeVM
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("course_period")]
        public string CoursePeriod { get; set; }

        [Column("course_week_day")]
        public string CourseWeekDay { get; set; }

        public int ThisWeekStudentCount {get; set;}

        public virtual IEnumerable<CA_R_PERIOD_STUDENTS> PeriodStudentList { get;set; }
    }
}