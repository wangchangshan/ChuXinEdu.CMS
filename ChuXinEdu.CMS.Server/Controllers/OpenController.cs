using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.ViewModel;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class OpenController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public OpenController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }



        /// <summary>
        /// [配置] arrange_guid 数据迁移接口 GET api/open/migrate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Migrate()
        {
            string result = "200";
            using (BaseContext context = new BaseContext())
            {
                var arrangeList = context.StudentCourseArrange.Where(a => a.ArrangeGuid == null).ToList();
                if (arrangeList.Count > 0)
                {
                    foreach (StudentCourseArrange arrange in arrangeList)
                    {
                        string guid = Guid.NewGuid().ToString("N");
                        var courseList = context.StudentCourseList.Where(c => c.StudentCoursePackageId == arrange.StudentCoursePackageId
                                                                               && c.StudentCode == arrange.StudentCode
                                                                               && c.CourseWeekDay == arrange.CourseWeekDay
                                                                               && c.CoursePeriod == arrange.CoursePeriod
                                                                               && c.Classroom == arrange.Classroom
                                                                               && c.ArrangeTemplateCode == arrange.ArrangeTemplateCode
                                                                        ).ToList();
                        foreach (StudentCourseList course in courseList)
                        {
                            course.ArrangeGuid = guid;
                        }
                        arrange.ArrangeGuid = guid;
                        context.SaveChanges();
                    }

                    var otherCourseList = context.StudentCourseList.Where(c => c.ArrangeGuid == null).ToList();
                    foreach (var otherCourse in otherCourseList)
                    {
                        otherCourse.ArrangeGuid = "0";
                    }

                    context.SaveChanges();
                }
                else
                {
                    result = "no record need to migrate.";
                }

            }

            return result;
        }

        // GET api/open/getpackages
        [HttpGet]
        public IEnumerable<SysCoursePackage> GetPackages(string q)
        {
            QUERY_SYS_PACKAGE query = JsonConvert.DeserializeObject<QUERY_SYS_PACKAGE>(q);
            IEnumerable<SysCoursePackage> packageList = _chuxinQuery.GetSysCoursePackageList(query);
            return packageList;
        }

        /// <summary>   
        /// 获取某天排课信息 GET api/open/getcoursearrangedbyday
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<StudentCourseList> GetCourseArrangedbyDay(string day)
        {
            DateTime theDay = Convert.ToDateTime(day).Date;
            List<StudentCourseList> scls = _chuxinQuery.GetCoursesByday(theDay);
            return scls;
        }

        /// <summary>
        /// 获取学生上课列表 GET api/open/getcourselist
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
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StudentArtwork, ART_WORK_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            IEnumerable<StudentArtwork> artworks = _chuxinQuery.GetArkworkByStudent(studentCode);

            List<ART_WORK_R_LIST> artWorkList = new List<ART_WORK_R_LIST>();
            ART_WORK_R_LIST aw = null;

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var artwork in artworks)
            {
                aw = mapper.Map<StudentArtwork, ART_WORK_R_LIST>(artwork);
                aw.ShowURL = accessUrlHost + "api/upload/getimage?id=" + artwork.ArtworkId + "&type=artwork";

                artWorkList.Add(aw);
            }

            return artWorkList;
        }

        /// <summary>
        /// [学生列表] 获取所有学生list GET api/student/getstudentlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetStudentList(int pageIndex, int pageSize, string q)
        {
            QUERY_STUDENT query = JsonConvert.DeserializeObject<QUERY_STUDENT>(q);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Student, STUDENT_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            int totalCount = 0;
            IEnumerable<Student> students = _chuxinQuery.GetStudentList(pageIndex, pageSize, query, out totalCount);
            List<STUDENT_R_LIST> studentList = new List<STUDENT_R_LIST>();
            DataTable dtScpSimplify = _chuxinQuery.GetScpSimplify();

            STUDENT_R_LIST studentVM = null;
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (Student student in students)
            {
                var studentCode = student.StudentCode;
                studentVM = mapper.Map<Student, STUDENT_R_LIST>(student);

                DataRow[] drArr = dtScpSimplify.Select("student_code = '" + studentCode + "'");
                List<Simplify_StudentCourse> ssList = new List<Simplify_StudentCourse>();
                foreach (DataRow dr in drArr)
                {
                    Simplify_StudentCourse ss = new Simplify_StudentCourse
                    {
                        StudentCode = studentCode,
                        Code = dr["course_category_code"].ToString(),
                        Name = dr["course_category_name"].ToString()
                    };
                    dtScpSimplify.Rows.Remove(dr);
                    ssList.Add(ss);
                }
                studentVM.StudentAvatarPath = accessUrlHost + "api/upload/getimage?id=" + student.Id + "&type=avatar-s";
                studentVM.StudentCourseCategory = ssList;
                studentList.Add(studentVM);
            }

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd"
            };

            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = studentList
            }, settings);
        }
    }
}