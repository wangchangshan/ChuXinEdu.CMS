using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChuXinEdu.CMS.Server.Model
{
    public class Simplify_StudentCourse
    {
        public string StudentCode { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

    }
}