using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.Context;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class TableCodeHelper
    {
        public static string GenerateCode(string tableName, string columnName)
        {
            string result = string.Empty;
            string perfix = string.Empty;
            int length = 3;
            switch (tableName.ToLower())
            {
                case "student":
                {
                    perfix = "BJ-" + DateTime.Now.ToString("yyyyMM");
                    length = 3;
                    break;
                }
                case "teacher":
                {
                    perfix = "T-";
                    length = 6;
                    break;
                }
                case "sys_course_package":
                {
                    perfix = "P-";
                    length = 6;
                    break;
                }
            }

            using (BaseContext context = new BaseContext())
            {
                var codeFactory = context.SysCodeFactory.Where(f => f.TableName == tableName
                                                    && f.ColumnName == columnName
                                                    && f.Prefix == perfix)
                                        .FirstOrDefault();

                if(codeFactory != null)
                {
                    int nextNum =  ++ codeFactory.CurrentNum;
                    result = perfix + nextNum.ToString().PadLeft(length, '0');

                    codeFactory.CurrentNum += 1;
                    context.SaveChanges();
                }
                else
                {
                    SysCodeFactory factory = new SysCodeFactory {
                        TableName = tableName,
                        ColumnName = columnName,
                        Prefix = perfix,
                        SequenceLength = length,
                        CurrentNum = 1
                    };
                    context.SysCodeFactory.Add(factory);
                    context.SaveChanges();

                    result = perfix + "1".PadLeft(length, '0');
                }
            }
            return result;
        }       
    }
}