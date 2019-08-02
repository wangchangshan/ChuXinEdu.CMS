using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //course arrage create stuedents  排课：新增学生实体。
    public class CA_C_STUDENTS_MAIN
    {
        public string TemplateCode{ get; set; }

        public string RoomCode { get; set; }

        public string DayCode { get; set; }

        public string PeriodName { get; set; }

         public string CourseType { get; set; }

        public virtual IEnumerable<CA_C_STUDENTS_DETAIL> StudentList { get;set; }
    }
}