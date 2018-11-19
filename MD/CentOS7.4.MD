## CentOS7.4 环境配置

### 1. Mysql（8.0.x）安装

#### 1.1 添加包
```bash
## 创建一个存放rpm的目录
[root@izm5e17v57iiomu87 ~]# cd ..

[root@izm5e17v57iiomu87 /]# cd home

[root@izm5e17v57iiomu87 home]# mkdir downloads

[root@izm5e17v57iiomu87 home]# cd downloads

## 获取rpm地址的方式在：https://dev.mysql.com/downloads/repo/yum/
[root@izm5e17v57iiomu87 downloads]# wget https://repo.mysql.com//mysql80-community-release-el7-1.noarch.rpm

[root@izm5e17v57iiomu87 downloads]# sudo rpm -ivh mysql80-community-release-el7-1.noarch.rpm
```

#### 1.2 安装

```bash
## 安装
[root@izm5e17v57iiomu87 downloads]# sudo yum -y install mysql-community-server

## 重启服务
service mysqld restart

## 查看版本信息
mysql -V
```
> 说明：MySql默认是自启动的

#### 1.3 root账号密码修改
```bash
## 查看MySQL为Root账号生成的临时密码
grep "A temporary password" /var/log/mysqld.log

## 进入MySQL shell
mysql -u root -p

## 修改密码
ALTER USER 'root'@'localhost' IDENTIFIED BY 'Mypwd123!';
```

#### 1.4 开放端口
```bash
## 开放端口
firewall-cmd --add-port=3306/tcp --permanent

## 重新加载防火墙设置
firewall-cmd --reload
```

#### 1.5 创建用户 & 数据库(mysql-shell)
> 通过 mysql -u root -p 进入MySql Shell 后：
```bash
## 新建本地用户
CREATE USER 'test'@'localhost' IDENTIFIED BY '123456';

## 新建远程用户
CREATE USER 'test'@'%' IDENTIFIED BY '123456';

## 查看用户
use mysql;
select host,user from mysql.user;

## 新建数据库
CREATE DATABASE testdb;

## 赋予指定账户指定数据库远程访问权限
GRANT ALL PRIVILEGES ON testdb.* TO 'test'@'%';

## 赋予指定账户对所有数据库远程访问权限
GRANT ALL PRIVILEGES ON *.* TO 'test'@'%';

## 赋予指定账户对所有数据库本地访问权限
GRANT ALL PRIVILEGES ON *.* TO 'test'@'localhost';

## 刷新权限
FLUSH PRIVILEGES;
```
> 设置防火墙，允许远程访问：
``` bash
## 设置一下防火墙，允许远程连接，mysql端口为3306
[root@izm5e17v57iiomu87 ~]# iptables -I INPUT -p tcp -m state --state NEW -m tcp --dport 3306 -j ACCEPT

## 查看防火墙状态，3306端口是否可以访问
[root@izm5e17v57iiomu87 ~]# iptables -L -n

## 如果想要限制访问：
[root@izm5e17v57iiomu87 ~]# iptables -D INPUT -p tcp -m state --state NEW -m tcp --dport 3306 -j ACCEPT
```

#### 1.6 权限示例(mysql-shell)
```bash
## 查看权限
SHOW GRANTS FOR 'test'@'%';

## 赋予权限
GRANT ALL PRIVILEGES ON *.* TO 'test'@'%';

## 收回权限
REVOKE ALL PRIVILEGES ON *.* FROM 'test'@'%';

## 刷新权限
FLUSH PRIVILEGES;

## 删除用户
DROP USER 'test'@'localhost';
```

#### 1.7 修改字符编码
```bash
## 查找配置文件位置
[root@izm5e17v57iiomu87 ~]# whereis my.cnf
my: /etc/my.cnf

## 修改配置文件
vi /etc/my.cnf

## 修改1：增加client配置（文件开头）
[client]
default-character-set=utf8mb4

## 修改2：增加mysqld配置（文件结尾）
## charset
character-set-server=utf8mb4
collation-server=utf8mb4_general_ci


## 重启服务已使配置生效
[root@izm5e17v57iiomu87 ~]# service mysqld restart
```

#### 1.8 定时备份
1. 创建备份目录
```bash
mkdir /home/dbback
cd /home/dbback
```
2. 编写数据库备份脚本 database_mysql_shell.sh
```bash
vi database_mysql_shell.sh

#!/bin/bash
DATE=`date +%Y%m%d_%H%M%S`          #every minute
DATABASE=dbname                     #database name
BACKUP_PATH=/backup/mysqldata       #backup path

#backup command 备份并压缩 （Mysql5.6以后命令行界面上使用明文密码）
/usr/bin/mysqldump $DATABASE | gzip > ${BACKUP_PATH}\/${DATABASE}_${DATE}.sql.gz

#just backup the latest 10 days

find ${BACKUP_PATH} -mtime +10 -name "${DATABASE}_*.sql.gz" -exec rm -f {} \;
```
3. 修改my.cnf配置文件
```bash
vi /etc/my.cnf

## 添加以下内容，数据库用户名和密码。保存退出。
[mysqldump]
user=root
password=root
```
4. 重启mysql服务
```bash
service mysqld retart
```
5. 为备份脚本添加可执行权限
```bash
chmod u+x database_mysql_shell.sh
```
6. 添加计划任务
```bash
crontab -e

## 在页面中编写以下内容并保存退出
00 3 * * * /root/database_backup_shell.sh
```
> 以上实现每天凌晨三点自动执行编写的备份脚本，备份mysql数据库到/backup/mysqldata，且只保留最近10天的数据库备份。

#### 1.9 配置阿里云上的安全组规则，使MySQL(3306)可远程访问

---

### 2. .Net Core运行环境安装

#### 2.1 注册微软产品，安装需要的依赖

```bash
sudo rpm -Uvh https://packages.microsoft.com/config/rhel/7/packages-microsoft-prod.rpm
```

#### 2.2 安装.Net RunTime

> 官方参考：[为支持的 CentOS 和 Oracle Linux 分发/版本（64 位）安装 .NET Core](https://docs.microsoft.com/zh-cn/dotnet/core/linux-prerequisites?tabs=netcore2x)

```bash
## 更新yum，使其含有最新的 Net运行时
sudo yum update

## 通过yum安装运行时
sudo yum install aspnetcore-runtime-2.1
```
> 说明：The previous command will install the .NET Core Runtime Bundle, which includes the .NET Core runtime and the ASP.NET Core runtime. To install just the .NET Core runtime, use the dotnet-runtime-2.1 package.

---
### 3 安装Nginx

#### 3.1 环境准备
```bash
## gcc 安装 nginx 需要先将官网下载的源码进行编译，编译依赖 gcc 环境 
yum install gcc-c++

## PCRE pcre-devel 是一个Perl库，包括 perl 兼容的正则表达式库。nginx 的 http 模块使用 pcre 来解析正则表达式，所以需要在 linux 上安装 pcre 库，pcre-devel 是使用 pcre 开发的一个二次开发库。
yum install -y pcre pcre-devel

## zlib 提供了很多种压缩和解压缩的方式， nginx 使用 zlib 对 http 包的内容进行 gzip
yum install -y zlib zlib-devel

## OpenSSL 是一个强大的安全套接字层密码库，囊括主要的密码算法、常用的密钥和证书封装管理功能及 SSL 协议，并提供丰富的应用程序供测试或其它目的使用。nginx 不仅支持 http 协议，还支持 https（即在ssl协议上传输http）
yum install -y openssl openssl-devel

## 查看已经安装过的软件包
yum list installed
```
#### 3.2 yum包准备
> 这里我们通过简单的yum包管理器进行安装Nginx
```bash
## 查看yum 中是否含有nginx的安装包 
## 更新当前yum包 
sudo yum update

## 查看是否有nginx包
yum search nginx

## 如果没有nginx包，可以通过Nginx官网提供的Centos源地址，添加nginx源
sudo rpm -Uvh http://nginx.org/packages/centos/7/noarch/RPMS/nginx-release-centos-7-0.el7.ngx.noarch.rpm
```

#### 3.3 通过yum安装
```bash
## 安装
sudo yum install -y nginx

## 启动Nginx服务
sudo systemctl start nginx.service  
# or 
service nginx start

## 设置nginx开机启动
sudo systemctl enable nginx.service
```
> 说明：Nginx的默认安装配置文件在 `/etc/nginx`路径下的 `nginx.conf`

#### 3.4 配置阿里云上的安全组规则，开放 HTTP(80)端口

Finish
---
