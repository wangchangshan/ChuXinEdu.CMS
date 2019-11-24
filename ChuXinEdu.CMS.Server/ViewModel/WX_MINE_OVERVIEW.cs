
using System;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_MINE_OVERVIEW
    {
        public int? tStudentCount { get; set; }

        public int? tTodayCourseCount { get; set; }

        public int? tStudentBirthCount { get; set; }

        public int? tExpirationCount { get; set; }

        public string studentName { get; set; }

        public string studentAvatarPath { get; set; }
        public DateTime? studentBirthday { get; set; }
        public string studentPhone { get; set; }
        public string studentSex { get; set; }
        public string studentAddress { get; set; }

        public int? studentArtworkCount { get; set; }

        // string 存储 json格式
        public string studentCourseOverview { get; set; }
    }
}