using System;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_COURSE_HISTORY_MM
    {
        // 年月
        public string courseDay { get; set; }

        public string courseWeekday { get; set; }

        public string courseSubject { get; set; }

        public string coursePeriod { get; set; }

        public virtual IEnumerable<WX_COURSE_HISTORY_ARTWORK> courseArtworks { get; set; }

    }
}