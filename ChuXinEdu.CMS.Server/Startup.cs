using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChuXinEdu.CMS.Server.BLL;
using ChuXinEdu.CMS.Server.BLLService;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using ChuXinEdu.CMS.Server.Middlewares;
using ChuXinEdu.CMS.Server.Filters;

namespace ChuXinEdu.CMS.Server
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region mysql 数据连接配置 
            // string conn = _configuration.GetConnectionString("MySqlConnection");
            // services.AddDbContext<ADOContext>(options => options.UseMySql(conn));
            // services.AddDbContext<BaseContext>(options => options.UseMySql(conn));
            #endregion

            #region 支持跨域处理 Cors
            services.AddCors(Options=> {
                Options.AddPolicy("any", builder => {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); //指定处理cookie
                });
            });
            #endregion

            // 添加mvc需要的服务（全局）
            services.AddMvc(options => 
            {
                // 实例注册
                // options.Filters.Add(new MyAuthenFilter());
                options.Filters.Add(new MyReturnValueFilter());
                // 类型注册
                // options.Filters.Add(typeof(MyAuthenFilter)); // 添加自定义过滤器 （全局）
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // 注入自定义服务
            services.AddTransient<IChuXinQuery, ChuXinQuery>(); //每一次GetService都会创建一个新的实例
            services.AddTransient<IChuXinWorkFlow, ChuXinWorkFlow>(); //每一次GetService都会创建一个新的实例
            services.AddTransient<IConfigQuery, ConfigQuery>(); //每一次GetService都会创建一个新的实例
            services.AddTransient<IChuXinStatistics, ChuXinStatistics>(); //每一次GetService都会创建一个新的实例
            
            // 注入NLog.Web需要的服务
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 添加自定义中间件
            app.UseMiddleware<RealIpMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // 绑定自定义配置
            CustomConfig.SetAppSetting(_configuration.GetSection("CustomSetting"));

            //app.UseHttpsRedirection();
            app.UseMvc();

            // 配置支持反向代理nginx. 配置中间件以转接 X-Forwarded-For 和 X-Forwarded-Proto 标头,防止转接的默认表头为none
            app.UseForwardedHeaders(new ForwardedHeadersOptions{
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}
