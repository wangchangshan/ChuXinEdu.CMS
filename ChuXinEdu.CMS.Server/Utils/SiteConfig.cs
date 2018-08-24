using Microsoft.Extensions.Configuration;

namespace ChuXinEdu.CMS.Server.Utils
{
    public class SiteConfig
    {
        private static IConfigurationSection _appSection = null;

        public static string AppSetting(string key)
        {
            string str = string.Empty;
            if(_appSection.GetSection(key) != null)
            {
                str = _appSection.GetSection(key).Value;
            }
            return str;
        }

        public static void SetAppSetting(IConfigurationSection section)
        {
            _appSection = section;
        }

        public static string GetSite(string apiName)
        {
            return AppSetting(apiName);
        }
    }
}