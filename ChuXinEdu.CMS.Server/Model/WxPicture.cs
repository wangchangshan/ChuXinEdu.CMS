using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    [Table("wx_picture")]
    public class WxPicture
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("subject")]
        public string subject { get; set; }

        [Column("student_code")]
        public string StudentCode { get; set; }

        [Column("student_name")]
        public string StudentName { get; set; }

        [Column("student_age")]
        public int? StudentAge { get; set; }

        [Column("teacher_code")]
        public string TeacherCode { get; set; }

        [Column("picture_path")]
        public string PicturePath { get; set; }

        [Column("rate_level")]
        public int RateLevel { get; set; } = 0;

        [Column("wx_picture_type")]
        public string WxPictureType { get; set; }
    }
}