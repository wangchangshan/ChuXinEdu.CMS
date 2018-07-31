<template>
    <header class="head-nav" id='header_container'>
        <el-row style="margin:0 10px">
            <el-col :span="6" class="logo-container">
                <img src="../../static/image/logo.png" class="logo" alt=""/>
                <span class="title">初心教育后台管理系统</span>
            </el-col>
            <div class="userinfo">
                <img src="../../static/image/avatar.png" class="avatar" alt="">
                <div class="welcome">
                    <p class="name comename">欢迎</p>
                    <p class="name avatarname">Admin</p>
                </div>
                <span class="username">
                    <el-dropdown trigger="click" @command='setDialogInfo'>
                        <span class="el-dropdown-link">
                            <i class="el-icon-caret-bottom el-icon--right"></i>
                        </span>
                        <el-dropdown-menu slot="dropdown">
                            <el-dropdown-item command='info'>修改信息</el-dropdown-item>
                            <el-dropdown-item command='pwd'>修改密码</el-dropdown-item>
                            <el-dropdown-item command='exit'>退出</el-dropdown-item>
                        </el-dropdown-menu>
                    </el-dropdown>
                </span>
                 <i class="fa fa-sign-out logout" @click="logout"></i>
            </div>
            <div class="notify-row">
                <ul class="top-menu">
                    <li class="li-badge">
                        <a href='#/index'>
                            <el-badge :value="6" class="item one">
                                <i class="fa fa-tasks"></i>
                            </el-badge>
                        </a>
                    </li>
                    <li class="li-badge">
                        <a href='#/index'>
                            <el-badge :value="12" class="item two">
                                <i class="fa fa-envelope-o"></i>
                            </el-badge>
                        </a>
                    </li>
                     <li class="li-badge">
                        <a href='#/notifyBirthday'>
                            <el-badge :value="34" class="item three">
                                <i class="fa fa-bell-o"></i>
                            </el-badge>
                        </a>
                    </li>
                </ul>
            </div>
        </el-row>
    </header>
</template>

<script>
    import { LocalDB } from '@/utils/' 
    export default {
        name: "head-nav",
        data(){
            return {
                userinfo:''
            }
        },
        created(){
            this.userinfo = LocalDB.instance('USER_').getValue('userInfo');
        },
        methods: {
            logout(){
                this.$router.push('/');
            },
            showInfoList(){
                //this.$router.push('/'); // '/infoModify'
            },
            setDialogInfo(cmdItem) {
                if(!cmdItem) {
                    this.$message('菜单选项缺少command属性');
                    return;
                }
                switch (cmdItem) {
                    case 'info':
                        break;
                    case 'pwd':
                        this.showInfoList();
                        break;
                    case 'exit':
                        this.logout();
                        break;
                }
            }
        }
    }
</script>


<style scoped lang="less">
    .logo-container {
        line-height: 60px;
        min-width: 400px;
        .logo {
            height: 50px;
            width: 50px;
            margin-right: 5px;
            vertical-align: middle;
            display: inline-block;
        }
        .title {
            vertical-align: middle;
            font-size: 22px;
            font-family: cursive;
            letter-spacing: 3px;
        }
    }
    .fa-user {
        position: relative;
        top: -2px;
        margin-right: 4px;
    }
    .head-nav {
        width: 100%;
        height: 60px;
        min-width: 600px;
        padding: 5px;
        background: #324057;
        position: fixed;
        top: 0;
        left: 0;
        z-index: 100;
        color: #fff;
        border-bottom: 1px solid #1F2D3D;
        .logout {
            vertical-align: middle;
            cursor: pointer;
        }
    }
    .userinfo {
        line-height: 60px;
        text-align: right;
        float: right;
    }
    .avatar {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        vertical-align: middle;
        display: inline-block;
    }
    .welcome {
        display: inline-block;
        width: auto;
        vertical-align: middle;
        padding: 0 5px;
        .name {
            line-height: 20px;
            text-align: center;
            font-size: 14px;
        }
        .comename {
            font-size: 12px;
        }
        .avatarname {
            color: #a9d86e;
            font-weight: bolder;
        }
    }
    .username {
        cursor: pointer;
        margin-right: 5px;
        .el-dropdown {
            color: #fff;
        }
    }
    .border {
        border: 1px solid;
    }
    .notify-row {
        line-height: 60px;
        float: right;
        margin-right: 20px;
        margin-top: 5px;
    }
    ul.top-menu > li {
        float: left;
        margin-right: 20px;
    }
    ul.top-menu > li > a {
        color: #3bc5ff;
        font-size: 16px;
        border-radius: 4px;
        -webkit-border-radius: 4px;
        border: 1px solid #f0f0f8 !important;
        padding: 2px 6px 4px 6px;
    }
</style>
