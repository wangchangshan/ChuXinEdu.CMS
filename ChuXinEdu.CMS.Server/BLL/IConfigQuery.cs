using System;
using System.Data;
using System.Collections.Generic;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.ViewModel;

namespace ChuXinEdu.CMS.Server.BLL
{
    /// <summary>
	/// This interface will define all the query function, so the data query from UI must call this interface
	/// </summary>
    public interface IConfigQuery
    {
        IEnumerable<DIC_R_KEY_VALUE> GetDicByCode(string typeCode);

        IEnumerable<DIC_R_KEY_VALUE> GetSysCoursePackage(string categoryCode);

        IEnumerable<StudentCourseArrange> GetArrangeDirty();
        
        IEnumerable<StudentCoursePackage> GetPackageDirty();
    }
}