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