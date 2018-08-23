using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_finace")]
    public class SysFinace
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("item_subject")]
        public string ItemSubject { get; set; }

        [Column("item_remark")]
        public string ItemRemark { get; set; }

        [Column("item_type")]
        public string ItemType { get; set; }

        [Column("item_amount")]
        public decimal ItemAmount { get; set; }

        [Column("person_code")]
        public string PersonCode { get; set; }

        [Column("person_name")]
        public string PersonName { get; set; }

        [Column("happen_date")]
        public DateTime HappenDate { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }
    }
}