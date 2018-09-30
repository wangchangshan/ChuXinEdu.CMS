using System;
using System.Data;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Context;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class TableCodeHelper
    {
        public static string GenerateStudentCode()
        {
            string result = string.Empty;
            string filter = DateTime.Now.ToString("yyyyMM");
            using (BaseContext context = new BaseContext())
            {
                DataTable dt = ADOContext.GetDataTable("select max(student_code) from student where student_code like '%"+ filter +"%' ");
                if(dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    int sequence = 0;
                    string curCode = dt.Rows[0][0].ToString();
                    string[] arr = curCode.Split("-");
                    if(arr.Length > 1)
                    {
                        sequence = Int32.Parse(arr[1].Substring(6, 3));
                        sequence ++;
                        result = arr[0] + "-" + filter + sequence.ToString("000");
                    }
                }
                else
                {
                    result = "BJ-" + filter + "001";
                }
            }

            return result;
        }
    }
}