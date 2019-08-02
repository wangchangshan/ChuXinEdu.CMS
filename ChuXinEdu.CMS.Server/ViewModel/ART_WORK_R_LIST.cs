using System;

namespace ChuXinEdu.CMS.Server.ViewModel
{
    // 作品列表
    public class ART_WORK_R_LIST
    {
        public int ArtworkId { get; set; }

        public string ArtworkTitle{ get; set; }

        public int ArtworkCostCourseCount { get; set; }

        public string DocumentSize { get; set; }

        public string DocumentPath { get; set; }
        
        public DateTime FinishDate { get; set; }

        public string ShowURL { get; set; }
    }
}