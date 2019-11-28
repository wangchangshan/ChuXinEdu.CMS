using System;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class WX_ARTWORK_HISTORY
    {
        // 年月
        public string yyyymm { get; set; }

        public virtual IEnumerable<WX_ARTWORK_HISTORY_DETAIL> artworks { get; set; }

    }
}

/*  format
[
    {
        yyyymm: '2019-06',
        artworks: [
            {
                artworkTitle: '',
                artworkUrl: ''
            }
        ]
    }
]
*/
