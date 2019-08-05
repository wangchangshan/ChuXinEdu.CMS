using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.BLL;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;
using System.Linq;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Utils;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IChuXinStatistics _chuxinStatistic;
        private readonly IConfigQuery _configQuery;

        public StatisticsController(IChuXinStatistics chuxinStatistic, IConfigQuery configQuery)
        {
            _chuxinStatistic = chuxinStatistic;
            _configQuery = configQuery;
        }

        // GET api/statistics/type
        [HttpGet("{type}")]
        public ActionResult<string> Get(string type, [FromQuery] string range)
        {
            string resultJson = string.Empty;

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            switch (type)
            {
                case "dashboard":
                    {
                        var categories = _configQuery.GetDicByCode("course_category");
                        DateTime beginDate = DateTime.Parse(CustomConfig.GetSetting("StatisticsStartDate") ?? "2017-12-31");

                        HomeGroupChat hgc = new HomeGroupChat();
                        hgc.student = GetStudentClassifyForHome(beginDate, categories);
                        hgc.course = GetCourseClassifyForHome(beginDate, categories); // 销课大类分布
                        hgc.trialStudent = GetTrialStudentClassifyForHome(beginDate, categories);
                        hgc.income = GetIncomeClassifyForHome(beginDate, categories);
                        resultJson = JsonConvert.SerializeObject(hgc, settings);
                        break;
                    }
                case "totalsignupincome":
                    {
                        DataTable dtTotalSignUp = _chuxinStatistic.GetTotalIncome();
                        resultJson = JsonConvert.SerializeObject(dtTotalSignUp, settings);
                        break;
                    }

                case "totalactualincome":
                    {
                        IDictionary<string, decimal> actualIncome = _chuxinStatistic.GetTotalActualIncome();
                        DataTable dtActualCourseIncome = new DataTable();
                        dtActualCourseIncome.Columns.Add("name");
                        dtActualCourseIncome.Columns.Add("value", typeof(decimal));

                        foreach (var income in actualIncome)
                        {
                            DataRow dr = dtActualCourseIncome.NewRow();
                            dr["name"] = income.Key;
                            dr["value"] = income.Value;

                            dtActualCourseIncome.Rows.Add(dr);
                        }

                        resultJson = JsonConvert.SerializeObject(dtActualCourseIncome, settings);
                        break;
                    }

                case "coursedistribution": //销课分布
                    {
                        string begin = DateTime.Now.ToString("yyyy-MM");
                        string end = begin;
                        if (!String.IsNullOrEmpty(range))
                        {
                            begin = range.Split(",")[0];
                            end = range.Split(",")[1];
                        }
                        CourseDistribution cd = GetCoursedistribution(begin, end);
                        resultJson = JsonConvert.SerializeObject(cd, settings);
                        break;
                    }

                default:
                    break;
            }

            return resultJson;
        }

        #region dashboard line chart
        private ClassifyStatistic GetStudentClassifyForHome(DateTime beginDate, IEnumerable<DIC_R_KEY_VALUE> categories)
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.yTotal = new List<int>();
            cs.courseCategory = new List<CourseCategory>();

            DataTable dtTotal = _chuxinStatistic.GetStudentDistribution();
            if (dtTotal == null)
            {
                return cs;
            }
            DateTime date1 = beginDate;
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (date1 <= DateTime.Now || (date1.Year == curY && date1.Month == curM))
            {
                string ym = date1.ToString("yyyy-MM");
                int total = GetAmountByYm(dtTotal, ym);
                cs.xMonth.Add(ym);
                cs.yTotal.Add(total);

                date1 = date1.AddMonths(1);
            }

            DataTable dtCategory = null;

            foreach (DIC_R_KEY_VALUE c in categories)
            {
                dtCategory = _chuxinStatistic.GetStudentDistribution(c.Value);
                if (dtCategory == null)
                {
                    continue;
                }
                CourseCategory category = new CourseCategory();
                category.name = c.Text;
                category.sum = new List<int>();

                DateTime date2 = beginDate;
                curY = DateTime.Now.Year;
                curM = DateTime.Now.Month;
                while (date2 <= DateTime.Now || (date2.Year == curY && date2.Month == curM))
                {
                    string ym = date2.ToString("yyyy-MM");
                    int ymCount = GetAmountByYm(dtCategory, ym);
                    category.sum.Add(ymCount);

                    date2 = date2.AddMonths(1);
                }

                cs.courseCategory.Add(category);
            }
            return cs;
        }

        private ClassifyStatistic GetCourseClassifyForHome(DateTime beginDate, IEnumerable<DIC_R_KEY_VALUE> categories)
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.courseCategory = new List<CourseCategory>();
            cs.yTotal = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetCourseDistribution();

            if (dtTotal == null)
            {
                return cs;
            }
            DateTime date1 = beginDate;
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (date1 <= DateTime.Now || (date1.Year == curY && date1.Month == curM))
            {
                string ym = date1.ToString("yyyy-MM");
                int ymCount = GetAmountByYm(dtTotal, ym, "TOTAL");

                cs.xMonth.Add(ym);
                cs.yTotal.Add(ymCount);

                date1 = date1.AddMonths(1);
            }

            foreach (DIC_R_KEY_VALUE c in categories)
            {
                CourseCategory category = new CourseCategory();
                category.name = c.Text;
                category.sum = new List<int>();

                DateTime date2 = beginDate;
                curY = DateTime.Now.Year;
                curM = DateTime.Now.Month;
                while (date2 <= DateTime.Now || (date2.Year == curY && date2.Month == curM))
                {
                    string ym = date2.ToString("yyyy-MM");
                    int ymCount = GetAmountByYm(dtTotal, ym, c.Value);
                    category.sum.Add(ymCount);

                    date2 = date2.AddMonths(1);
                }

                cs.courseCategory.Add(category);
            }

            return cs;
        }

        private ClassifyStatistic GetTrialStudentClassifyForHome(DateTime beginDate, IEnumerable<DIC_R_KEY_VALUE> categories)
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.courseCategory = new List<CourseCategory>();
            cs.yTotal = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetTrialStudentDistribution();

            if (dtTotal == null)
            {
                return cs;
            }

            DateTime date1 = beginDate;
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (date1 <= DateTime.Now || (date1.Year == curY && date1.Month == curM))
            {
                string ym = date1.ToString("yyyy-MM");
                int ymCount = GetAmountByYm(dtTotal, ym, "TOTAL");

                cs.xMonth.Add(ym);
                cs.yTotal.Add(ymCount);

                date1 = date1.AddMonths(1);
            }

            foreach (DIC_R_KEY_VALUE c in categories)
            {
                CourseCategory category = new CourseCategory();
                category.name = c.Text;
                category.sum = new List<int>();

                DateTime date2 = beginDate;
                curY = DateTime.Now.Year;
                curM = DateTime.Now.Month;
                while (date2 <= DateTime.Now || (date2.Year == curY && date2.Month == curM))
                {
                    string ym = date2.ToString("yyyy-MM");
                    int ymCount = GetAmountByYm(dtTotal, ym, c.Value);
                    category.sum.Add(ymCount);

                    date2 = date2.AddMonths(1);
                }

                cs.courseCategory.Add(category);
            }
            return cs;
        }

        private ClassifyStatistic GetIncomeClassifyForHome(DateTime beginDate, IEnumerable<DIC_R_KEY_VALUE> categories)
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.courseCategory = new List<CourseCategory>();
            cs.yTotal = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetIncomeDistribution();

            if (dtTotal == null)
            {
                return cs;
            }

            DateTime date1 = beginDate;
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (date1 <= DateTime.Now || (date1.Year == curY && date1.Month == curM))
            {
                string ym = date1.ToString("yyyy-MM");
                int ymCount = GetAmountByYm(dtTotal, ym, "TOTAL");

                cs.xMonth.Add(ym);
                cs.yTotal.Add(ymCount);

                date1 = date1.AddMonths(1);
            }

            foreach (DIC_R_KEY_VALUE c in categories)
            {
                CourseCategory category = new CourseCategory();
                category.name = c.Text;
                category.sum = new List<int>();

                DateTime date2 = beginDate;
                curY = DateTime.Now.Year;
                curM = DateTime.Now.Month;
                while (date2 <= DateTime.Now || (date2.Year == curY && date2.Month == curM))
                {
                    string ym = date2.ToString("yyyy-MM");
                    int ymCount = GetAmountByYm(dtTotal, ym, c.Value);
                    category.sum.Add(ymCount);

                    date2 = date2.AddMonths(1);
                }

                cs.courseCategory.Add(category);
            }
            return cs;
        }

        private int GetAmountByYm(DataTable dt, string ym)
        {
            int amount = 0;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ym"].ToString() == ym)
                {
                    amount = Int32.Parse(dr["amount"].ToString());
                    dt.Rows.Remove(dr);
                    break;
                }
            }
            return amount;
        }

        private int GetAmountByYm(DataTable dt, string ym, string type)
        {
            int amount = 0;

            foreach (DataRow dr in dt.Rows)
            {
                if (type == "TOTAL")
                {
                    if (dr["ym"].ToString() == ym)
                    {
                        amount += Int32.Parse(dr["amount"].ToString().Split(".")[0]);
                    }
                }
                else if (dr["ym"].ToString() == ym && dr["type"].ToString() == type)
                {
                    amount = Int32.Parse(dr["amount"].ToString().Split(".")[0]);
                    dt.Rows.Remove(dr);
                    break;
                }
            }
            return amount;
        }
        #endregion  

        #region Bar chart  
        private CourseDistribution GetCoursedistribution(string begin, string end)
        {
            CourseDistribution cd = new CourseDistribution();
            cd.xMonth = new List<string>();
            cd.courseFolder = new List<CourseFolder>();

            DataTable dtDistribution = _chuxinStatistic.GetAllDistribution(begin, end);

            if (dtDistribution == null)
            {
                return cd;
            }

            DateTime beginDate = Convert.ToDateTime(begin);
            DateTime endDate = Convert.ToDateTime(end);

            while (beginDate <= endDate)
            {
                string ym = beginDate.ToString("yyyy-MM");
                cd.xMonth.Add(ym);
                beginDate = beginDate.AddMonths(1);
            }

            var folders = _configQuery.GetDicByCode("course_folder").OrderBy(f => f.Value);
            foreach (DIC_R_KEY_VALUE f in folders)
            {
                CourseFolder folder = new CourseFolder();
                folder.name = f.Text;
                folder.sum = new List<int>();

                beginDate = Convert.ToDateTime(begin);
                endDate = Convert.ToDateTime(end);

                while (beginDate <= endDate)
                {
                    string ym = beginDate.ToString("yyyy-MM");
                    int courseCount = GetCourseCount(dtDistribution, f.Value, ym);
                    folder.sum.Add(courseCount);

                    beginDate = beginDate.AddMonths(1);
                }

                cd.courseFolder.Add(folder);
            }
            return cd;
        }

        private int GetCourseCount(DataTable dt, string code, string ym)
        {
            int count = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["course_folder_code"].ToString() == code && dr["ym"].ToString() == ym)
                {
                    count = Int32.Parse(dr["course_count"].ToString());
                    dt.Rows.Remove(dr);
                    break;
                }
            }
            return count;
        }
        #endregion 
    }

    class HomeGroupChat
    {
        public ClassifyStatistic student { get; set; }
        public ClassifyStatistic course { get; set; }
        public ClassifyStatistic trialStudent { get; set; }
        public ClassifyStatistic income { get; set; }
    }

    class ClassifyStatistic
    {
        public List<string> xMonth { get; set; }

        public List<CourseCategory> courseCategory { get; set; }

        public List<int> yTotal { get; set; }
    }

    class CourseCategory
    {
        public string name { get; set; }
        public List<int> sum { get; set; }
    }

    class CourseDistribution
    {
        public List<string> xMonth { get; set; }

        public List<CourseFolder> courseFolder { get; set; }
    }

    class CourseFolder
    {
        public string name { get; set; }
        public List<int> sum { get; set; }
    }
}