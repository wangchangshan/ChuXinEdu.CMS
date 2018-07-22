<template>
    <div class="login_page">
        <transition name="form-fade" mode="in-out">
            <section class="form_container" v-show="showLogin">
                <div class="manager_tip">
                    <span class="title">初心教育后台管理系统</span>
                </div>
                <el-form v-bind:model="loginForm" v-bind:rules="rules" class="login_form">
                    <el-form-item prop="username">
                        <span class="fa-tips"><i class="fa fa-user"></i></span>
                        <el-input class="area" type="text" placeholder="用户名" v-model="loginForm.username"></el-input>
                    </el-form-item>
                    <el-form-item prop="password">
                        <span class="fa-tips"><i class="fa fa-lock"></i></span>
                        <el-input class="area" type="password" placeholder="密码" v-model="loginForm.password"></el-input>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="submitForm(loginForm)" class="submit_btn">登陆</el-button>
                    </el-form-item>
                    <div class="tiparea">
                        <p class="wxtip">温馨提示：</p>
                        <p class="tip">内部开发版</p>
                        <!-- <p class="tip">注册过的用户可凭账号密码登录</p> -->
                    </div>
                </el-form>
            </section>
        </transition>
    </div>
</template>

<script>
    import {axios, localDB} from '@/utils/' 

    export default {
        data(){
            return {
                loginForm: {
                    username: '',
                    password: ''
                },
                rules: {
                    username: [
                        { required: true, message: '请输入用户名', trigger: 'blur'},
                        { min: 2, max: 10, message: '长度在 2 到 10 个字符', trigger: 'blur'}
                    ],
                    password: [
                        { required: true, message: '请输入密码', trigger: 'blur'}
                    ]
                },
                showLogin: false
            }
        },
        mounted(){
            this.showLogin = true;
        },
        methods: {
            submitForm(loginForm) {
                this.$message.error("test");
                //alert('1');
            },
            generateMenu() {
                const leftMenu = [
                    {
                        path: '/index', name: '首页', component: 'index', icon: 'fa-server', noDropdown: true,
                            children: [
                                { path: '/index', name: '首页', component: 'index'}
                            ]
                    },
                    {
                        path: '/studentList', name: '学生列表', component: 'studentList', icon: 'fa-user', noDropdown: true,
                            children: [
                                { path: './studentList', name: '学生列表', component: 'studentList'}
                            ]
                    }
                ];
            }
        }
    }
</script>

<style lang="less" scoped>
    .login_page{
        position: absolute;
        width: 100%;
        height: 100%;
        background: url(../../static/image/bg9.jpg) no-repeat center center;
        background-size: 100% 100%;
    }
    .login_form{
        background-color:#fff;
        padding:20px;
        border-radius: 3px;
        box-shadow: 5px 5px 10px #01144c;
        .fa-tips{
            position: absolute;
            top: 0px;
            left: 10px;
            z-index: 20;
            color: #FF7c1A;
            font-size: 18px;
        }
    }
    .manager_tip{
        margin-bottom: 20px;
        .title{
            font-family: cursive;
            font-weight: bold;
            font-size: 30px;
            color: #fff;
        }
        .logo{
            width: 60px;
            height: 60px;
        }
    }
    .form_container{
        width: 370px;
        height:210px;
        position:absolute;
        top: 20%;
        left:34%;
        padding:25px;
        border-radius: 5px;
        text-align: center;
        .submit_btn{
            width: 100%;
            font-size:16px;
            letter-spacing: 20px;
        }
    }
    .tiparea{
        text-align: left;
        font-size: 12px;
        color:#4cbb15;
        .tip{
            margin-left:54px;
        }
    }
    .form-fade-enter-active, .form-fade-leave-active{
        transition: all 1s;
    }
    .form-fade-enter, .form-fade-leave-active{
        transform: translate3d(0, -50px, 0);
        opacity: 0;
    }

</style>
