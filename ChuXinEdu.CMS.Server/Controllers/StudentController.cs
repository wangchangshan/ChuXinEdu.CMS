using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json.Serialization;

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
        public ActionResult<string> GetStudentList(int pageIndex, int pageSize, string q)
        {
            QUERY_STUDENT query = JsonConvert.DeserializeObject<QUERY_STUDENT>(q);            
            
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Student, STUDENT_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            int totalCount = 0;
            IEnumerable<Student> students = _chuxinQuery.GetStudentList(pageIndex, pageSize, query, out totalCount);
            List<STUDENT_R_LIST> studentList = new List<STUDENT_R_LIST>();
            DataTable dtScpSimplify = _chuxinQuery.GetScpSimplify();

            STUDENT_R_LIST studentVM = null;
            foreach(Student student in students)
            {
                var studentCode = student.StudentCode;
                studentVM = mapper.Map<Student, STUDENT_R_LIST>(student);

                DataRow[] drArr = dtScpSimplify.Select("student_code = '" + studentCode + "'");
                List<Simplify_StudentCourse> ssList = new List<Simplify_StudentCourse>();
                foreach(DataRow dr in drArr)
                {
                    Simplify_StudentCourse ss = new Simplify_StudentCourse{
                        StudentCode = studentCode,
                        Code = dr["course_category_code"].ToString(),
                        Name = dr["course_category_name"].ToString()
                    };
                    dtScpSimplify.Rows.Remove(dr);
                    ssList.Add(ss);
                }
                studentVM.StudentAvatarPath = "http://localhost:8080/api/course/getimage?id=" + student.Id + "&type=avatar";
                studentVM.StudentCourseCategory = ssList;
                studentList.Add(studentVM);
            }

            dynamic obj = new {
                TotalCount = totalCount,
                StudentList = studentList
            };

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string reslutJson = JsonConvert.SerializeObject(obj,settings);

            return reslutJson;
        }

        /// <summary>
        /// [学生列表] 根据姓名获取学生列表for添加介绍的学生 list GET api/student/getstudentforrecommend
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentName}")]
        public ActionResult<string> GetStudentForRecommend(string studentName)
        {
            string reslutJson = string.Empty;
            DataTable dt = _chuxinQuery.GetStudentForRecommend(studentName);
            if(dt!= null)
            {
                reslutJson = JsonConvert.SerializeObject(dt);
            }
			return reslutJson;   
        }
        
         /// <summary>
        /// 添加新的推荐学员 POST api/student/postnewrecommend
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string PostNewRecommend([FromBody] StudentRecommend srd)
        {
            string result = string.Empty;

            srd.CreateDate = DateTime.Now;

            result = _chuxinWorkflow.AddNewRecommend(srd);

            return result;
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

            
            STUDENT_R_COURSE_OVERVIEW meishu = new STUDENT_R_COURSE_OVERVIEW {
                CourseCategoryCode = "meishu",
                CourseCategoryName = "美术",
                TotalCourseCount = 0,
                TotalRestCourseCount = 0,
                TotalTuition = 0.0m
            };
            STUDENT_R_COURSE_OVERVIEW shufa = new STUDENT_R_COURSE_OVERVIEW{
                CourseCategoryCode = "shufa",
                CourseCategoryName = "书法",
                TotalCourseCount = 0,
                TotalRestCourseCount = 0,
                TotalTuition = 0.0m
            };
            foreach (var coursePackage in baseInfo.CoursePackageList)
            {
                if(coursePackage.CourseCategoryCode == "meishu")
                {
                    meishu.TotalCourseCount += coursePackage.ActualCourseCount;
                    meishu.TotalRestCourseCount += coursePackage.RestCourseCount;
                    if(coursePackage.IsPayed == "Y")
                    {
                        meishu.TotalTuition += coursePackage.ActualPrice - coursePackage.FeeBackAmount;
                    }
                }
                else if(coursePackage.CourseCategoryCode == "shufa")
                {
                    shufa.TotalCourseCount += coursePackage.ActualCourseCount;
                    shufa.TotalRestCourseCount += coursePackage.RestCourseCount;
                    if(coursePackage.IsPayed == "Y")
                    {
                        shufa.TotalTuition += coursePackage.ActualPrice;
                    }
                }
            }
            List<STUDENT_R_COURSE_OVERVIEW> overview = new List<STUDENT_R_COURSE_OVERVIEW>();
            overview.Add(meishu);
            overview.Add(shufa);
            baseInfo.CourseOverview = overview;
            
            return baseInfo;
        }

        /// <summary>
        /// 获取学生附属信息[是否开启新试听 | 介绍人] GET api/student/getauxiliaryinfo
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentcode}")]
        public ActionResult<string> GetAuxiliaryInfo(string studentCode)
        {
            DataTable dt = _chuxinQuery.GetStudentAuxiliaryInfo(studentCode);
            string reslutJson = JsonConvert.SerializeObject(dt);
            return reslutJson;
        }

        /// <summary>
        /// 获取学生为完成套餐列表 GET api/student/getnofinishpackage
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentCode}")]
        public IEnumerable<StudentCoursePackage> GetNoFinishPackage(string studentCode)
        {
            IEnumerable<StudentCoursePackage> packageList = _chuxinQuery.GetNoFinishPackage(studentCode);
            return packageList;
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
        /// [通知信息] 获取未来7天过生日的学生 GET api/student/getbirthdaynotify
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetBirthdayNotify()
        {
            DataTable dt = _chuxinQuery.GetBirthdayIn7Days();
            string reslutJson = JsonConvert.SerializeObject(dt);
			//return Json(new { result = "Success", returnObj = reslutJson });            
            return reslutJson;            
        }

        /// <summary>
        /// [其他信息] 获取介绍的新学员列表 GET api/student/getrecommend
        /// </summary>
        /// <returns></returns>
        [HttpGet("{studentCode}")]
        public IEnumerable<StudentRecommend> GetRecommend(string studentCode)
        {
            IEnumerable<StudentRecommend> list = _chuxinQuery.GetRecommendStudentList(studentCode);
            return list;
        }

        /// <summary>
        /// [其他信息] 删除介绍的新学员 GET api/student/delrecommend
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public string DelRecommend(int id)
        {
            string result = string.Empty;
            result = _chuxinWorkflow.RemoveStudentRecommend(id);
            return result;
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
            package.PackagePrice = sysPackage.PackagePrice;            

            package.FlexCourseCount = package.ActualCourseCount;
            package.RestCourseCount = package.ActualCourseCount;
            package.ScpStatus = "00";
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
        /// 更新学生新试听状态 PUT api/student/updatetrialothercourse
        /// </summary>
        /// <returns></returns>
        [HttpPut("{studentcode}")]
        public string UpdateTrialOtherCourse(string studentCode, [FromBody] dynamic obj)
        {
            string result = _chuxinWorkflow.UpdateStudentTrialOtherCourse(studentCode, obj.curVal.ToString());
            return result;
        }

        /// <summary>
        /// 学生退费 PUT api/student/feeback
        /// </summary>
        /// <returns></returns>
        [HttpPut("{studentcode}")]
        public string Feeback(string studentCode, [FromBody] List<StudentCoursePackage> packageList)
        {
            string result = _chuxinWorkflow.SetStudentFeeBack(studentCode, packageList);
            return result;
        }

        /// <summary>
        /// 添加学生基本信息 PUT api/student/addstudent
        /// </summary>
        /// <returns>studentCode</returns>
        [HttpPost]
        public string AddStudent([FromBody] Student student)
        {
            DateTime time  = new DateTime();
            if(student.StudentRegisterDate != null)
            {
                time  = student.StudentRegisterDate;
            }
            else
            {
                time  = DateTime.Now;
            }
            string studentCode = TableCodeHelper.GenerateCode("student", "student_code", time);
            student.StudentCode = studentCode;
            student.TrialOtherCourse = "否";
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