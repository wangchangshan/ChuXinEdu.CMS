using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class DIC_R_KEY_VALUE
    {   
        [Key]
        [Column("item_code")]
        public string Value { get; set; }

        [Column("item_name")]
        public string Label { get; set; }
    }
}
