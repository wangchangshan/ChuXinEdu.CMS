using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    //[MyAuthenFilter]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        public DownloadController(IChuXinQuery chuxinQuery, IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// [导出] 导出学生上课记录 GET api/download/studentcourse
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> StudentCourse(string studentCode)
        {
            string docPath = string.Empty;
            string fileName = studentCode + "_course_history.xlsx";

            DataTable dtCourse = ADOContext.GetDataTable($@"select package_name as '套餐名称',scl.course_date as '日期', scl.course_period as '时间',scl.course_folder_name as '课程类别',course_subject as '课程内容',teacher_name '上课教师' 
                                                                        from student_course_list scl
                                                                        left join student_course_package scp on scl.student_course_package_id = scp.id
                                                                        where scl.student_code='{studentCode}' and (attendance_status_code = '01' or attendance_status_code = '02') and course_type='正式' 
                                                                        order by student_course_package_id,course_period,course_date");

            docPath = GenerateExcel_StudentCourse(dtCourse, fileName);

            // Response...
            ContentDisposition cd = new ContentDisposition
            {
                Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
            };
           // Response.Headers.Add("Content-Disposition", cd.ToString());
            //Response.Headers.Add("X-Content-Type-Options", "nosniff");

            Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "test.xlsx"
            }.ToString();

            if (System.IO.File.Exists(docPath))
            {
                var stream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(docPath));
                byte[] result = System.IO.File.ReadAllBytes(docPath);
                return File(result, "application/ms-excel", "Employee.xlsx");
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(stream, "application/ms-excel", "Employee.xlsx");
                //return File(stream, "application/octet-stream");

                /*
                https://gist.github.com/javilobo8/097c30a233786be52070986d8cdb1743
                https://stackoverflow.com/questions/43136185/asp-net-core-return-excel-file-xlsx-directly-in-one-call-to-the-server-on-the?rq=1
                https://forums.asp.net/t/2123126.aspx?Download+file+using+ASP+NET+Core
                https://hk.saowen.com/a/5ccc9ba55e48329b63304c8096b563abb117cdfe62acedeb95ab2e85a490632b
                https://github.com/axios/axios/issues/1660
                https://github.com/kennethjiang/js-file-download

                 */
            }
            else
            {
                return NotFound();
            }
        }

        private string GenerateExcel_StudentCourse(DataTable dt, string fileName)
        {
            string path = _hostingEnvironment.ContentRootPath + "/cxdocs/temp/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            path = Path.Combine(path, fileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }

            DataTable dtPackages = dt.DefaultView.ToTable(true, "套餐名称");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (DataRow dr in dtPackages.Rows)
                {
                    // 构造当前套餐的数据源
                    DataTable dtCourse = dt.Clone();
                    DataRow[] drs = dt.Select("套餐名称 = '" + dr[0].ToString() + "'");
                    foreach (DataRow row in drs) 
                    {
                        dtCourse.Rows.Add(row.ItemArray);
                    }

                    var worksheet = package.Workbook.Worksheets.Add(dr[0].ToString());
                    worksheet.Cells["A1"].LoadFromDataTable(dtCourse, true);

                    //Format 标题列
                    using (ExcelRange rng = worksheet.Cells[1, 1, 1, dtCourse.Columns.Count])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Font.Size = 12;
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 246));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.FromArgb(51, 51, 51));
                    }                    

                    //Format 正文 从第二行开始
                    ExcelBorderStyle borderStyle = ExcelBorderStyle.Thin;
                    Color borderColor = Color.FromArgb(155, 155, 155);
                    using (ExcelRange rng = worksheet.Cells[2, 1, dtCourse.Rows.Count + 1, dtCourse.Columns.Count])
                    {
                        rng.Style.Font.Name = "宋体";
                        rng.Style.Font.Size = 10;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));

                        rng.Style.Border.Top.Style = borderStyle;
                        rng.Style.Border.Top.Color.SetColor(borderColor);

                        rng.Style.Border.Bottom.Style = borderStyle;
                        rng.Style.Border.Bottom.Color.SetColor(borderColor);

                        rng.Style.Border.Right.Style = borderStyle;
                        rng.Style.Border.Right.Color.SetColor(borderColor);
                    }

                    // 设置第二列的日期格式
                    using (ExcelRange rng = worksheet.Cells[2, 2, dtCourse.Rows.Count + 1, 2])
                    {
                        rng.Style.Numberformat.Format = "yyyy-MM-dd";
                    }

                    // 所有单元格自适应
                    worksheet.Cells.AutoFitColumns();
                }

                package.Save();
            }
            return path;
        }

        private string GenerateTempExcel1<T>(IEnumerable<T> collection)
        {
            string path = string.Empty;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/temp/";
            path = contentRootPath + documentPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string sFileName = $@"test{ DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            path = Path.Combine(path, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells["A2"].LoadFromCollection(collection, true);
                //worksheet.Cells["A2"].LoadFromDataTable();


                package.Save();
            }
            return path;
        }
    }
}
