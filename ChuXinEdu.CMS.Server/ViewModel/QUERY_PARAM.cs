using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //
    public class QUERY_PARAM
    {

        public string name { get; set; }

        public DateTime? startDate { get;set; }

        public DateTime? endDate { get; set; }

    }
}