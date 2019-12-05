using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_SCHEDULE
    {
        public string coursePeriod { get; set; }

        public virtual IEnumerable<WX_SCHEDULE_ROOM> roomSchedule { get; set; }
    }
}
