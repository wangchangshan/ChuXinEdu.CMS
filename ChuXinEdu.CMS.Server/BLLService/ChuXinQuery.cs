using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
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
            using (BaseContext context = new BaseContext())
            {
                return context.Student.Where(s => s.StudentCode == studentCode).FirstOrDefault();
            }            
        }
    }
}