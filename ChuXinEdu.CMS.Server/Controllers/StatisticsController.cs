using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using ChuXinEdu.CMS.Server.BLL;
using Newtonsoft.Json.Serialization;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("any")]
    [MyAuthenFilter]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IChuXinStatistics _chuxinStatistic;

        public StatisticsController(IChuXinStatistics chuxinStatistic)
        {
            _chuxinStatistic = chuxinStatistic;
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
                        HomeGroupChat hgc = new HomeGroupChat();
                        hgc.student = GetStudentClassifyForHome();
                        hgc.course = GetCourseClassifyForHome();
                        hgc.trialStudent = GetTrialStudentClassifyForHome();
                        hgc.income = GetIncomeClassifyForHome();
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
        private ClassifyStatistic GetStudentClassifyForHome()
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.yTotal = new List<int>();
            cs.yMeishu = new List<int>();
            cs.yShufa = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetStudentDistribution();
            DataTable dtMeishu = _chuxinStatistic.GetStudentDistribution_meishu();
            DataTable dtShufa = _chuxinStatistic.GetStudentDistribution_shufa();

            if (dtTotal == null || dtMeishu == null || dtShufa == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
            {
                string ym = beginDate.ToString("yyyy-MM");
                int meishu = GetAmountByYm(dtMeishu, ym);
                int shufa = GetAmountByYm(dtShufa, ym);
                int total = GetAmountByYm(dtTotal, ym);

                cs.xMonth.Add(ym);
                cs.yMeishu.Add(meishu);
                cs.yShufa.Add(shufa);
                cs.yTotal.Add(total);

                beginDate = beginDate.AddMonths(1);
            }
            return cs;
        }

        private ClassifyStatistic GetCourseClassifyForHome()
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.yTotal = new List<int>();
            cs.yMeishu = new List<int>();
            cs.yShufa = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetCourseDistribution();

            if (dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
            {
                string ym = beginDate.ToString("yyyy-MM");
                int meishu = GetAmountByYm(dtTotal, ym, "meishu");
                int shufa = GetAmountByYm(dtTotal, ym, "shufa");

                cs.xMonth.Add(ym);
                cs.yMeishu.Add(meishu);
                cs.yShufa.Add(shufa);
                cs.yTotal.Add(meishu + shufa);

                beginDate = beginDate.AddMonths(1);
            }
            return cs;
        }

        private ClassifyStatistic GetTrialStudentClassifyForHome()
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.yTotal = new List<int>();
            cs.yMeishu = new List<int>();
            cs.yShufa = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetTrialStudentDistribution();

            if (dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
            {
                string ym = beginDate.ToString("yyyy-MM");
                int meishu = GetAmountByYm(dtTotal, ym, "meishu");
                int shufa = GetAmountByYm(dtTotal, ym, "shufa");

                cs.xMonth.Add(ym);
                cs.yMeishu.Add(meishu);
                cs.yShufa.Add(shufa);
                cs.yTotal.Add(meishu + shufa);

                beginDate = beginDate.AddMonths(1);
            }
            return cs;
        }

        private ClassifyStatistic GetIncomeClassifyForHome()
        {
            ClassifyStatistic cs = new ClassifyStatistic();
            cs.xMonth = new List<string>();
            cs.yTotal = new List<int>();
            cs.yMeishu = new List<int>();
            cs.yShufa = new List<int>();

            DataTable dtTotal = _chuxinStatistic.GetIncomeDistribution();

            if (dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while (beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
            {
                string ym = beginDate.ToString("yyyy-MM");
                int meishu = GetAmountByYm(dtTotal, ym, "meishu");
                int shufa = GetAmountByYm(dtTotal, ym, "shufa");

                cs.xMonth.Add(ym);
                cs.yMeishu.Add(meishu);
                cs.yShufa.Add(shufa);
                cs.yTotal.Add(meishu + shufa);

                beginDate = beginDate.AddMonths(1);
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
                if (dr["ym"].ToString() == ym && dr["type"].ToString() == type)
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
            cd.guohua = new List<int>();
            cd.xihua = new List<int>();
            cd.ruanbi = new List<int>();
            cd.yingbi = new List<int>();

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

                int guohua = GetCourseCount(dtDistribution, "meishu_00", ym);
                int xihua = GetCourseCount(dtDistribution, "meishu_01", ym);
                int ruanbi = GetCourseCount(dtDistribution, "shufa_00", ym);
                int yingbi = GetCourseCount(dtDistribution, "shufa_01", ym);

                cd.xMonth.Add(ym);
                cd.guohua.Add(guohua);
                cd.xihua.Add(xihua);
                cd.ruanbi.Add(ruanbi);
                cd.yingbi.Add(yingbi);

                beginDate = beginDate.AddMonths(1);
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

        public List<int> yMeishu { get; set; }

        public List<int> yShufa { get; set; }

        public List<int> yTotal { get; set; }
    }

    class CourseDistribution
    {
        public List<string> xMonth { get; set; }

        public List<int> guohua { get; set; }

        public List<int> xihua { get; set; }

        public List<int> ruanbi { get; set; }

        public List<int> yingbi { get; set; }
    }
}