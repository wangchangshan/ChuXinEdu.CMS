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
        public ActionResult<string> Get(string type)
        {
            HomeGroupChat hgc = new HomeGroupChat();
            switch (type)
            {
                case "dashboard":
                    hgc.student = GetStudentClassifyForHome();
                    hgc.course = GetCourseClassifyForHome();
                    hgc.trialStudent = GetTrialStudentClassifyForHome();
                    hgc.income = GetIncomeClassifyForHome();
                break;

                default:
                break;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            string resultJson = JsonConvert.SerializeObject(hgc,settings);
            return resultJson;
        }

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

            if(dtTotal == null || dtMeishu == null || dtShufa == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while(beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
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

            if(dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while(beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
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

            if(dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while(beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
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

            if(dtTotal == null)
            {
                return cs;
            }

            DateTime beginDate = new DateTime(2017, 12, 31);
            int curY = DateTime.Now.Year;
            int curM = DateTime.Now.Month;
            while(beginDate <= DateTime.Now || (beginDate.Year == curY && beginDate.Month == curM))
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
                if(dr["ym"].ToString() == ym)
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
                if(dr["ym"].ToString() == ym && dr["type"].ToString() == type)
                {
                    amount = Int32.Parse(dr["amount"].ToString().Split(".")[0]);
                    dt.Rows.Remove(dr);
                    break;
                }
            }
            return amount;
        }
    }

    class HomeGroupChat
    {
        public ClassifyStatistic student{ get; set; }
        public ClassifyStatistic course{ get; set; }
        public ClassifyStatistic trialStudent{ get; set; }
        public ClassifyStatistic income{ get; set; }
    }

    class ClassifyStatistic
    {
        public List<string> xMonth{ get; set; }

        public List<int> yMeishu{ get; set; }

        public List<int> yShufa{ get; set; }

        public List<int> yTotal{ get; set; }
    }
}