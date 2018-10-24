using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class QUERY_SYS_PACKAGE
    {
        public string packageName { get; set; }

        public string packageEnabled { get; set; }
    }
}