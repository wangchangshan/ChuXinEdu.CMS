using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_activity")]
    public class SysActivity
    {
        [Key]
        [Column("activity_id")]
        public int ActivityId { get; set; }

        [Column("activity_subject")]
        public string ActivitySubject { get; set; }

        [Column("activity_from_date")]
        public DateTime ActivityFromDate { get; set; }

        [Column("activity_to_date")]
        public DateTime ActivityToDate { get; set; }

        [Column("activity_course_count")]
        public int ActivityCourseCount { get; set; }

        [Column("activity_child_price")]
        public decimal ActivityChildPrice { get; set; }

        [Column("activity_adult_price")]
        public decimal ActivityAdultPrice { get; set; }

        [Column("activity_address")]
        public string ActivityAddress { get; set; }

        [Column("activity_content")]
        public string ActivityContent { get; set; }
    }
}