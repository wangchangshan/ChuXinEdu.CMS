using System;

namespace ChuXinEdu.CMS.Server.Provider
{
    public class MyServiceProvider
    {
        public static IServiceProvider ServiceProvider
        {
            get; set;
        }
    }
}

/** 
    var context = MyHttpContextClass.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    就可以使用   context.Session等方法了


    string contentRootPath = MyServiceProvider.ServiceProvider.GetRequiredService<IHostingEnvironment>().ContentRootPath;
*/
