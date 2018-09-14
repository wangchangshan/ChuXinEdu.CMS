using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public CourseController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取学生排课列表 GET api/course/getarrangedcourselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<Simplify_StudentCourseList> GetArrangedCourseList(string studentCode, string dayCode, string coursePeriod)
        {
            IEnumerable<Simplify_StudentCourseList> courseList = _chuxinQuery.GetArrangedCourseList(studentCode, dayCode, coursePeriod);
            return courseList;
        }

        /// <summary>
        /// 获取学生待签到列表 GET api/course/getattendancelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<StudentCourseList> GetAttendanceList()
        {
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetCoursesToSignIn();
            return courseList;
        }

        /// <summary>
        /// 签到 PUT api/course/putsignin
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutSignIn(CL_U_SIGN_IN course)
        {
            string result = _chuxinWorkFlow.SignInSingle(course);

            return result;
        }

        /// <summary>
        /// 批量签到 PUT api/course/putsigninbatch
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string PutSignInBatch(List<CL_U_SIGN_IN> courseList)
        {
            string result = _chuxinWorkFlow.SignInBatch(courseList);

            return result;
        }

        /// <summary>
        /// 签到 上传作品 POST api/course/uploadartwork
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int UploadArtwork()
        {
            int result = -1;
            int courseId = -1;
            string studentCode = string.Empty;
            string studentName = string.Empty;
            string uid = string.Empty;
            if(HttpContext.Request.Form.ContainsKey("courseId"))
            {
                courseId = Int32.Parse(HttpContext.Request.Form["courseId"]);
                studentCode = HttpContext.Request.Form["studentCode"];
                studentName = HttpContext.Request.Form["studentName"];
                uid = HttpContext.Request.Form["uid"];
            }
            else
            {
                return result;
            }

            //string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "\\cxdocs\\" + studentCode + "\\" ;
            
            if(!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);            
            }

            var file =  HttpContext.Request.Form.Files.FirstOrDefault();
            if(file != null)
            {  
                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}_{1}_{2}{3}", studentName,System.Guid.NewGuid().ToString("N"),courseId.ToString(), ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                using(var stream = System.IO.File.Create(savePath))
                {
                    file.CopyTo(stream);
                }

                string fileSize = System.Math.Ceiling(file.Length / 1024.0 / 1024.0) + " MB";
                // 数据入库
                StudentArtwork artWork = new StudentArtwork {
                    TempUId = uid,
                    StudentCourseId = courseId,
                    StudentCode = studentCode,
                    StudentName = studentName,
                    DocumentPath = documentPath,  
                    DocumentType = ext,
                    DocumentSize = fileSize, 
                    ArtworkStatus = "00",
                    CreateDate = DateTime.Now             
                };
                result = _chuxinWorkFlow.UploadArtWork(artWork);
            }
            else
            {
                return result;
            }
            return result;
        }

        /// <summary>
        /// 删除临时文件 DELETE api/course/deltempfile
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string DelTempFile(int courseId, string uid)
        {
            string result = string.Empty;
            result = _chuxinWorkFlow.RemoveTempArtWork(courseId, uid);
            return result;
        }
    }   
}