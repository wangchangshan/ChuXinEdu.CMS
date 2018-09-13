using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("student_artwork")]
    public class StudentArtwork
    {
        [Key]
        [Column("artwork_id")]
        public int ArtworkId { get; set; }
        
        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }
        
        [Column("artwork_title")]
        public string ArtworkTitle { get; set; }
        
        [Column("artwork_label")]
        public string ArtworkLabel { get; set; }
        
        [Column("artwork_cost_course_count")]
        public int ArtworkCostCourseCount { get; set; }
        
        [Column("document_type")]
        public string DocumentType { get; set; }
        
        [Column("document_width_height")]
        public string DocumentWidthHeight { get; set; }
        
        [Column("document_size")]
        public string DocumentSize { get; set; }
        
        [Column("document_path")]
        public string DocumentPath { get; set; }
        
        [Column("student_course_id")]
        public int StudentCourseId { get; set; }
        
        [Column("artwork_remark")]
        public string ArtworkRemark { get; set; }

        [Column("artwork_status")]
        public string ArtworkStatus { get; set; }
        
        [Column("create_date")]
        public DateTime CreateDate { get; set; }
    }
}