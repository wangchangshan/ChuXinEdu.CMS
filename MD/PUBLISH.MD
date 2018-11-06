## 系统发布和部署

### 1. 项目打包

#### 1.1 前端项目

> 运行打包命令`npm run build`, 项目将会被打包生成在根目录 `dist`。也可以在`/config/index.js`文件中修改相应的配置。
```bash
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm run build
```

#### 1.2 后端 `webapi` 项目

> 运行打包命令`dotnet publish`， 相关[dotnet 命令](https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet?tabs=netcore21)。
```bash
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.Server> dotnet publish -c debug
```
> 运行命令后，会在 .vscode/launch.json配置文件中program配置的同级目录生成一个 `/publish`。

---

### 2. 项目部署Windows（前端IIS + 后端IIS）

#### 2.1 前端部署

+ 在IIS中新建一个站点。
+ 新站点名称`cxweb`，物理路径指向 1.1中生成的 `dist`目录，端口设置为`8080`，对应用程序池无要求。

#### 2.2 后端部署

> `ASP.NET Core`与经典ASP.NET应用程序完全不同，它并不是运行在IIS的工作进程(`W3wp.exe`)中，而是独立运行的。IIS为 netcore 加载了名为`AspNetCoreModule`的本地Module，这个Module用于执行外部的控制台程序。   
> `AspNetCoreModule`是作为`ASP.NET Core Server Hosting Bundle`的一部分被安装在服务器上的，因此需要先安装`Hosting Bundle`。

+ 安装[`Hosting Bundle`](https://www.microsoft.com/net/download/dotnet-core/2.1),由于项目版本是2.1.4，所以找到 Runtime 2.1.4 -> Windows -> [`ASP.NET Core/.NET Core: Runtime & Hosting Bundle`](https://download.microsoft.com/download/A/7/8/A78F1D25-8D5C-4411-B544-C7D527296D5E/dotnet-hosting-2.1.4-win.exe) 下载并安装。安装成功后，IIS的模块（Modules）中就会出现AspNetCoreModule。
+ 在IIS中新建应用程序池 `cxapi`，Net CLR Version选择 无代码托管（No Managed Code），集成模式.
+ 在IIS中使用上一步的应用程序池新建一个站点。
+ 新站点名称 **`cxapi`** ，物理路径指向 1.2中生成的 `publish`目录，端口设置为`8081`。

#### 2.3 IIS反向代理配置

> 由于前后端分离，所以还需要设置反向代理，以便可以调用api站点的接口。

+ 安装 [Application Request Routing](https://www.iis.net/downloads/microsoft/application-request-routing)（应用程序请求路由）
+ 安装 [URL Rewrite Module](https://www.microsoft.com/en-us/download/details.aspx?id=7435) （URL重写）,说明：一般上一步中就集成安装此模块
+ 安装成功后，在IIS home页面会出现`Application Request Routing Cache` 和 `URL Rewrite`
+ 选中 2.1中部署的前端站点`cxweb`，为其配置`URL Rewrite`:   
    1. URL Rewrite -> Add Rule(s) -> Blank rule
    2. 填写 Rule Name
    3. 填写 Match URL: Pattern填写`^(.*)` 表示处理所有格式的URL字符串
    4. 配置Conditions: condition input填写`{URL}`，Pattern填写`^/api/`，意思为匹配URL中以`/api/`开头的请求。说明：condition input可以有很多种({HTTP_HOST} {URL} {QUERY_STRING})，URL的格式为：`http(s)://<host>:<port>/<path>?<querystring>`
    5. 设置 Action: Action Type选择`Rewrite(重写)`，Rewrite URL填写2.2中发布的后端项目地址，如:`http://localhost:8081{C:0}' *C:0* 意思为上一步中测试后截取的url部分。勾选Append query string。
    6. 保存后，相关配置会保存在前端项目的web.config文件中。

> 说明：在配置过程中，最好不要选择`Redirct（重定向）`，重定向会产生2次跳转，需要更多的配置来应对额外产生的跨域问题。

#### 2.4 跨域问题

> 使用 `Cors` 实现跨域请求
1. 前端项目使用的是 axios，可以配置其header部分(如：`'Access-Control-Allow-Origin': '*'`)。但是还是配置后端项目允许Cors（通过实验，在后端配置cores的情况下，前端axios不配置header也可以实现跨域请求）。
2. 后端Netcore Webapi的跨域配置（**重要**）[例子](https://www.cnblogs.com/tianma3798/p/6920704.html)。
``` xml
<!-- 网上有说跨域需要在web.config中配置以下内容， 但是实验后结论为不需要配置！ 在此仅供记录-->
<system.webServer>
	<httpProtocol>
		<customHeaders>
			<add name="Access-Control-Allow-Origin" value="*" />
			<add name="Access-Control-Allow-Methods" value="POST,GET,PUT,OPTIONS" />
			<add name="Access-Control-Allow-Headers" value="x-requested-with,content-type,authorization,mypara,username" />
		</customHeaders>
	</httpProtocol>
</system.webServer>
```
3. 以上配置成功后，就可以访问项目了`http://localhost:8080`， 但是如果当 `put`|`delete` 的跨域请求报 *405* 错误，则应该是IIS 谓词限制造成的。解决办法：关闭IIS中的 `WebDAV Publishing`功能。（启用或关闭Windows功能 -> Internet Infomation Services -> World Wide Web Services -> Common HTTP Features -> WebDAV Publishing）关闭后，可以查看项目web.config的配置变化。





