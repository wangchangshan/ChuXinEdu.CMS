using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IChuXinQuery _chuxinQuery;

        public StudentController(IChuXinQuery chuxinQuery)
        {
            _chuxinQuery = chuxinQuery;    
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
                aw.ShowURL = "http://localhost:8080/api/course/getimage?artworkId=" + artwork.ArtworkId;

                artWorkList.Add(aw);
            }

            return artWorkList;
        }

        /// <summary>
        /// 提交新的课程套餐 POST api/student/postnewpackage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string PostNewPackage([FromBody] StudentCoursePackage package)
        {
            string packageCode = package.PackageCode;
            var sysPackage = _chuxinQuery.GetSysCoursePackage(packageCode);
            package.PackageName = sysPackage.PackageName;
            package.PackageCourseCount = sysPackage.PackageCourseCount;
            package.FlexCourseCount = sysPackage.PackageCourseCount;

            return "OK";
        }




        /// <summary>
        /// 添加新学生 POST api/student
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void Create([FromBody] string value)
        {
        }

        /// <summary>
        /// 更新学生 PUT api/student/5
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public void Update(int id, [FromBody] string value)
        {
        }



        // // 数据连接测试
        // [HttpGet("{studentcode}", Name = "GetStudentDescByCode")]
        // public StudentDescTest GetStudentDescByCode(string studentCode)
        // {
        //     StudentDescTest studentDesc = _chuxinQuery.GetStudentDescTest(studentCode);

        //     return studentDesc;
        // }

    }   
}