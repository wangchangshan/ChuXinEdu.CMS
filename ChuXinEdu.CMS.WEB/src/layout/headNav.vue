<template>
<div class="head-nav" id='header_container'>
    <div class="bread-bars">
        <i class="fa fa-bars" :class="{vertical:isVertical, horizontal:isHorizontal}" aria-hidden="true" @click="handleLefeMenu"></i>
    </div>
    <div class="bread-container">
        <el-breadcrumb>
            <transition-group name="breadcrumb">
                <el-breadcrumb-item :to="{ path: item.path }" v-for="(item) in breadcrumbList" v-if="item.title" :key="item.path">
                    {{item.title}}
                </el-breadcrumb-item>
            </transition-group>
        </el-breadcrumb>
    </div>
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
        </el-dropdown-menu>
        </el-dropdown>
        </span>
        <i class="fa fa-sign-out logout" @click="logout"></i>
    </div>

    <div class="notify-row">
        <ul class="top-menu">
            <li class="li-badge">
                <a>
                <el-popover
                    width="225"
                    trigger="hover">
                    <el-table :data="$store.state.header.birthdayList" stripe size="mini">                        
                        <el-table-column width="100" property="student_name" label="姓名"></el-table-column>
                        <el-table-column width="120" property="student_birthday" label="生日"></el-table-column>
                    </el-table>
                    <el-badge :value="$store.state.header.birthdayCount" class="item one" slot="reference">
                        <i class="fa fa-birthday-cake"></i>
                    </el-badge>
                </el-popover>
                    
                </a>
            </li>
            <li class="li-badge">
                <a href='#/courseSignIn'>
                    <el-badge :value="$store.state.header.toRecordCount" class="item two">
                        <i class="fa fa-calendar-check-o"></i>
                            <!-- 待签到数目 -->
                    </el-badge>
                </a>
            </li>
            <li class="li-badge">
                <a>
                <el-popover
                    width="430"
                    trigger="hover">
                    <el-table :data="$store.state.header.toFinishList" stripe size="mini">                        
                        <el-table-column width="80" property="student_name" label="姓名"></el-table-column>
                        <el-table-column width="190" property="package_name" label="套餐名称"></el-table-column>
                        <el-table-column width="70" property="rest_course_count" label="剩余课时" align='center'>
                            <template slot-scope="scope">  
                                <span style="color:#f56767;font-weight:600"> {{ scope.row.rest_course_count }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column prop="operation" align='center' label="操作" fixed="right" width="80">
                            <template slot-scope='scope'>
                                <el-button type="success" icon='edit' size="mini" @click='showStudentDetail(scope.row.student_code,scope.row.student_name)'>详细</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <el-badge :value="$store.state.header.toFinishCount" class="item three" slot="reference">
                        <i class="fa fa-bell-o"></i>
                    </el-badge>
                </el-popover>
                <!-- 剩余课时数小于等于5的学生 -->
                </a>
            </li>
        </ul>
    </div>
</div>
</template>

<script>
import {
    axios,
    LocalDB
} from '@/utils/'

export default {
    name: "head-nav",
    data() {
        return {
            isVertical: false,
            isHorizontal: true,
            userInfo: '',
            path: '',
            breadcrumbList: '',
        }
    },
    created() {
        let strUser = LocalDB.instance('USER_').getValue('BASEINFO').value;
        this.userInfo = JSON.parse(strUser);
    },
    watch: {
        $route() {
            this.getBreadcrumb()
        }
    },
    mounted() {
        this.getBreadcrumb();
    },
    methods: {
        getBreadcrumb() {
            var current = {
                title: this.$route.name,
                path: this.$route.path
            }

            if (current.path == '/dashboard') {
                this.breadcrumbList = [current];
            } else if (current.path == '/studentDetail') {
                current.title = this.$route.query.studentname
                this.breadcrumbList = [{
                    title: '首 页',
                    path: '/dashboard'
                }, {
                    title: '学生列表',
                    path: '/student'
                }].concat(current);
            } else if (current.path == '/teacherDetail') {
                current.title = this.$route.query.teacherName
                this.breadcrumbList = [{
                    title: '首 页',
                    path: '/dashboard'
                }, {
                    title: '教师列表',
                    path: '/teacher'
                }].concat(current);
            } else {
                this.breadcrumbList = [{
                    title: '首 页',
                    path: '/dashboard'
                }].concat(current);
            }
        },

        handleLefeMenu() {
            this.isVertical = !this.isVertical;
            this.isHorizontal = !this.isHorizontal;
            this.$store.dispatch('setMenuCollapse');

        },

        showStudentDetail(studentCode, studentName) {
            this.$router.push({
                path: '/studentDetail',
                query: {
                    studentcode: studentCode,
                    studentname: studentName
                }
            })
        },

        logout() {
            axios({
                type: 'post',
                path: '/api/account/logout/' + this.userInfo.username,
                fn: result => {}
            });
            
            this.$store.state.menu.isCollapse = false;
            LocalDB.instance('MENU_').remove("LEFTMENU");
            LocalDB.instance('USER_').remove('BASEINFO');
            this.$router.push('/');
        },

        setDialogInfo(cmdItem) {
            if (!cmdItem) {
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

<style lang="less" scoped>
.bread-bars {
    vertical-align: middle;
    display: inline-block;
    margin: 0 10px;
    font-size: 26px;
    cursor: pointer;
    line-height: 60px;

    .vertical {
        -webkit-transform: rotate(90deg);
        transform: rotate(90deg);
        transition: .38s;
        -webkit-transform-origin: 50% 50%;
        transform-origin: 50% 50%;
    }

    .horizontal {
        -webkit-transform: rotate(0deg);
        transform: rotate(0deg);
        transition: .38s;
        -webkit-transform-origin: 50% 50%;
        transform-origin: 50% 50%;
    }
}

.bread-container {
    vertical-align: middle;
    min-width: 270px;
    display: inline-block;
}

.fa-user {
    position: relative;
    top: -2px;
    margin-right: 4px;
}

.head-nav {
    height: 60px;
    background: #fff;
    border-bottom: 1px solid #d8dce5;
    box-shadow: 0 1px 3px 0 rgba(0, 0, 0, .12), 0 0 3px 0 rgba(0, 0, 0, .04);

    .logout {
        vertical-align: middle;
        cursor: pointer;
        font-size: 26px;
        margin-right: 15px;
    }
}

.userinfo {
    line-height: 60px;
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
    padding: 0;

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
    margin-right: 10px;
}

.border {
    border: 1px solid;
}

.notify-row {
    line-height: 60px;
    float: right;
    margin-top: 3px;
    margin-right: 15px;
}

ul.top-menu>li {
    float: left;
    margin-right: 15px;
}

ul.top-menu>li>a {
    color: #3bc5ff;
    font-size: 16px;
    border-radius: 4px;
    -webkit-border-radius: 4px;
    border: 1px solid #f0f0f8 !important;
    padding: 2px 8px 8px 8px;
    cursor: pointer;
}
</style>
