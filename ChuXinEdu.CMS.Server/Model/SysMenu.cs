using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_menu")]
    public class SysMenu
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("path")]
        public string Path { get; set; }

        [Column("component")]
        public string Component { get; set; }

        [Column("icon")]
        public string Icon { get; set; }

        [Column("is_folder")]
        public string IsFolder { get; set; }

        [Column("hidden")]
        public bool Hidden { get; set; }

        [Column("is_enable")]
        public string IsEnable { get; set; }

        [Column("meta")]
        public string Meta { get; set; }

        [Column("roles")]
        public string Roles { get; set; }

        [Column("sort_weight")]
        public string SortWeight { get; set; }
    }
}