## HOW TO INIT THE PROJECT?

### 1. 安装 [mac .net core SDK](https://download.microsoft.com/download/E/8/A/E8AF2EE0-5DDA-4420-A395-D1A50EEFD83E/dotnet-sdk-2.1.401-osx-gs-x64.pkg) / `node`/ `npm`/ `git`（windows的话需要配置环境变量，在powershell中可以使用git命令）

### 2. 使用命令创建solution 以及webapi项目

```bash
D:\myproject> dotnet new sln -o ChuXinEdu.CMS
D:\myproject> cd ChuXinEdu.CMS
D:\myproject\ChuXinEdu.CMS> dotnet new webapi -o ChuXinEdu.CMS.Server
D:\myproject\ChuXinEdu.CMS> dotnet sln ChuXinEdu.CMS.sln add **\*.csproj
```

### 3. 使用VSCode 打开solution目录ChuXinEdu.CMS，根据提示安装相应assets

### 4. 创建ChuXinEdu.CMS.WEB项目
#### 4.1 全局安装vue-cli脚手架
```bash
npm install -g vue-cli
```
#### 4.2 在ChuXinEdu.CMS目录下通过vue-cli创建一个vue项目
```bash
D:\myproject\ChuXinEdu.CMS> vue init webpack ChuXinEdu.CMS.WEB
```
> 注意： 在项目信息引导过程中，不要安装eslint。

#### 4.3 安装完成后，编译运行查看是否创建成功
```bash
D:\myproject\ChuXinEdu.CMS> cd ChuXinEdu.CMS.WEB
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm run dev
```

### 5. 为ChuXinEdu.CMS.WEB项目安装一些开发过程中需要的依赖
```bash
D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install element-ui -S

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install sass-loader --save-dev

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install less --save-dev

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install less-loader --save-dev

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install font-awesome --save-dev

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install echarts --save

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install vuex --save

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install axios --save-dev

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install vue-axios --save

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install nprogress --save

D:\myproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB> npm install vue-image-crop-upload  --save-dev
```

### 6. 修改ChuXinEdu.CMS.WEB项目目录结构如下

##############待完善



### 7. 创建`.gitignore`文件

#### 7.1 删除ChuXinEdu.CMS.WEB目录下的 `.gitignore`文件
#### 7.2 在ChuXinEdu.CMS目录下创建 `.gitignore`文件，内容如下：
```gitignore
# --
.vscode

# -- ChuXinEdu.CMS.Server
ChuXinEdu.CMS.Server/bin
ChuXinEdu.CMS.Server/obj
ChuXinEdu.CMS.Server/package-lock.json
ChuXinEdu.CMS.Server/package.lock.json

# -- ChuXinEdu.CMS.WEB
ChuXinEdu.CMS.WEB/.DS_Store
ChuXinEdu.CMS.WEB/node_modules/
ChuXinEdu.CMS.WEB/dist/
ChuXinEdu.CMS.WEB/npm-debug.log*
ChuXinEdu.CMS.WEB/yarn-debug.log*
ChuXinEdu.CMS.WEB/yarn-error.log*
ChuXinEdu.CMS.WEB/package-lock.json
ChuXinEdu.CMS.WEB/package.lock.json

# Editor directories and files
ChuXinEdu.CMS.WEB/.idea
ChuXinEdu.CMS.WEB/.vscode
ChuXinEdu.CMS.WEB/*.suo
ChuXinEdu.CMS.WEB/*.ntvs*
ChuXinEdu.CMS.WEB/*.njsproj
ChuXinEdu.CMS.WEB/*.sln
```

### 8. 将整个项目托管至github
####  8.1 登陆github网站，创建一个新的仓库 `ChuxinEdu.CMS`， 记录仓库的地址
> https://github.com/wangchangshan/ChuXinEdu.CMS.git
####  8.2 在vscode中初始化本地仓库，并上传创建的项目
```bash
D:\myproject\ChuXinEdu.CMS> git init

D:\myproject\ChuXinEdu.CMS> git add .

D:\myproject\ChuXinEdu.CMS> git commit -m "project init"

D:\myproject\ChuXinEdu.CMS> git remote add origin https://github.com/wangchangshan/ChuXinEdu.CMS.git

D:\myproject\ChuXinEdu.CMS> git push -u origin master
```
####  8.3 以上上传成功后，就可以在本地开发了。
```bash
D:\myproject\ChuXinEdu.CMS> git add .

D:\myproject\ChuXinEdu.CMS> git commit -m "xxxx"

D:\myproject\ChuXinEdu.CMS> git push
```

### 9. 新环境开发

#### 9.1 获取项目
```bash
E:\mynewproject> git clone https://github.com/wangchangshan/ChuXinEdu.CMS.git
```
#### 9.2 使用vsCode打开ChuXinEdu.CMS文件夹，安装项目所需的依赖项
```bash
E:\mynewproject\ChuXinEdu.CMS> cd ChuXinEdu.CMS.WEB

E:\mynewproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB > npm install vue-cli -g

##下载依赖包，生成node_modules目录
E:\mynewproject\ChuXinEdu.CMS\ChuXinEdu.CMS.WEB > npm install --save-dev
```
## Server part
### 10. 安装依赖包
#### 10.1 通过nuget.org官网搜索需要的依赖包
#### 10.2 使用命令安装所需依赖包，注意查看依赖项
由于MySql.Data.EntityFrameworkCore无法支持netcore.2.1，因此使用[Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
```bash
## EFcore for mysql （）
D:\myproject\ChuXinEdu.CMS.Server> dotnet add package Microsoft.AspNetCore.All --version 2.1.2
D:\myproject\ChuXinEdu.CMS.Server> dotnet add package Pomelo.EntityFrameworkCore.MySql --version 2.1.1

D:\myproject\ChuXinEdu.CMS.Server> dotnet add package AutoMapper --version 7.0.1
```


