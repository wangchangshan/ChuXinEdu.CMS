using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_SCHEDULE_ROOM
    {
        // 班级
        public string classroom { get; set; }

        public virtual IEnumerable<WX_SCHEDULE_DETAIL> scheduleDetail { get; set; }
    }
}
