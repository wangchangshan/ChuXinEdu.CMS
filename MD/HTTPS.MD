## Https 配置

### 1. 开放阿里云443端口

### 2. 备案域名
> 略
### 3. 安装 acme.sh
[中文文档](https://github.com/Neilpang/acme.sh/wiki/%E8%AF%B4%E6%98%8E)   
[dns api](https://github.com/Neilpang/acme.sh/wiki/dnsapi)

### 4. 在阿里云创建RAM访问子用户

### 5. 为子用户分配包含DNS相关的权限

### 6. 根据3中的文档，使用dns生成证书
```bash
export Ali_Key="sdfsdfsdfljlbjkljlkjsdfoiwje"
export Ali_Secret="jlsdflanljkljlfdsaklkjflsa"
```
>之后：
```bash
acme.sh --issue --dns dns_ali -d shuyouxiaowu.com -d *.shuyouxiaowu.com
```
### 7. 安装或copy证书到nginx目录
> 先确保存在/etc/nginx/ssl/目录
```bash
mkdir /etc/nginx/ssl
```
>copy证书并指定nginx reload命令
``` bash
acme.sh  --installcert  -d  www.shuyouxiaowu.com   \
        --key-file   /etc/nginx/ssl/www.shuyouxiaowu.key \
        --fullchain-file /etc/nginx/ssl/fullchain.cer \
        --reloadcmd  "service nginx force-reload"
```

### 8. 修改nignx,支持https访问，并强制http跳转https.
```bash
server {
	listen 80;
	# listen [::]:80;
	server_name  www.shuyouxiaowu.com;
	return 301 https://$server_name$request_uri;
	}

    server {
        # listen       80 default_server;
        # listen       [::]:80 default_server;

	listen       443 ssl http2 default_server;
	listen       [::]:443 ssl http2 default_server;
        server_name  www.shuyouxiaowu.com;

	ssl_certificate "/etc/nginx/ssl/fullchain.cer";
	ssl_certificate_key "/etc/nginx/ssl/shuyouxiaowu.key";
	ssl_session_cache shared:SSL:1m;
	ssl_session_timeout  10m;
	ssl_ciphers HIGH:!aNULL:!MD5;
	ssl_prefer_server_ciphers on;

	client_max_body_size 15m;


        # root         /usr/share/nginx/html;
	# root         /home/myproject/chuxin_live/web/index.html

        # Load configuration files for the default server block.
        include /etc/nginx/default.d/*.conf;

	    # 前端项目路径
        location / {
		    root	/home/myproject/chuxin_live/web;
		    index	index.html index.htm;
        }

	    # 配置反向代理
	    location ~ /api/* {
		    proxy_pass	http://localhost:5000;
		    proxy_set_header	Host $host;
		    proxy_set_header X-Real-IP $remote_addr;
		    proxy_set_header REMOTE-HOST $remote_addr;
		    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
	    }

        error_page 404 /404.html;
            location = /40x.html {
        }

        error_page 500 502 503 504 /50x.html;
            location = /50x.html {
        }
    }
```

### 9. 参考
[快速签发 Let's Encrypt 证书指南](https://www.cnblogs.com/esofar/p/9291685.html)