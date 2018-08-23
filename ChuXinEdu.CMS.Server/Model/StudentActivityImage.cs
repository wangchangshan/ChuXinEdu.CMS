using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_activity_image")]
    public class StudentActivityImage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("activity_id")]
        public int ActivityId { get; set; }

        [Column("activity_image_name")]
        public string ActivityImageName { get; set; }

        [Column("activity_image_path")]
        public string ActivityImagePath { get; set; }
    
        [Column("activity_image_width_height")]
        public string ActivityImageWidthHeight { get; set; }

        [Column("activity_image_size")]
        public string ActivityImageSize { get; set; }

       [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}