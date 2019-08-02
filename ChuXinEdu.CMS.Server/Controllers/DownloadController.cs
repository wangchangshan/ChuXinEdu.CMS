using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(IChuXinQuery chuxinQuery, IHostingEnvironment hostingEnvironment, ILogger<DownloadController> logger)
        {
            _chuxinQuery = chuxinQuery;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// [导出] 导出学生上课记录（旧页面） GET api/download/studentcourse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult StudentCourse(string studentCode)
        {
            byte[] result = null;
            try
            {
                string docPath = string.Empty;
                string fileName = studentCode + "_course_history.xlsx";

                DataTable dtCourse = ADOContext.GetDataTable($@"select package_name as '套餐名称',scl.course_date as '日期', scl.course_period as '时间',scl.course_folder_name as '课程类别',course_subject as '课程内容',teacher_name '上课教师' 
                                                                        from student_course_list scl
                                                                        left join student_course_package scp on scl.student_course_package_id = scp.id
                                                                        where scl.student_code='{studentCode}' and (attendance_status_code = '01' or attendance_status_code = '02') and course_type='正式' 
                                                                        order by student_course_package_id,course_period,course_date");

                result = GenerateExcel_StudentCourse(dtCourse, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "学生上课记录导出失败！");
            }

            return File(result, "application/vnd.ms-excel");
        }

        /// <summary>
        /// [导出] 导出学生上课记录（新页面） GET api/download/studentcourse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult StudentHistoryCourse(string studentCode, int? studentPackageId)
        {
            byte[] result = null;
            try
            {
                string docPath = string.Empty;
                DataTable dtCourse = null;
                string fileName = studentCode + "_course_history.xlsx";
                if (studentPackageId != null)
                {
                    dtCourse = ADOContext.GetDataTable($@"select package_name as '套餐名称',scl.course_date as '日期', scl.course_period as '时间',scl.course_folder_name as '课程类别',course_subject as '课程内容',teacher_name '上课教师' 
                                                        from student_course_list scl
                                                        left join student_course_package scp on scl.student_course_package_id = scp.id
                                                        where scl.student_code='{studentCode}' and (attendance_status_code = '01' or attendance_status_code = '02') and course_type='正式' 
                                                        and scl.student_course_package_id = {studentPackageId} 
                                                        order by student_course_package_id,course_period,course_date");
                }
                else
                {
                    dtCourse = ADOContext.GetDataTable($@"select package_name as '套餐名称',scl.course_date as '日期', scl.course_period as '时间',scl.course_folder_name as '课程类别',course_subject as '课程内容',teacher_name '上课教师' 
                                                        from student_course_list scl
                                                        left join student_course_package scp on scl.student_course_package_id = scp.id
                                                        where scl.student_code='{studentCode}' and (attendance_status_code = '01' or attendance_status_code = '02') and course_type='正式' 
                                                        order by student_course_package_id,course_period,course_date");
                }
                
                result = GenerateExcel_StudentCourse(dtCourse, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "学生上课记录导出失败！");
            }

            return File(result, "application/vnd.ms-excel");
        }

        private Byte[] GenerateExcel_StudentCourse(DataTable dt, string fileName)
        {
            DataTable dtPackages = dt.DefaultView.ToTable(true, "套餐名称");
            using (ExcelPackage package = new ExcelPackage())
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

                var result = package.GetAsByteArray();
                return result;
            }
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
