using System;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_SIGNIN_LIST
    {
        // 年月
        public DateTime? courseDate { get; set; }

        public string coursePeriod { get; set; }

        public string courseWeekday { get; set; }

        public virtual IEnumerable<StudentCourseList> signCourses { get; set; }
    }
}
