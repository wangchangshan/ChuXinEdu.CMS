using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkflow;

        public StudentController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkflow,  IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;    
            _chuxinWorkflow = chuxinWorkflow;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// [学生列表] 获取所有学生list GET api/student/getstudentlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<STUDENT_R_LIST> GetStudentList(int pageIndex, int pageSize)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Student, STUDENT_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();


            IEnumerable<Student> students = _chuxinQuery.GetStudentList(pageIndex, pageSize);
            List<STUDENT_R_LIST> studentList = new List<STUDENT_R_LIST>();
            IEnumerable<Simplify_StudentCourse> studentsCourseCategory = _chuxinQuery.GetAllStudentsCourse();

            STUDENT_R_LIST studentVM = null;
            foreach(Student student in students)
            {
                var studentCode = student.StudentCode;
                studentVM = mapper.Map<Student, STUDENT_R_LIST>(student);
                studentVM.StudentCourseCategory = studentsCourseCategory.Where(s => s.StudentCode == studentCode);
                studentList.Add(studentVM);
            }

            return studentList;
        }

        /// <summary>
        /// [学生排课] 获取待排课学生列表 GET api/student/getstudentstoselectcourse
        /// 说明：不显示当前时间段已经选过课程的学生
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<StudentCoursePackage> GetStudentsToSelectCourse(string dayCode, string periodName)
        {
            IEnumerable<StudentCoursePackage>  studentList = _chuxinQuery.GetStudentToSelectCourse(dayCode, periodName);
            return studentList;
        }

        /// <summary>
        /// [学生排课] 获取待试听排课学生列表 GET api/student/gettempstudentstoselectCourse
        /// 说明：不显示当前时间段已经选过课程的学生
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<StudentTemp> GetTempStudentsToSelectCourse()
        {
            IEnumerable<StudentTemp> studentList = _chuxinQuery.GetTempStudentToSelectCourse();
            return studentList;
        }    

        /// <summary>
        /// 获取学生基础信息 GET api/student/getbaseinfo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public STUDENT_R_BASEINO GetBaseInfo(string studentCode)
        {
            STUDENT_R_BASEINO baseInfo = new STUDENT_R_BASEINO();

            baseInfo.StudentInfo = _chuxinQuery.GetStudentByCode(studentCode);
            if(!String.IsNullOrEmpty(baseInfo.StudentInfo.StudentAvatarPath))
            {
                int id = baseInfo.StudentInfo.Id;
                baseInfo.StudentInfo.StudentAvatarPath = "http://localhost:8080/api/course/getimage?id=" + id + "&type=avatar&rnd="+ System.Guid.NewGuid().ToString("N");
            }
            baseInfo.CoursePackageList =  _chuxinQuery.GetStudentCoursePackage(studentCode);

            int totalCount = 0;
            decimal tuition = 0.0m;
            foreach (var coursePackage in baseInfo.CoursePackageList)
            {
                totalCount += coursePackage.PackageCourseCount;
                if(coursePackage.IsPayed == "Y")
                {
                    tuition += coursePackage.ActualPrice;
                }
            }
            baseInfo.TotalCourseCount = totalCount;
            baseInfo.TotalTuition = tuition;
            if(totalCount > 0)
            {
                int signInCount = _chuxinQuery.GetStudentSignInCourseCount(studentCode);
                baseInfo.RestCourseCount = totalCount - signInCount;
            }
            else
            {
                baseInfo.RestCourseCount = 0;
            }         
            
            return baseInfo;
        }

        /// <summary>
        /// 获取学生上课列表 GET api/student/getcourselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<StudentCourseList> GetCourseList(string studentCode)
        {
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetStudentCourseList(studentCode);
            return courseList;
        }

        /// <summary>
        /// 获取学生所有的课程作品 GET api/student/getartworklist
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IEnumerable<ART_WORK_R_LIST> GetArtworkList(string studentCode)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<StudentArtwork, ART_WORK_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            IEnumerable<StudentArtwork> artworks = _chuxinQuery.GetArkworkByStudent(studentCode);

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
        /// 添加新的课程套餐 POST api/student/postnewpackage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string PostNewPackage([FromBody] StudentCoursePackage package)
        {
            string result = string.Empty;

            string packageCode = package.PackageCode;
            var sysPackage = _chuxinQuery.GetSysCoursePackage(packageCode);
            package.PackageName = sysPackage.PackageName;
            package.CourseCategoryCode = sysPackage.PackageCourseCategoryCode;
            package.CourseCategoryName = sysPackage.PackageCourseCategoryName;
            package.PackageCourseCount = sysPackage.PackageCourseCount;
            package.FlexCourseCount = sysPackage.PackageCourseCount;
            package.PackagePrice = sysPackage.PackagePrice;
            package.CreateTime = DateTime.Now;
            if(package.ActualPrice == 0)
            {
                package.ActualPrice = sysPackage.PackagePrice;
                package.IsDiscount = "N";
            }
            else
            {
                package.IsDiscount = "Y";
            }

            result = _chuxinWorkflow.AddStudentCoursePackage(package);

            return result;
        }

        /// <summary>
        /// 删除学生课程套餐 DELETE api/student/removecoursepackage
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string RemoveCoursePackage(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.RemoveStudentCoursePackage(id);
            return result;
        }

        /// <summary>
        /// 更新学生套餐 PUT api/student/updatestudentpackage
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public string UpdateStudentPackage(int id, [FromBody] StudentCoursePackage package)
        {
            string result = _chuxinWorkflow.UpdateStudentCoursePackage(id, package);
            return result;
        }

        /// <summary>
        /// 添加学生基本信息 PUT api/student/addstudent
        /// </summary>
        /// <returns>studentCode</returns>
        [HttpPost]
        public string AddStudent([FromBody] Student student)
        {
            string studentCode = TableCodeHelper.GenerateStudentCode();
            student.StudentCode = studentCode;
            string result = _chuxinWorkflow.AddStudentBaseInfo(student);
            if(result == "500")
            {
                studentCode = "";
            }
            return studentCode;
        }

        /// <summary>
        /// 更新学生基本信息 PUT api/student/updatestudent
        /// </summary>
        /// <returns></returns>
        [HttpPut("{studentCode}")]
        public string UpdateStudent(string studentCode, [FromBody] Student student)
        {
            string result = _chuxinWorkflow.UpdateStudentBaseInfo(studentCode, student);
            return result;
        }

        /// <summary>
        /// 上传头像 POST api/student/uploadavatar
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadAvatar()
        {
            string result = "201";
            string studentCode = string.Empty;
            string studentName = string.Empty;
            if(HttpContext.Request.Form.ContainsKey("studentCode"))
            {
                studentCode = HttpContext.Request.Form["studentCode"];
                studentName = HttpContext.Request.Form["studentName"];
            }
            else
            {
                return result;
            }

            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/avatars/" ;
            
            if(!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);            
            }

            var file =  HttpContext.Request.Form.Files.FirstOrDefault();
            if(file != null)
            {  
                string ext = Path.GetExtension(file.FileName);
                string newName = string.Format("{0}_{1}{2}", studentName,studentCode, ext);
                documentPath = documentPath + newName;
                string savePath = contentRootPath + documentPath;

                using(var stream = System.IO.File.Create(savePath))
                {
                    file.CopyTo(stream);
                }

                result = _chuxinWorkflow.UploadAvatar(studentCode, documentPath);
            }
            else
            {
                return result;
            }
            return result;
        }
    }   
}