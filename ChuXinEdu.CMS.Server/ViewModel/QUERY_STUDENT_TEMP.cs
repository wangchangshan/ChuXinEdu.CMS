using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //
    public class QUERY_STUDENT_TEMP
    {
        public string studentName { get; set; }

        public string[] studentTempStatus {get; set;}
    }
}