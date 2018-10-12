using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class DIC_R_PACKAGE
    {   
        public string Value { get; set; }

        public string Label { get; set; }

        public virtual IEnumerable<DIC_R_KEY_VALUE> children { get;set; }
    }
}
