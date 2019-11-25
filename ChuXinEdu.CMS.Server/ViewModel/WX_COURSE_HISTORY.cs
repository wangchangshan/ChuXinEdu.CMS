using System;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_COURSE_HISTORY
    {
        // 年月
        public string yyyymm { get; set; }

        public virtual IEnumerable<WX_COURSE_HISTORY_MM> courses { get; set; }

    }
}

/*  format
[
    {
        yyyymm: '2019-06',
        courses: [
            {
                courseDay: '09号',
                courseWeekday: '',
                courseSubject: '',
                coursePeriod: '',
                courseArtworks: [
                    {
                        artworkUrl: ''
                    }
                ]
            }
        ]
    }
]
*/
