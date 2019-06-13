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
using System.Drawing;

namespace ChuXinEdu.CMS.Server.Controllers
{
    /// <summary>
    /// 用于日常数据迁移、历史数据处理的开放接口。调用结束后添加BlockFiler过滤器关闭此接口。
    /// </summary>
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class OpenController : ControllerBase
    {
        private readonly IChuXinQuery _chuxinQuery;
        private readonly IChuXinWorkFlow _chuxinWorkFlow;
        private readonly IHostingEnvironment _hostingEnvironment;

        public OpenController(IChuXinQuery chuxinQuery, IChuXinWorkFlow chuxinWorkFlow, IHostingEnvironment hostingEnvironment)
        {
            _chuxinQuery = chuxinQuery;
            _chuxinWorkFlow = chuxinWorkFlow;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// [配置] arrange_guid 数据迁移接口 GET api/open/guidmigrate
        /// 修改表结构，添加guid字段时的历史数据迁移 目前不在使用，添加了MyAuthenFilter过滤器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BlockFilter]
        public string GuidMigrate()
        {
            string result = "1200";
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

        /// <summary>
        /// GET http://localhost:5000/api/open/generateweixinavatar
        /// 为历史图片生成微信小程序用到的头像， 尺寸为60X60
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BlockFilter]
        public string GenerateWeiXinAvatar()
        {
            string result = "1200";
            try
            {
                using (BaseContext context = new BaseContext())
                {
                    var students = context.Student.Where(s => s.StudentAvatarPath != null).ToList();
                    foreach (Student student in students)
                    {
                        string avatarPath = _hostingEnvironment.ContentRootPath + student.StudentAvatarPath;
                        if (System.IO.File.Exists(avatarPath))
                        {
                            using (Stream sm = System.IO.File.OpenRead(avatarPath))
                            {
                                Bitmap bitmap = new Bitmap(Bitmap.FromStream(sm));
                                ImageHelper.SaveThumbnailImage(bitmap, avatarPath, 60, 60);
                            }
                            // 原来上传的图片为300X300，现在修改为200X200
                            ImageHelper.ChangeImageSize(avatarPath, 200, 200);
                        }
                    }

                    var teachers = context.Teacher.Where(t => t.TeacherAvatarPath != null).ToList();
                    foreach (Teacher teacher in teachers)
                    {
                        string avatarPath = _hostingEnvironment.ContentRootPath + teacher.TeacherAvatarPath;
                        if (System.IO.File.Exists(avatarPath))
                        {
                            using (Stream sm = System.IO.File.OpenRead(avatarPath))
                            {
                                Bitmap bitmap = new Bitmap(Bitmap.FromStream(sm));
                                ImageHelper.SaveThumbnailImage(bitmap, avatarPath, 60, 60);
                            }
                            // 原来上传的图片为300X300，现在修改为200X200
                            ImageHelper.ChangeImageSize(avatarPath, 200, 200);
                        }
                    }
                }
            }
            catch { result = "1500"; }
            return result;
        }
    }
}