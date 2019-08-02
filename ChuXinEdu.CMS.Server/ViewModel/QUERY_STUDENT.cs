using System;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    public class QUERY_STUDENT
    {
        public string studentCode { get; set; }

        public string studentName { get; set; }

        public string studentStatus { get; set; }

        public DateTime? startRegisterDate { get;set; }

        public DateTime? endRegisterDate { get; set; }

    }
}