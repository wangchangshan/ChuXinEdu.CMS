using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_holiday")]
    public class SysHoliday
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("holiday_date")]
        public DateTime HolidayDate { get; set; }
    }
}