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
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class WxOpenController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigQuery _configQuery;
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;

        public WxOpenController(IHostingEnvironment hostingEnvironment, IConfigQuery configQuery, IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow)
        {
            _hostingEnvironment = hostingEnvironment;
            _configQuery = configQuery;
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
        public ActionResult<string> GetWxBestDraw(string wxPicCode, int pageIndex, int pageSize)
        {
            DataTable dt = _chuxinQuery.GetWxPictureDT(wxPicCode, pageIndex, pageSize);

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["studentId"] != null && dr["studentId"] != DBNull.Value)
                {
                    dr["avatarPath"] = accessUrlHost + "api/upload/getimage?id=" + dr["studentId"].ToString() + "&type=avatar-s-wx";
                }
                else
                {
                    // 取默认头像
                    dr["avatarPath"] = accessUrlHost + "api/upload/getimage?id=0&type=avatar-s-wx";
                }
                dr["picturePath"] = accessUrlHost + "api/upload/getimage?id=" + dr["id"].ToString() + "&type=ad-wx";
            }

            return JsonConvert.SerializeObject(dt);
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
        /// 获取某天排课信息 GET api/wxopen/getwxschedule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public List<WX_SCHEDULE> GetWxSchedule(string day)
        {
            DateTime theDay = Convert.ToDateTime(day).Date;
            DataTable dt = _chuxinQuery.GetScheduleByDay(theDay);

            List<WX_SCHEDULE> scheduleList = new List<WX_SCHEDULE>();
            WX_SCHEDULE schedule = null;
            List<WX_SCHEDULE_ROOM> scheduleRoomList = new List<WX_SCHEDULE_ROOM>();
            WX_SCHEDULE_ROOM scheduleRoom = null;
            List<WX_SCHEDULE_DETAIL> scheduleDetailList = new List<WX_SCHEDULE_DETAIL>();
            WX_SCHEDULE_DETAIL scheduleDetail = null;

            DataView view = new DataView(dt);
            DataTable distinctPeriod = view.ToTable(true, "course_period");
            DataTable distinctRoom = view.ToTable(true, "course_period", "classroom_name");

            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            foreach (DataRow drPeriod in distinctPeriod.Rows)
            {
                string period = drPeriod["course_period"].ToString();
                DataRow[] drRooms = distinctRoom.Select("course_period = '" + period + "'");
                foreach (DataRow drRoom in drRooms)
                {
                    string room = drRoom["classroom_name"].ToString();
                    DataRow[] temps = dt.Select("course_period = '" + period + "' and classroom_name='" + room + "'");
                    foreach (DataRow temp in temps)
                    {
                        scheduleDetail = new WX_SCHEDULE_DETAIL
                        {
                            id = Int32.Parse(temp["id"].ToString()),
                            studentCode = temp["student_code"].ToString(),
                            studentName = temp["student_name"].ToString(),
                            studentAvatarPath = accessUrlHost + "api/upload/getimage?id=" + temp["id"].ToString() + "&type=avatar-s-wx",
                            courseWeekDay = temp["course_week_day"].ToString(),
                            attendanceCode = temp["attendance_status_code"].ToString(),
                            attendanceName = temp["attendance_status_name"].ToString()
                        };
                        scheduleDetailList.Add(scheduleDetail);
                    }
                    scheduleRoom = new WX_SCHEDULE_ROOM
                    {
                        classroom = room,
                        scheduleDetail = scheduleDetailList
                    };
                    scheduleDetailList = new List<WX_SCHEDULE_DETAIL>();
                    scheduleRoomList.Add(scheduleRoom);
                }
                schedule = new WX_SCHEDULE
                {
                    coursePeriod = period,
                    roomSchedule = scheduleRoomList
                };
                scheduleRoomList = new List<WX_SCHEDULE_ROOM>();
                scheduleList.Add(schedule);
            }

            // 审核测试
            // scheduleList = new List<WX_SCHEDULE>();
            return scheduleList;
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
        /// 获取学员的课程作品 GET api/wxopen/getstudentartworks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetStudentArtworks(string studentCode, int pageIndex, int pageSize)
        {
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");
            List<StudentArtwork> myArtworks = _chuxinQuery.GetArkworkByStudent(studentCode, pageIndex, pageSize);
            int myArtworkCount = myArtworks.Count;

            List<WX_ARTWORK_HISTORY> artWorkList = new List<WX_ARTWORK_HISTORY>();
            WX_ARTWORK_HISTORY artWork = null;

            List<WX_ARTWORK_HISTORY_DETAIL> artWorkDetailList = new List<WX_ARTWORK_HISTORY_DETAIL>();
            WX_ARTWORK_HISTORY_DETAIL artWorkDetail = null;

            string lastMonth = string.Empty;
            for (int i = 0; i < myArtworkCount; i++)
            {
                StudentArtwork myArtwork = myArtworks[i];
                // 按月分组
                string curMonth = myArtwork.CreateDate.ToString("yyyy-MM");
                if (lastMonth != curMonth)
                {
                    if (!String.IsNullOrEmpty(lastMonth))
                    {
                        // 不是第一条数据。  将上一月的数据存入list
                        artWorkDetailList.Add(artWorkDetail);
                        artWork = new WX_ARTWORK_HISTORY
                        {
                            yyyymm = lastMonth,
                            artworks = artWorkDetailList
                        };
                        artWorkDetailList = new List<WX_ARTWORK_HISTORY_DETAIL>();
                        artWorkList.Add(artWork);
                    }
                    else
                    {
                        // 第一条数据  do nothing
                    }
                }
                else
                {
                    // 相同月
                    artWorkDetailList.Add(artWorkDetail);
                }

                artWorkDetail = new WX_ARTWORK_HISTORY_DETAIL
                {
                    artworkTitle = myArtwork.ArtworkTitle,
                    artworkUrl = accessUrlHost + "api/upload/getimage?id=" + myArtwork.ArtworkId + "&type=artwork-wx"
                };

                if (i == myArtworks.Count - 1)
                {
                    artWorkDetailList.Add(artWorkDetail);
                    artWork = new WX_ARTWORK_HISTORY
                    {
                        yyyymm = curMonth,
                        artworks = artWorkDetailList
                    };
                    artWorkList.Add(artWork);
                }

                lastMonth = curMonth;
            }

            return new JsonResult(new
            {
                totalCount = myArtworkCount,
                artworkList = artWorkList
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
        /// [所有学员列表] GET api/wxopen/getallstudents
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

            //string strStudents = JsonConvert.SerializeObject(dtStudents);
            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = dtStudents
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
            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = dt
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
            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = dt
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

        /// <summary>
        /// [获取所有的班级] GET api/wxopen/getclassrooms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public IEnumerable<DIC_R_KEY_VALUE> GetClassrooms()
        {
            var classrooms = _configQuery.GetDicByCode("classroom");
            foreach (var room in classrooms)
            {
                room.Label = _chuxinQuery.GetCoursesToSignInCount(room.Value).ToString();
            }
            return classrooms;
        }

        /// <summary>
        /// [获取待签到的学员列表] GET api/wxopen/getcoursestosignin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WxAuthenFilter]
        public ActionResult<string> GetCoursesToSignin(string roomCode, int pageIndex, int pageSize)
        {
            List<WX_SIGNIN_LIST> wxSignInList = new List<WX_SIGNIN_LIST>();
            var signCourses = _chuxinQuery.GetCoursesToSignIn(roomCode, pageIndex, pageSize);

            int totalCount = signCourses.ToList().Count;
            DataTable dt = _chuxinQuery.GetSignTimeCategory(roomCode);
            foreach (DataRow dr in dt.Rows)
            {
                var tempCourses = signCourses.Where(s => s.CourseDate == Convert.ToDateTime(dr["course_date"])
                                        && s.CoursePeriod == dr["course_period"].ToString()
                ).ToList();

                if (tempCourses.Count == 0)
                {
                    continue;
                }
                WX_SIGNIN_LIST wxSign = new WX_SIGNIN_LIST
                {
                    courseDate = Convert.ToDateTime(dr["course_date"]),
                    coursePeriod = dr["course_period"].ToString(),
                    courseWeekday = tempCourses[0].CourseWeekDay,
                    signCourses = tempCourses
                };
                wxSignInList.Add(wxSign);
            }

            return new JsonResult(new
            {
                TotalCount = totalCount,
                Data = wxSignInList
            });
        }

        /// <summary>
        /// 微信课程请假 POST api/wxopen/wxcourseqingjia
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [WxAuthenFilter]
        public string WxCourseQingJia(dynamic obj)
        {
            int studentCourseId = obj.StudentCourseId;
            string result = _chuxinWorkFlow.SingleQingJia(studentCourseId);

            return result;
        }

        /// <summary>
        /// 微信学员签到 Post api/wxopen/wxstudentsignin
        /// 微信销课 不修改当前课程的课程小类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [WxAuthenFilter]
        public string WxStudentSignIn(CL_U_SIGN_IN course)
        {
            // teacher code中存储得skey, 获取教师信息。
            SysWxUser curTeacher = _chuxinQuery.GetWxUserBySKey(course.TeacherCode);
            course.TeacherCode = curTeacher.InnerPersonCode;
            course.TeacherName = curTeacher.InnerPersonName;

            string result = _chuxinWorkFlow.SignInSingle(course);

            return result;
        }

        /// <summary>
        /// 微信签到 上传作品 POST api/wxopen/uploadartwork
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [WxAuthenFilter]
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

            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string documentPath = "/cxdocs/" + studentCode + "/";

            if (!Directory.Exists(contentRootPath + documentPath))
            {
                Directory.CreateDirectory(contentRootPath + documentPath);
            }

            var file = HttpContext.Request.Form.Files["wx_sign_image"];
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
        /// [获取教师作品用于教师简历] GET api/wxopen/getteacherartwork
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<string> GetTeacherArtwork(string teacherCode)
        {
            string accessUrlHost = CustomConfig.GetSetting("AccessUrl");

            List<string> list = new List<string>();
            return list;
        }
    }
}