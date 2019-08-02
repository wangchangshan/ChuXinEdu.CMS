using System;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    //
    public class CA_C_STUDENTS_DETAIL
    {
        public int StudentCoursePackageId { get; set; }

        public string StudentCode { get; set; }

        public string StudentName { get; set; }

        public string PackageCode { get; set; }

        public string CourseCategoryCode { get; set; }

        public string CourseCategoryName { get; set; }

        public string CourseFolderCode { get; set; }

        public string CourseFolderName { get; set; }

        public int CourseCount { get; set; }

        public DateTime StartDate { get;set; }
    }
}