using System;
using System.Data;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.Utils;
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
    public class WxOpenController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public WxOpenController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
        }

        /// <summary>
        /// 获取微信小程序用到的宣传图片 GET api/wxopen/getwxhomepicture
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WxPicture> GetWxHomePicture()
        {
            IEnumerable<WxPicture> wxPics = _chuxinQuery.GetWxHomePicture();

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var pic in wxPics)
            {
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=ad-wx";
            }

            return wxPics;
        }

        /// <summary>
        /// 获取微信小程序用到的宣传图片 GET api/wxopen/getwxpicture （暂时没有被调用）
        /// </summary>
        /// <returns></returns>
        [HttpGet("{wxPicCode}")]
        public IEnumerable<WxPicture> GetWxPicture(string wxPicCode)
        {
            IEnumerable<WxPicture> wxPics = _chuxinQuery.GetWxPicture(wxPicCode);

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var pic in wxPics)
            {
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=ad-wx";
            }

            return wxPics;
        }

        /// <summary>
        /// 获取微信小程序用到的宣传图片——作品展示（带屏幕上拉分页） GET api/wxopen/getwxbestdraw
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WxPicture> GetWxBestDraw(string wxPicCode, int pageIndex, int pageSize)
        {
            IEnumerable<WxPicture> wxPics = _chuxinQuery.GetWxPicture(wxPicCode, pageIndex, pageSize);

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var pic in wxPics)
            {
                pic.PicturePath = accessUrlHost + "api/upload/getimage?id=" + pic.Id + "&type=ad-wx";
            }

            return wxPics;
        }

        // GET api/wxopen/getpackages
        [HttpGet]
        [WxAuthenFilter]
        public IEnumerable<SysCoursePackage> GetPackages(string q)
        {
            QUERY_SYS_PACKAGE query = JsonConvert.DeserializeObject<QUERY_SYS_PACKAGE>(q);
            IEnumerable<SysCoursePackage> packageList = _chuxinQuery.GetSysCoursePackageList(query);
            return packageList;
        }

        /// <summary>   
        /// 获取某天排课信息 GET api/wxopen/getcoursearrangedbyday
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public List<StudentCourseList> GetCourseArrangedbyDay(string day)
        {
            DateTime theDay = Convert.ToDateTime(day).Date;
            List<StudentCourseList> scls = _chuxinQuery.GetCoursesByday(theDay);
            return scls;
        }

        /// <summary>
        /// 【弃用】获取学生上课列表 GET api/wxopen/getcourselist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public IEnumerable<StudentCourseList> GetCourseList(string studentCode, int pageIndex, int pageSize)
        {
            IEnumerable<StudentCourseList> courseList = _chuxinQuery.GetStudentCourseList(studentCode, pageIndex, pageSize);
            return courseList;
        }

        /// <summary>
        /// 获取学生套餐内的上课记录 GET api/wxopen/getcoursesbypackage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetCoursesByPackage(int sPackageId, int pageIndex, int pageSize)
        {
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            List<StudentCourseList> courseList = _chuxinQuery.GetCoursesByPackage(sPackageId, pageIndex, pageSize);
            int courseCount = courseList.Count;

            List<WX_COURSE_HISTORY> courseHistoryList = new List<WX_COURSE_HISTORY>();
            WX_COURSE_HISTORY courseHistory = null;

            List<WX_COURSE_HISTORY_MM> courseHistoryMonthList = new List<WX_COURSE_HISTORY_MM>();
            WX_COURSE_HISTORY_MM courseHistoryMonth = null;

            string lastMonth = string.Empty;
            for (int i = 0; i < courseCount; i++)
            {
                var myCourse = courseList[i];
                // 按月分组
                string curMonth = myCourse.CourseDate.ToString("yyyy-MM");
                if (lastMonth != curMonth)
                {
                    if (!String.IsNullOrEmpty(lastMonth))
                    {
                        // 不是第一条数据。  将上一月的数据存入list
                        courseHistoryMonthList.Add(courseHistoryMonth);
                        courseHistory = new WX_COURSE_HISTORY
                        {
                            yyyymm = lastMonth,
                            courses = courseHistoryMonthList
                        };
                        courseHistoryMonthList = new List<WX_COURSE_HISTORY_MM>();
                        courseHistoryList.Add(courseHistory);
                    }
                    else
                    {
                        // 第一条数据  do nothing
                    }
                }
                else
                {
                    // 相同月
                    courseHistoryMonthList.Add(courseHistoryMonth);
                }

                // 获取作品
                #region
                IEnumerable<StudentArtwork> artworkList = _chuxinQuery.GetArtworkByCourse(myCourse.StudentCourseId);
                List<WX_COURSE_HISTORY_ARTWORK> realArtworks = new List<WX_COURSE_HISTORY_ARTWORK>();
                foreach (var myArtwork in artworkList)
                {
                    WX_COURSE_HISTORY_ARTWORK art = new WX_COURSE_HISTORY_ARTWORK
                    {
                        artworkUrl = accessUrlHost + "api/upload/getimage?id=" + myArtwork.ArtworkId + "&type=artwork-wx"
                    };
                    realArtworks.Add(art);
                }
                #endregion

                if (String.IsNullOrEmpty(myCourse.CourseSubject))
                {
                    myCourse.CourseSubject = "暂未填写";
                }
                // 构造上课记录
                courseHistoryMonth = new WX_COURSE_HISTORY_MM
                {
                    courseDay = myCourse.CourseDate.ToString("dd") + "号",
                    courseWeekday = myCourse.CourseWeekDay,
                    courseSubject = myCourse.CourseSubject,
                    coursePeriod = myCourse.CoursePeriod,
                    courseArtworks = realArtworks
                };


                if (i == courseList.Count - 1)
                {
                    courseHistoryMonthList.Add(courseHistoryMonth);
                    courseHistory = new WX_COURSE_HISTORY
                    {
                        yyyymm = curMonth,
                        courses = courseHistoryMonthList
                    };
                    courseHistoryMonthList = new List<WX_COURSE_HISTORY_MM>();
                    courseHistoryList.Add(courseHistory);
                }

                lastMonth = curMonth;
            }

            return new JsonResult(new
            {
                totalCount = courseCount,
                courseList = courseHistoryList
            });
        }

        /// <summary>
        /// 【弃用】获取学生的课程作品 GET api/wxopen/getartworklist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public IEnumerable<ART_WORK_R_LIST> GetArtworkList(string studentCode, int pageIndex, int pageSize)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StudentArtwork, ART_WORK_R_LIST>();
            });
            IMapper mapper = config.CreateMapper();

            IEnumerable<StudentArtwork> artworks = _chuxinQuery.GetArkworkByStudent(studentCode, pageIndex, pageSize);

            List<ART_WORK_R_LIST> artWorkList = new List<ART_WORK_R_LIST>();
            ART_WORK_R_LIST aw = null;

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (var artwork in artworks)
            {
                aw = mapper.Map<StudentArtwork, ART_WORK_R_LIST>(artwork);
                aw.ShowURL = accessUrlHost + "api/upload/getimage?id=" + artwork.ArtworkId + "&type=artwork-wx";

                artWorkList.Add(aw);
            }

            return artWorkList;
        }

        /// <summary>
        /// [微信学员详细信息主页面] GET api/wxopen/getstudenthomepage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> getStudentDetail(string studentCode)
        {
            WX_MINE_OVERVIEW overView = null;
            IEnumerable<StudentCourseList> myWeekCourse = null;
            IEnumerable<StudentCoursePackage> myPackage = null;

            Student student = _chuxinQuery.GetStudentByCode(studentCode);
            if (student != null)
            {
                DataTable dtRestCourseCount = _chuxinQuery.GetRestCourseCountByCategorty(studentCode);
                foreach (DataRow dr in dtRestCourseCount.Rows)
                {
                    dr["course_category_name"] = dr["course_category_name"].ToString() + "剩余课时";
                }
                string strCourseInfo = JsonConvert.SerializeObject(dtRestCourseCount);
                int artWorkCount = _chuxinQuery.GetStudentArkworkCount(studentCode);

                string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
                overView = new WX_MINE_OVERVIEW
                {
                    studentName = student.StudentName,
                    studentAvatarPath = accessUrlHost + "api/upload/getimage?id=" + student.Id + "&type=avatar-s-wx",
                    studentBirthday = student.StudentBirthday,
                    studentSex = student.StudentSex,
                    studentPhone = student.StudentPhone,
                    studentAddress = student.StudentAddress,
                    studentArtworkCount = artWorkCount,
                    studentCourseOverview = strCourseInfo
                };

                myWeekCourse = GetStudentWeekCourse(studentCode);
                myPackage = GetStudentPackages(studentCode);
            }
            return new JsonResult(new
            {
                overView = overView,
                weekCourse = myWeekCourse,
                allPackage = myPackage
            });
        }

        /// <summary>
        /// [本周过生日的学生列表] GET api/wxopen/getallstudents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetAllStudents(int pageIndex, int pageSize, string q)
        {
            WX_QUERY_STUDENT query = JsonConvert.DeserializeObject<WX_QUERY_STUDENT>(q);
            DataTable dtStudents = _chuxinQuery.GetStudentList(pageIndex, pageSize, query);

            int totalCount = dtStudents.Rows.Count;
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (DataRow dr in dtStudents.Rows)
            {
                string id = dr["id"].ToString();
                string studentCode = dr["student_code"].ToString();
                if (dr["student_avatar_path"] != null)
                {
                    dr["student_avatar_path"] = accessUrlHost + "api/upload/getimage?id=" + id + "&type=avatar-s-wx";
                }

                DataTable dtRestCourseCount = _chuxinQuery.GetRestCourseCountByCategorty(studentCode);

                string strCourseInfo = JsonConvert.SerializeObject(dtRestCourseCount);
                dr["rest_course_info"] = strCourseInfo;
            }

            string strStudents = JsonConvert.SerializeObject(dtStudents);

            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = strStudents
            });
        }

        /// <summary>
        /// [本周过生日的学生列表] GET api/wxopen/getstudentstobirth
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetStudentsToBirth(int pageIndex, int pageSize)
        {
            DataTable dt = _chuxinQuery.GetBirthdayIn7Days(pageIndex, pageSize);

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["id"].ToString();
                string studentCode = dr["student_code"].ToString();
                if (dr["student_avatar_path"] != null)
                {
                    dr["student_avatar_path"] = accessUrlHost + "api/upload/getimage?id=" + id + "&type=avatar-s-wx";
                }
                dr["rest_course_count"] = _chuxinQuery.getStudentRestCourseCount(studentCode);
            }

            int totalCount = dt.Rows.Count;
            string strStudentList = JsonConvert.SerializeObject(dt);

            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = strStudentList
            });
        }

        /// <summary>
        /// [套餐即将到期的学生列表（学员+套餐））] GET api/wxopen/getstudentstoexpiration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetStudentsToExpiration(int pageIndex, int pageSize)
        {
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");

            DataTable dt = _chuxinQuery.GetExpirationStudents(pageIndex, pageSize);
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["id"].ToString();
                dr["student_avatar_path"] = accessUrlHost + "api/upload/getimage?id=" + id + "&type=avatar-s-wx";
            }
            int totalCount = dt.Rows.Count;
            string strStudentList = JsonConvert.SerializeObject(dt);
            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = strStudentList
            });
        }

        /// <summary>   
        /// 获取学员本周的课程安排 GET api/wxopen/getstudentweekcourse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public List<StudentCourseList> GetStudentWeekCourse(string studentCode)
        {
            DateTime weekLastDay = DateTime.Now.AddDays(7 - Convert.ToInt16(DateTime.Now.DayOfWeek));
            List<StudentCourseList> scls = _chuxinQuery.GetStudentWeekCourse(studentCode, weekLastDay);
            return scls;
        }

        /// <summary>
        /// [获取学员所有套餐] GET api/wxopen/getstudentpackages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public IEnumerable<StudentCoursePackage> GetStudentPackages(string studentCode)
        {
            return _chuxinQuery.GetStudentCoursePackage(studentCode);
        }
    }
}