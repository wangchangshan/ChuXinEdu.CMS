using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_course_arrange_template")]
    public class SysCourseArrangeTemplate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("arrange_template_code")]
        public string ArrangeTemplateCode { get; set; }

        [Column("arrange_template_name")]
        public string ArrangeTemplateName { get; set; }

        [Column("template_enabled")]
        public string TemplateEnabled { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}