using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ChuXinEdu.CMS.Server.Context;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.BLLService;
using ChuXinEdu.CMS.Server.Utils;

namespace ChuXinEdu.CMS.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // mysql 数据连接配置
            services.AddDbContext<MyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // 注入服务
            services.AddTransient<IChuXinQuery, ChuXinQuery>(); //每一次GetService都会创建一个新的实例
            //services.AddSingleton<IChuXinQuery, ChuXinQuery>(); //整个应用程序生命周期以内只创建一个实例 
            //services.AddScoped<IChuXinQuery, ChuXinQuery>(); //在同一个Scope内只初始化一个实例 ，可以理解为（ 每一个request级别只创建一个实例，同一个http request会在一个 scope内）

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // 绑定自定义配置
            SiteConfig.SetAppSetting(Configuration.GetSection("CustomSetting"));
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
