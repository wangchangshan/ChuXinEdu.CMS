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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public UploadController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _hostingEnvironment = hostingEnvironment;
        }
       

        /// <summary>
        /// 签到 上传作品 POST api/upload/uploadartwork
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
        /// 签到 上传作品 POST api/upload/uploadartworksimple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int UploadArtworkSimple()
        {
            int result = -1;
            string studentCode = string.Empty;
            string studentName = string.Empty;
            string uid = string.Empty;
            if(HttpContext.Request.Form.ContainsKey("studentCode"))
            {
                studentCode = HttpContext.Request.Form["studentCode"];
                studentName = HttpContext.Request.Form["studentName"];
                uid = HttpContext.Request.Form["uid"];
            }
            else
            {
                return result;
            }

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
                string newName = string.Format("{0}_{1}_x{2}", studentName,System.Guid.NewGuid().ToString("N"), ext);
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
                    StudentCourseId = 0,
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
        /// 删除临时文件 DELETE api/upload/deltempfile
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

        /// <summary>
        /// 删除临时文件 DELETE api/upload/delalltempfile
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string DelAllTempFile(string[] uids)
        {
            string result = string.Empty;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            foreach(string uid in uids)
            {
                result = _chuxinWorkFlow.RemoveTempArtWork(0, uid, contentRootPath);
            }
            return result;
        }

        /// <summary>
        /// 删除临时文件 DELETE api/upload/deltempfilebycourse
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{courseId}")]
        public string DelTempFileByCourse(int courseId, string[] uids)
        {
            string result = string.Empty;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            foreach(string uid in uids)
            {
                result = _chuxinWorkFlow.RemoveTempArtWork(courseId, uid, contentRootPath);
            }
            return result;
        }

        /// <summary>
        /// 删除作品 DELETE api/upload/delachievement
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{achievementId}")]
        public string DelAchievement(int achievementId)
        {
            string result = string.Empty;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            result = _chuxinWorkFlow.RemoveArtWorkById(achievementId, contentRootPath);
            return result;
        }

        /// <summary>
        /// 确定批量上传作品 PUT api/upload/artwork2yes
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public string Artwork2Yes([FromBody] string[] uids)
        {
            string result = _chuxinWorkFlow.SupplementArtWork(uids);

            return result;
        }


        /// <summary>
        /// 上传头像 POST api/upload/uploadavatar
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadAvatar()
        {
            string result = "1600";
            string code = string.Empty;
            string name = string.Empty;
            string type = string.Empty;
            if(HttpContext.Request.Form.ContainsKey("type"))
            {
                type = HttpContext.Request.Form["type"];
                code = HttpContext.Request.Form["code"];
                name = HttpContext.Request.Form["name"];
            }
            else
            {
                return result;
            }

            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/avatars/" + type + "/";
            
            if(!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);            
            }

            var file =  HttpContext.Request.Form.Files.FirstOrDefault();
            if(file != null)
            {  
                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}_{1}{2}", name,code, ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                using(var stream = System.IO.File.Create(savePath))
                {
                    file.CopyTo(stream);
                }

                result = _chuxinWorkFlow.UploadAvatar(code, documentPath, type);
            }
            else
            {
                return result;
            }
            return result;
        }

        /// <summary>
        /// 获取图片 Get api/upload/getimage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetImage(int id, string type)
        {
            string docPath = string.Empty;
            type = type.ToLower();
            switch (type)
            {
                case "artwork":
                    docPath = _chuxinQuery.GetArtWorkTruePath(id);
                break;
                
                case "avatar-s":
                    docPath = _chuxinQuery.GetAvatarTruePath(id, "student");
                break;
                case "avatar-t":
                    docPath = _chuxinQuery.GetAvatarTruePath(id, "teacher");
                break;

                default:
                break;
            }
            if(string.IsNullOrEmpty(docPath))
            {
                if(type.IndexOf("avatar") > -1 )
                {
                    docPath = "/image/avatar-default.png";
                }
                else 
                {
                    return NotFound();
                }
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

        // /// <summary>
        // /// 获取图片 
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
    }   
}