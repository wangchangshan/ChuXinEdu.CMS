using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Data;
using System.Net.Http;
using AutoMapper;
using Newtonsoft.Json;
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
        /// 获取待签到数目 GET api/course/gettorecordcount
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public int GetToRecordCount()
        {
            int count = _chuxinQuery.GetCoursesToSignInCount();
            return count;
        }

        /// <summary>
        /// 获取课时套餐将要结束（5节课）并且没有报新套餐的学生 GET api/course/gettofinish
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public ActionResult<string> GetToFinish()
        {
            // 暂时没有处理是否已经报了新的套餐
            DataTable dt = _chuxinQuery.GetCourseToFinishList();
            string reslutJson = JsonConvert.SerializeObject(dt);        
            return reslutJson;   
        }

        /// <summary>
        /// 获取课程作品 GET api/course/getcourseartwork
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<ART_WORK_R_LIST> GetCourseArtwork(int courseId)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<StudentArtwork, ART_WORK_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            IEnumerable<StudentArtwork> artworks = _chuxinQuery.GetArkworkByCourse(courseId);

            List<ART_WORK_R_LIST> artWorkList = new List<ART_WORK_R_LIST>();
            ART_WORK_R_LIST  aw = null;

            foreach (var artwork in artworks)
            {
                aw = mapper.Map<StudentArtwork, ART_WORK_R_LIST>(artwork);
                aw.ShowURL = "http://localhost:8080/api/course/getimage?id=" + artwork.ArtworkId + "&type=artwork";

                artWorkList.Add(aw);
            }

            return artWorkList;
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
        /// 确定补充上传作品 PUT api/course/artworksupplement
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string ArtworkSupplement(CL_U_SIGN_IN course)
        {
            string result = _chuxinWorkFlow.SupplementArtWork(course);

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
            //Environment.CurrentDirectory;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/" + studentCode + "/" ;
            
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
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            result = _chuxinWorkFlow.RemoveTempArtWork(courseId, uid, contentRootPath);
            return result;
        }

        // /// <summary>
        // /// 获取图片 DELETE api/course/getimage
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet]
        // public HttpResponseMessage GetImage(int artworkId)
        // {
        //     // netcore 下无法使用此方法
        //     // https://stackoverflow.com/questions/42460198/return-file-in-asp-net-core-web-api
        //     string docPath = _chuxinQuery.GetArtWorkTruePath(artworkId);
        //     string truePath = _hostingEnvironment.ContentRootPath + docPath;

        //     if(System.IO.File.Exists(truePath))
        //     {
        //         var imgByte = System.IO.File.ReadAllBytes(truePath);
        //         //从图片中读取流
        //         var imgStream = new MemoryStream(System.IO.File.ReadAllBytes(truePath));
        //         var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        //         {
        //             Content = new ByteArrayContent(imgByte)
        //             //或者
        //             //Content = new StreamContent(stream)
        //         };
        //         resp.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
        //         return resp;
        //     }
        //     else
        //     {
        //         var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        //         {
        //             Content = null
        //         };

        //         return resp;
        //     }    
        // }

        /// <summary>
        /// 获取图片 DELETE api/course/getimage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetImage(int id, string type)
        {
            string docPath = string.Empty;
            switch (type.ToLower())
            {
                case "artwork":
                    docPath = _chuxinQuery.GetArtWorkTruePath(id);
                break;
                
                case "avatar":
                    docPath = _chuxinQuery.GetAvatarTruePath(id);
                break;

                default:
                break;
            }
            string truePath = _hostingEnvironment.ContentRootPath + docPath;

            if(System.IO.File.Exists(truePath))
            {
                var imgByte = System.IO.File.ReadAllBytes(truePath);
                //从图片中读取流
                var imgStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(truePath));
                return File(imgStream, "image/jpg");
            }
            else
            {
                return NotFound();
            }    
        }
    }   
}