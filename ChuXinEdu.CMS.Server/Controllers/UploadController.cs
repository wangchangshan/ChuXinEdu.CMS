using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using Microsoft.Extensions.Logging;
using System.Drawing;
using ChuXinEdu.CMS.Server.Utils;

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
        private readonly ILogger<UploadController> _logger;
        public UploadController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, IHostingEnvironment hostingEnvironment, ILogger<UploadController> logger)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// 获取图片 Get api/upload/getimage
        /// 说明：微信端用的宣传图片，在上传时就已经作了尺寸裁剪、以及图片质量压缩操作。
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
                    // 学员作品（PC端）
                    docPath = _chuxinQuery.GetArtWorkTruePath(id);
                    break;

                case "artwork-wx":
                    // 学员作品（微信端）
                    docPath = _chuxinQuery.GetArtWorkTruePath(id);
                    break;

                case "avatar-s":
                    // 学员头像（PC端）
                    docPath = _chuxinQuery.GetAvatarTruePath(id, "student");
                    break;

                case "avatar-s-wx":
                    // 学员小头像（微信端）
                    docPath = _chuxinQuery.GetAvatarTruePath(id, "student");
                    if (!String.IsNullOrEmpty(docPath))
                    {
                        docPath += "_80.png";
                    }
                    break;

                case "avatar-t":
                    // 教师头像（PC端）
                    docPath = _chuxinQuery.GetAvatarTruePath(id, "teacher");
                    break;

                case "ad-wx":
                    // 宣传图片（上传时，已经被压缩处理过了）
                    docPath = _chuxinQuery.GetWeiXinPicTruePath(id);
                    break;

                default:
                    break;
            }
            if (string.IsNullOrEmpty(docPath))
            {
                if (type.IndexOf("avatar") > -1)
                {
                    docPath = "/image/avatar-default.png";
                }
                else
                {
                    return NotFound();
                }
            }
            string truePath = _hostingEnvironment.ContentRootPath + docPath;

            if (System.IO.File.Exists(truePath))
            {
                if (type == "artwork-wx")
                {
                    // 微信小程序用的作品图， 判断是否生成了相应的缩略图

                    int wxWidth = 750;
                    string wxImageWidth = CustomConfig.GetSetting("WeiXinImageWidth");
                    if (!String.IsNullOrEmpty(wxImageWidth))
                    {
                        wxWidth = Convert.ToInt32(wxImageWidth);
                    }

                    string ext = Path.GetExtension(truePath);
                    string thumbPath = truePath + "_" + wxWidth.ToString() + ext;
                    if (!System.IO.File.Exists(thumbPath))
                    {
                        using (var stream = System.IO.File.OpenRead(truePath))
                        {
                            Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                            int oriWidth = bitmap.Width;
                            int oriHeight = bitmap.Height;

                            int wxHeight = Convert.ToInt32(wxWidth * 1.0 / oriWidth * oriHeight);

                            ImageHelper.SaveThumbnailImage(bitmap, truePath, wxWidth, wxHeight, true, ext);
                        }
                    }
                    truePath = thumbPath;
                }

                //从图片中读取流
                MemoryStream imgStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(truePath));
                return File(imgStream, "image/jpg");
            }
            else
            {
                return NotFound();
            }
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
            if (HttpContext.Request.Form.ContainsKey("courseId"))
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
            string documentPath = "/cxdocs/" + studentCode + "/";

            if (!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);
            }

            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                int imageCompressLevel = 100; // max
                string strImageCompressLevel = CustomConfig.GetSetting("ImageCompressLevel");
                if (!String.IsNullOrEmpty(strImageCompressLevel))
                {
                    imageCompressLevel = Convert.ToInt32(strImageCompressLevel);
                }

                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}_{1}_{2}{3}", studentName, System.Guid.NewGuid().ToString("N"), courseId.ToString(), ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                string fileSize = string.Empty;
                // 压缩上传图片
                if (imageCompressLevel < 100)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (Stream s = new FileStream(savePath, FileMode.Create))
                        {

                            Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                            ImageHelper.Compress(bitmap, s, imageCompressLevel);
                            fileSize = (s.Length / 1024.0).ToString("0.00") + " KB";
                        }
                    }
                }
                else
                {
                    // 存储原图
                    using (var stream = System.IO.File.Create(savePath))
                    {
                        file.CopyTo(stream);
                    }
                    fileSize = System.Math.Ceiling(file.Length / 1024.0) + " KB";
                }

                // 数据入库
                StudentArtwork artWork = new StudentArtwork
                {
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
        /// 签到 上传作品（学员作品墙页面的上传）POST api/upload/uploadartworksimple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int UploadArtworkSimple()
        {
            int result = -1;
            string studentCode = string.Empty;
            string studentName = string.Empty;
            string uid = string.Empty;
            if (HttpContext.Request.Form.ContainsKey("studentCode"))
            {
                studentCode = HttpContext.Request.Form["studentCode"] + "";
                studentName = HttpContext.Request.Form["studentName"] + "";
                uid = HttpContext.Request.Form["uid"] + "";
            }
            else
            {
                _logger.LogWarning("批量上传：无法获取studentCode");
                return result;
            }
            try
            {
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                string documentPath = "/cxdocs/" + studentCode + "/";

                if (!Directory.Exists(contentRootPath + documentPath))
                {
                    Directory.CreateDirectory(contentRootPath + documentPath);
                }

                var file = HttpContext.Request.Form.Files.FirstOrDefault();
                if (file != null)
                {
                    int imageCompressLevel = 100; // max
                    string strImageCompressLevel = CustomConfig.GetSetting("ImageCompressLevel");
                    if (!String.IsNullOrEmpty(strImageCompressLevel))
                    {
                        imageCompressLevel = Convert.ToInt32(strImageCompressLevel);
                    }

                    string ext = Path.GetExtension(file.FileName);
                    string newName = string.Format("{0}_{1}{2}", studentName, System.Guid.NewGuid().ToString("N"), ext);
                    documentPath = documentPath + newName;
                    string savePath = contentRootPath + documentPath;

                    string fileSize = string.Empty;
                    // 压缩上传图片
                    if (imageCompressLevel < 100)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            using (Stream s = new FileStream(savePath, FileMode.Create))
                            {

                                Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                                ImageHelper.Compress(bitmap, s, imageCompressLevel);
                                fileSize = (s.Length / 1024.0).ToString("0.00") + " KB";
                            }
                        }
                    }
                    else
                    {
                        using (var stream = System.IO.File.Create(savePath))
                        {
                            file.CopyTo(stream);
                        }
                        fileSize = (file.Length / 1024.0).ToString("0.00") + " KB";
                    }

                    // 数据入库
                    StudentArtwork artWork = new StudentArtwork
                    {
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
                    _logger.LogWarning("批量上传：无法获取文件");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量上传：错误！{0}", ex.Message.ToString());
            }
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
            if (HttpContext.Request.Form.ContainsKey("type"))
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

            if (!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);
            }

            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}_{1}{2}", name, code, ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                // 存储原图
                using (var stream = System.IO.File.Create(savePath))
                {
                    file.CopyTo(stream);
                }

                // 存储微信小程序缩略图头像 80X80
                using (var stream = file.OpenReadStream())
                {
                    Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                    ImageHelper.SaveThumbnailImageAvatar(bitmap, savePath, 80, 80, ext);
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
        /// 上传活动照片 POST api/upload/uploadactivityimage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadActivityImage()
        {
            string result = "1600";

            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/activitys/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/";

            if (!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);
            }

            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                int imageCompressLevel = 100; // max
                string strImageCompressLevel = CustomConfig.GetSetting("ImageCompressLevel");
                if (!String.IsNullOrEmpty(strImageCompressLevel))
                {
                    imageCompressLevel = Convert.ToInt32(strImageCompressLevel);
                }

                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}{1}", Guid.NewGuid().ToString("N"), ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                string fileSize = string.Empty;
                // 压缩上传图片
                if (imageCompressLevel < 100)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (Stream s = new FileStream(savePath, FileMode.Create))
                        {

                            Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                            ImageHelper.Compress(bitmap, s, imageCompressLevel);
                            fileSize = (s.Length / 1024.0).ToString("0.00") + " KB";
                        }
                    }
                }
                else
                {
                    using (var stream = System.IO.File.Create(savePath))
                    {
                        file.CopyTo(stream);
                    }
                    fileSize = (file.Length / 1024.0).ToString("0.00") + " KB";
                }

                // 数据入库
                StudentActivityImage activity = new StudentActivityImage
                {
                    ActivityId = 0
                };

                // 暂时不处理
                //result = _chuxinWorkFlow.UploadAvtivityImages(activity);
            }
            else
            {
                return result;
            }
            return result;
        }

        /// <summary>
        /// 上传微信小程序用到的宣传图片（默认压缩） POST api/upload/uploadwxpic
        /// 将文件宽度限制为750px, 高度等比例缩放
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadWxPic()
        {
            string result = "1200";
            string studentCode = string.Empty;
            string studentName = string.Empty;
            string picTypCode = string.Empty;
            if (!HttpContext.Request.Form.ContainsKey("wxPictureType"))
            {
                _logger.LogWarning("微信小程序图片上传，无法获取pictypecode");
                return "1404";
            }

            picTypCode = HttpContext.Request.Form["wxPictureType"] + "";

            try
            {
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                string documentPath = "/cxdocs/weixin" + picTypCode + "/";

                if (!Directory.Exists(contentRootPath + documentPath))
                {
                    Directory.CreateDirectory(contentRootPath + documentPath);
                }

                var file = HttpContext.Request.Form.Files.FirstOrDefault();
                if (file != null)
                {
                    string newName = System.Guid.NewGuid().ToString("N");
                    documentPath = documentPath + newName;
                    string savePath = contentRootPath + documentPath;

                    // 存储微信用到的图像 750X?
                    using (var stream = file.OpenReadStream())
                    {
                        Bitmap bitmap = new Bitmap(Bitmap.FromStream(stream));
                        int oriWidth = bitmap.Width;
                        int oriHeight = bitmap.Height;

                        int newWidth = 750;
                        string wxImageWidth = CustomConfig.GetSetting("WeiXinImageWidth");
                        if (!String.IsNullOrEmpty(wxImageWidth))
                        {
                            newWidth = Convert.ToInt32(wxImageWidth);
                        }
                        int newHeight = Convert.ToInt32(newWidth * 1.0 / oriWidth * oriHeight);

                        string ext = ".png";
                        ImageHelper.SaveThumbnailImage(bitmap, savePath, newWidth, newHeight, true, ext);
                        documentPath = documentPath + "_" + newWidth.ToString() + ext;
                    }

                    int age = 0;
                    try
                    {
                        age = Int32.Parse(HttpContext.Request.Form["studentAge"]);
                    }
                    catch { }

                    WxPicture wxPicture = new WxPicture
                    {
                        subject = HttpContext.Request.Form["subject"] + "",
                        StudentCode = HttpContext.Request.Form["studentCode"] + "",
                        StudentName = HttpContext.Request.Form["studentName"] + "",
                        StudentAge = age,
                        TeacherCode = HttpContext.Request.Form["teacherCode"] + "",
                        PicturePath = documentPath,
                        WxPictureType = picTypCode
                    };
                    result = _chuxinWorkFlow.UploadWxPicture(wxPicture);
                }
                else
                {
                    _logger.LogWarning("微信图片上传：无法获取文件");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量上传：错误！{0}", ex.Message.ToString());
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
            foreach (string uid in uids)
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
            foreach (string uid in uids)
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
        /// 删除微信小程序图片 DELETE api/upload/dewxpicture
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public string DelWxPicture(int id)
        {
            string result = string.Empty;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            result = _chuxinWorkFlow.RemoveWxPicture(id, contentRootPath);
            return result;
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