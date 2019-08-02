using System.Data;
using System.Collections.Generic;

namespace ChuXinEdu.CMS.Server.BLL
{
    public interface IChuXinStatistics
    {
        DataTable GetCourseDistribution();
        DataTable GetStudentDistribution();
        DataTable GetStudentDistribution_meishu();
        DataTable GetStudentDistribution_shufa();
        DataTable GetTrialStudentDistribution();
        DataTable GetIncomeDistribution();
        DataTable GetTotalIncome();
        IDictionary<string, decimal> GetTotalActualIncome();
        DataTable GetAllDistribution(string begin, string end);
    }
}