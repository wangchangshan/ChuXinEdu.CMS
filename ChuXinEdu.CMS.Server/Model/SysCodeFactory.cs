using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("sys_code_factory")]
    public class SysCodeFactory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("table_name")]
        public string TableName { get; set; }

        [Column("column_name")]
        public string ColumnName { get; set; }

        [Column("prefix")]
        public string Prefix { get; set; }

        [Column("sequence_length")]
        public int SequenceLength { get; set; }

        [Column("current_num")]
        public int CurrentNum { get; set; }
        
    }
}