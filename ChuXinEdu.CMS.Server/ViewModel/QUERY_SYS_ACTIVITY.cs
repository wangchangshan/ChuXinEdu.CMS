using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class QUERY_SYS_ACTIVITY
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }
        
        public string activitySubject { get; set; }
    }
}