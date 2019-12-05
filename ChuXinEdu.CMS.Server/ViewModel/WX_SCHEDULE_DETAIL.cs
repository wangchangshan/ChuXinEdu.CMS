namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_SCHEDULE_DETAIL
    {
        // 学员主键
        public int id { get; set; }

        public string studentCode { get; set; }

        public string studentName { get; set; }

        public string studentAvatarPath { get; set; }

        public string courseWeekDay { get; set; }

        public string attendanceCode { get; set; }

        public string attendanceName { get; set; }
    }
}
