## 系统发布和部署

### 1. 项目打包

#### 1.1 前端静态web项目

> 运行打包命令`npm run build`, 项目将会被打包生成在根目录 `dist`。也可以在`/config/index.js`文件中修改相应的配置。
```bash
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm run build
```

#### 1.2 后端 `webapi` 项目

> 运行打包命令`dotnet publish`， 相关[dotnet 命令](https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet?tabs=netcore21)。
```bash
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.Server> dotnet publish -c release
```
> 运行命令后，会在 .vscode/launch.json配置文件中program配置的同级目录生成一个 `/publish`。

---

### 2. 项目部署Windows（前端 IIS + 后端 IIS）

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

---

### 3. 项目部署Windows（前端 Nginx + 后端 IIS）

#### 3.1 前端部署 Nginx作为静态web服务器

1. 下载 windows版本 [Nginx](http://nginx.org/en/download.html)（nginx/Windows-1.15.6）。
2. 安装 Nginx。 [教程](https://www.cnblogs.com/jiangwangxiang/p/8481661.html)。
3. 将 1.1中生成的前端项目包(dist目录)，拷贝到nginx目录下，重命名dist目录名称为`cxweb`。
4. 配置`nginx/conf/nginx.conf`文件

``` conf
worker_processes  1;

events {
    worker_connections  1024;
}

http {
    include       /etc/nginx/mime.types;
    default_type  application/octet-stream;
    sendfile        on;
    #tcp_nopush     on;

    keepalive_timeout  65;
    
    # 配置nginx压缩
    gzip on;
    gzip_min_length 5k;
    gzip_comp_level 1;
    gzip_types application/javascript text/css text/plain application/json application/font-woff font/ttf image/jpeg image/png image/svg+xml;
    gzip_buffers 4 32k;
    gzip_vary on;

    server {
        # listen 端口
        listen       8010;
        server_name  localhost;
        # 前端项目路径，root 根目录
        location / {
            root   cxweb;
            index  index.html index.htm;
        }

        error_page   500 502 503 504  /50x.html;
        location = /50x.html {
            root   html;
        }

        # 反向代理，匹配所有的 api请求。 
        # http://localhost:8077为部署在IIS上的webapi项目站点
        location ~ /api/* {
            proxy_pass   http://localhost:8077;

            # 配置远程地址，以便后端项目能够获取真实的客户端ip地址
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header REMOTE-HOST $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}

```

```bash
## 重启nginx 使配置生效
nginx -s reload
```

> 说明：也可以将Nginx只作为反向代理服务器，在 location / {} 内配置proxy_pass为 IIS上的前端站点地址（http://localhost:8078）

#### 3.2 后端部署 (IIS)

> 同 2.2配置

#### 3.3 跨域问题

> 同 2.4配置

### 4. 项目部署Windows（前端 Nginx + 后端使用netcore内置的 KestrelServer）

#### 4.1 后端部署 (KestrelServer 通过Nginx转发Http请求)

1. 使用`dotnet publish -c release`发布程序后，会生成一个publish目录
2. 使用powershell或者cmd，转到publish所在目录。执行运行命令：
```bash
D:\webServer\kestrel\cxweb\publish> dotnet ChuXinEdu.CMS.Server.dll

# 成功后会出现一下提示
# Hosting environment: Production
# Content root path: D:\cswang\webServer\kestrel\cxapi
# Now listening on: http://[::]:5000
```
#### 4.2 前端部署 Nginx作为静态web服务器

> 部署同3.1。 配置`nginx/conf/nginx.conf`文件如下：


``` conf
worker_processes  1;

events {
    worker_connections  1024;
}

http {
    include       mime.types;
    default_type  application/octet-stream;
    sendfile        on;
    #tcp_nopush     on;

    keepalive_timeout  65;
    #gzip  on;

    server {
        # listen 端口
        listen       8010;
        server_name  localhost;
        # 前端项目路径，root 根目录
        location / {
            root   cxweb;
            index  index.html index.htm;
        }

        error_page   500 502 503 504  /50x.html;
        location = /50x.html {
            root   html;
        }

        # 反向代理，匹配所有的 api请求。 
        # http://localhost:5000为4.1中的api site地址
        location ~ /api/* {
            proxy_pass   http://localhost:5000;

            # 配置远程地址，以便后端项目能够获取真实的客户端ip地址
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header REMOTE-HOST $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}
```
```bash
## 重启nginx 使配置生效
nginx -s reload
```
#### 4.3 跨域问题

> 同 2.4配置

---

### 5. 项目部署 CentOS7.4（前端 Nginx + 后端使用netcore内置的 KestrelServer）

> 参考：[使用 Nginx 在 Linux 上托管 ASP.NET Core](https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.1)
#### 5.1 前期准备
1. 修改前后端项目的公钥和私钥
2. 前后端项目分别打包。`npm run build` `dotnet publish`
3. 修改api项目的配置文件。包括：数据库链接串、AccessUrl。
4. Windows下通过 xftp（linux下通过 scp）将前后端项目上传到 CentOS服务器（创建目录 `/home/myproject/chuxin_live`）
5. Windows下通过 xshell（linux下通过ssh）远程centos，准备部署环境：MySql、Nginx、NetCore RunTime。
6. 远程连接CentOS数据库，创建所需数据库。

#### 5.2 配置Nginx（前端以及后端反向代理）
> 说明：通过yum安装的nginx，配置文件在/etc/nginx/下
```bash
cd /etc/nginx
# 配置nginx 参见：4.2
vi nginx.config
```
> 在http{}或server{}或location{}节点下添加client_max_body_size `client_max_body_size 10m;`
```bash
## 重启nginx 使配置生效
nginx -s reload
```
>关闭防火墙或者开放端口
```bash
# https://www.jianshu.com/p/411274f96492
systemctl stop firewalld.service
```


#### 5.3 转到后端项目目录运行项目
```bash
dotnet xxx.dll
```
#### 5.4 使用守护进程supervisor 托管netcore
```bash
## 1. 通过yum安装
yum install epel-release
yum install supervisor

## 2. 创建supervisor的配置目录
mkdir /ect/supervisor

## 3. 根据现有的配置文件模板，创建新的配置文件
echo_supervisord_conf > /etc/supervisor/supervisord.conf

## 4. 创建目录 confg.d，存放待托管程序的配置文件
mkdir /etc/supervisor/confg.d

## 5. 在 confg.d目录下创建托管程序配置文件
vi /etc/supervisor/confg.d/chuxin_live_netcore.conf
## 内容：
[program:chuxin_live_netcore]
command=dotnet xxxxx.dll 
directory=/home/wwwroot 
environment=ASPNETCORE_ENVIRONMENT=Production
user=www-data 
stopsignal=INT
autostart=true 
autorestart=true
startsecs=1
stderr_logfile=/var/log/xxxxx.err.log 
stdout_logfile=/var/log/xxxxx.out.log

## 6. 配置 step-3中supervisord.conf文件，在其底部添加以下内容
[include]
files = /etc/supervisor/conf.d/*.conf

## 7. 指定配置文件启动supervisor
cd /etc/supervisor
supervisord -c /etc/supervisor/supervisord.conf

## 8. 通过以上配置就已经将应用程序托管到 supervisor中了， 以下为一些常用命令
ps -ef | grep dotnet    # 查看dotnet 是否运行
supervisorctl status    # 查看所有任务状态
supervisorctl shutdown  # 关闭所有任务
supervisorctl stop all  # 停止全部进程
supervisorctl stop|start program_name  # 开启或者停止任务
supervisorctl restart program_name  # 重启某个进程
supervisorctl stop groupworker:  # 结束所有属于名为 groupworker 这个分组的进程 (start，restart 同理)
supervisorctl stop groupworker:name1  # 结束 groupworker:name1 这个进程 (start，restart 同理)
supervisorctl reload  # 载入最新的配置文件，停止原有进程并按新的配置启动、管理所有进程
supervisorctl update  # 根据最新的配置文件，启动新配置或有改动的进程，配置没有改动的进程不会受影响而重启
```
> 说明：更新程序dll后，需要重启一下任务：`supervisorctl restart chuxin_live_api_netcore`
#### 5.5 设置supervisor开机启动
> 设置开机启动，这样就不用每次重启服务器还需要启用守护进程了
```bash
## 1. 在/usr/lib/systemd/system/目录新建个supervisord.service文件
vi /usr/lib/systemd/system/supervisord.service

## 2. 添加以下内容
　　# dservice for systemd (CentOS 7.0+)
　　# by ET-CS (https://github.com/ET-CS)
　　[Unit]
　　Description=Supervisor daemon

　　[Service]
　　Type=forking
　　ExecStart=/usr/bin/supervisord -c /etc/supervisor/supervisord.conf
　　ExecStop=/usr/bin/supervisorctl shutdown
　　ExecReload=/usr/bin/supervisorctl reload
　　KillMode=process
　　Restart=on-failure
　　RestartSec=42s

　　[Install]
　　WantedBy=multi-user.target

## 3. 设置开机启动
systemctl enable supervisord
```
---