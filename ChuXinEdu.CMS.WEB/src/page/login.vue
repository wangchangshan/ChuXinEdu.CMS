<template>
<div class="login_page">
    <transition name="form-fade" mode="in-out">
        <div class="login_content">
            <section class="form_container" v-show="showLogin">
                <div class="form_name">
                    <span class="title">初心教育后台管理系统</span>
                </div>
                <el-form :model="loginForm" :rules="rules" ref="loginForm" class="login_form" v-loading="loading" element-loading-text="登录验证中...">
                    <el-form-item prop="username">
                        <span class="fa-tips"><i class="fa fa-user"></i></span>
                        <el-input class="area" type="text" placeholder="用户名" v-model="loginForm.username"></el-input>
                    </el-form-item>
                    <el-form-item prop="password">
                        <span class="fa-tips"><i class="fa fa-lock"></i></span>
                        <el-input class="area" type="password" placeholder="密码" v-model="loginForm.password"></el-input>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="submitForm(loginForm)" class="submit_btn">登录</el-button>
                    </el-form-item>
                    <div class="tiparea">
                        <!-- <p class="wxtip">内部开发版</p>
                    <p class="tip"></p> -->
                        <!-- <p class="tip">测试</p> -->
                    </div>
                </el-form>
            </section>
        </div>
    </transition>
</div>
</template>

<script>
import {
    axios,
    LocalDB,
    RSAHelper,
    menuHelper
} from '@/utils/index'

import {
    mapActions,
    mapGetters,
    mapState
} from 'vuex'
import Home from '@/layout/home'

export default {
    data() {
        return {
            loading: false,
            loginForm: {
                username: '',
                password: ''
            },
            rules: {
                username: [{
                        required: true,
                        message: '请输入用户名',
                        trigger: 'blur'
                    },
                    {
                        min: 2,
                        max: 10,
                        message: '长度在 2 到 10 个字符',
                        trigger: 'blur'
                    }
                ],
                password: [{
                    required: true,
                    message: '请输入密码',
                    trigger: 'blur'
                }]
            },
            showLogin: false,
            ip: ''
        }
    },
    created() {
        document.onkeydown = (e) => {
            let _key = window.event.keyCode;
            if(_key === 13){
                this.submitForm('loginForm');
            }
        }
    },  
    mounted() {
        this.showLogin = true;
    },
    computed: {
        ...mapGetters(['getMenuItems', 'getRouterLoadedStatus'])
    },
    methods: {
        ...mapActions(['addMenu', 'loadRouters']),
        showMessage(type, message) {
            this.$message({
                type: type,
                message: message
            })
        },
        generateMenu(roles) {
            let arrRole = roles && roles.split(',') || [];
            axios({
                type: 'get',
                path: '/api/config/getmenus',
                fn: result => {
                    result.forEach(item => item.meta = {roles: ',' + item.roles + ','})
                    // 菜单按角色权限生成
                    let menuList = result.filter((item) => {
                        item.roles = ',' + item.roles + ','
                        for(let r of arrRole){
                            if(item.roles.indexOf(',' + r + ',') > -1){
                                return true;
                            }
                        }
                        return false;
                    })
                    LocalDB.instance('MENU_').setValue('LEFTMENU', menuList);
                    this.addMenu(menuList);
                    let defaultPage = "/dashboard";
                    console.log("roles:" + roles)
                    if((',' + roles + ',').indexOf(',1007,') == -1){
                        defaultPage = "/courseSignIn";
                    }

                    if (true) { //!this.getRouterLoadedStatus
                        const routers = menuHelper.generateRoutesFromMenu(menuList);
                        const asyncRouterMap = [{
                            path: '/layout',
                            name: '',
                            hidden: true,
                            component: Home,
                            redirect: defaultPage,
                            children: routers
                        }];

                        this.$router.addRoutes(asyncRouterMap);
                        this.loadRouters();
                    }
                    this.$router.push('layout');
                }
            });
        },
        submitForm(loginForm) {
            this.$refs.loginForm.validate((valid) => {
                if (valid) {
                    this.loading = true;
                    let loginData = {
                        username: this.loginForm.username,
                        password: RSAHelper.encrypt(this.loginForm.password)
                    }
                    axios({
                        type: 'post',
                        path: '/api/account/login',
                        data: loginData,
                        fn: result => {
                            if (result.code == 1200) {
                                this.saveUserInfo(result);
                                this.getConfigs();
                                this.generateMenu(result.roles);
                                this.$store.dispatch('initLeftMenu');
                            } else if (result.code == 1700) {
                                this.$message.error("不合法的请求，请不要重复尝试！");
                            } else if (result.code == 1701) {
                                this.$message.error("当前用户已在其他地方登陆，请稍后重试！");
                            } else if (result.code == 1103) {
                                this.$message.error("当前账户已被锁，请联系管理员。");
                            } else {
                                this.$message.error("用户名或密码错误，请重试！");
                            }
                            this.loading = false;
                        }
                    });
                } else {
                    this.$notify.error({
                        title: '错误',
                        message: '请输入正确的用户名密码',
                        offset: 100
                    });
                    return false;
                }
            });
        },
        getConfigs() {
            var test = 'student_temp_status,student_status,teacher_status,course_category,course_folder,pay_pattern';
            axios({
                type: 'get',
                path: '/api/config/getdics',
                data: {
                    codes: test
                },
                fn: result => {
                    if (result) {
                        this.$store.commit('set_all_dic', result);
                        LocalDB.instance('DIC_').setValue('ALL', result);
                    }
                }
            });
        },
        getIP() {
            axios({
                type: 'get',
                path: 'http://httpbin.org/ip',
                data: '',
                fn: data => {
                    this.ip = data.origin;
                }
            })
        },
        saveUserInfo(info) {
            const userinfo = {
                username: this.loginForm.username,
                token: info.data,
                roles: info.roles
            }

            LocalDB.instance('USER_').setValue('BASEINFO', userinfo);
        }
    }
}
</script>

<style lang="less" scoped>
.login_page {
    position: absolute;
    width: 100%;
    height: 100%;
    background: url(../../static/image/bg9.jpg) no-repeat center center;
    background-size: 100% 100%;
}

.login_content {
    top: 50%;
    width: 370px;
    position: relative;
    margin: 0 auto;
}

.form_container {
    width: 370px;
    border-radius: 5px;
    text-align: center;
    transform: translateY(-50%);

    .submit_btn {
        width: 100%;
        font-size: 16px;
        letter-spacing: 20px;
    }
}

.login_form {
    background-color: #fff;
    padding: 20px;
    border-radius: 3px;
    box-shadow: 5px 5px 10px #01144c;

    .fa-tips {
        position: absolute;
        top: 0px;
        left: 10px;
        z-index: 20;
        color: #FF7c1A;
        font-size: 18px;
    }
}

.form_name {
    margin-bottom: 20px;

    .title {
        font-family: cursive;
        font-weight: bold;
        font-size: 30px;
        color: #fff;
    }

    .logo {
        width: 60px;
        height: 60px;
    }
}

.tiparea {
    text-align: left;
    font-size: 12px;
    color: #4cbb15;

    .tip {
        margin-left: 54px;
    }
}

.form-fade-enter-active,
.form-fade-leave-active {
    transition: all 1s;
}

.form-fade-enter,
.form-fade-leave-active {
    transform: translate3d(0, -50px, 0);
    opacity: 0;
}
</style>
