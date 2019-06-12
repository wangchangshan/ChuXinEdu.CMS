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
    }
}