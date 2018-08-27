using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Context;

namespace ChuXinEdu.CMS.Server.BLLService
{
    public class ChuXinQuery : IChuXinQuery
    {
        public IEnumerable<Student> GetAllStudents()
        {
            using (BaseContext context = new BaseContext())
			{
				return context.Student.ToList();
			}
        }

        public Student GetStudentByCode(string studentCode)
        {
            var aa = MyDbContext.GetDataTable("select * from student");
            using (BaseContext context = new BaseContext())
            {
                //var bb = context.Database.SqlQuery<Student>("select * from student");
                //var cc = BaseContext.Student.FromSql("");
                return context.Student.Where(s => s.StudentCode == studentCode).FirstOrDefault();

            }

        }
    }
}