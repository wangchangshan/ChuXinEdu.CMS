using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_dictionary")]
    public class SysDictionary
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("type_code")]
        public string TypeCode { get; set; }

        [Column("type_name")]
        public string TypeName { get; set; }

        [Column("item_code")]
        public string ItemCode { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("item_desc")]
        public string ItemDesc { get; set; }

        [Column("is_parent")]
        public string IsParent { get; set; }

        [Column("parent_item_code")]
        public string ParentItemCode { get; set; }

        [Column("item_sort_weight")]
        public string ItemSortWeight { get; set; }

        [Column("item_enabled")]
        public string ItemEnabled { get; set; }
    }
}