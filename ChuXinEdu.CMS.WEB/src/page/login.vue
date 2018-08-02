<template>
    <div class="login_page">
        <transition name="form-fade" mode="in-out">
            <section class="form_container" v-show="showLogin">
                <div class="manager_tip">
                    <span class="title">初心教育后台管理系统</span>
                </div>
                <el-form :model="loginForm" :rules="rules" ref="loginForm" class="login_form">
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
    import { axios, LocalDB, menuHelper } from '@/utils/index' 
    import { mapActions, mapGetters, mapState } from 'vuex'
    import NotFound from '@/page/404'
    import Home from '@/layout/home'

    export default {
        data(){
            return {
                loginForm: {
                    username: 'admin',
                    password: '123456'
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
                showLogin: false,
                ip: ''
            }
        },
        mounted(){
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
            generateMenu() {
                const leftMenu = [
                    {
                        path: '/index', name: '首页', component: 'index', icon: 'fa-server', noDropdown: true
                    },
                    // {
                    //     path: '/studentList', name: '学生列表', component: 'studentList', icon: 'fa-user', noDropdown: true
                    // },
                    {
                        path:'3',name:'学生信息',component:'content',icon:'fa-user',
						children:[
							{
                                path:'/studentAdd',name:'新增学生',component:'studentAdd'
                            },
							{
                                path:'/studentList',name:'学生列表',component:'studentList'
                            },
						]
					},
                    {
                        path:'2',name:'课程管理',component:'content',icon:'fa-asterisk',
						children:[                            
                            {
                                path:'/courseArrangMain',name:'学生排课',component:'courseArrangMain'
                            },
							{
                                path:'/sysInfoShow',name:'课程安排',component:'sysInfoShow'
                            },
							{
                                path:'/sysInfoModify',name:'上课历史',component:'sysInfoModify'
                            }
						]
                    },
                    {
                        path:'4',name:'活动组织',component:'content',icon:'fa-asterisk',
						children:[
							{
                                path:'/activityAdd',name:'新的策划',component:'activityAdd'
                            },
							{
                                path:'/activityList',name:'活动列表',component:'activityList'
                            }
						]
					},
                    {
                        path:'1',name:'系统管理',component:'content',icon:'fa-asterisk',
						children:[
							{
                                path:'/sysInfoShow',name:'个人信息',component:'sysInfoShow'
                            },
							{
                                path:'/sysInfoModify',name:'修改信息',component:'sysInfoModify'
                            }
						]
                    },
                ];

                LocalDB.instance('MENU_').setValue('leftMenu',leftMenu);
                this.addMenu(leftMenu);
                if(true) { // !this.getRouterLoadedStatus 首次进来为false,改变其状态为true
                    const routers = menuHelper.generateRoutesFromMenu(leftMenu);
                    const asyncRouterMap = [
                        {
                            path: '/404',
                            name: '404',
                            hidden: true,
                            component: NotFound //require('@/page/404.vue')  //不能使用require 无法加载component,
                        },
                        {
                            path: '/layout',
                            name: '',
                            hidden: true,
                            component: Home, //require('@/layout/home.vue'), //不能使用require 无法加载component,
                            redirect: '/index',
                            children: routers
                        }
                    ];
                        
                    this.$router.addRoutes(asyncRouterMap);
                    this.loadRouters();
                }

                this.showMessage('success', '登录成功')
                this.$router.push('layout');
            },
            submitForm(loginForm) {
                this.$refs.loginForm.validate((valid) => {
                    if(valid) {
                        let userInfo = this.loginForm;
                        let userData = Object.assign(userInfo, this.ip);
                        // axios({
                        //     type: 'get',
                        //     path: '/api/user/login',
                        //     data: userData, // 需要加密处理
                        //     fn: data => {
                        //         if(data.status == 1){
                        //             this.generateMenu(); // 模拟动态生成菜单并定位到index
                        //             this.$store.dispatch('initLeftMenu');
                        //         } else {
                        //             this.$message.error("登陆失败请重试");
                        //         }
                        //     }
                        // })
                        this.saveUserInfo(); // 存入缓存，用于显示用户名     
                        this.generateMenu(); // 模拟动态生成菜单并定位到index
                        this.$store.dispatch('initLeftMenu');   
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
            getIP(){
                axios({
                    type: 'get',
                    path: 'http://httpbin.org/ip',
                    data: '',
                    fn: data => {
                        this.ip = data.origin;
                    }
                })
            },
            saveUserInfo(){
                const userinfo = {
                    username: this.loginForm.username
                }
                LocalDB.instance('USER_').setValue('userInfo', userinfo);
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
