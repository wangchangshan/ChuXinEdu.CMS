<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">
        <el-tab-pane :name="'baseinfo'">
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>基础信息</span>
            <student-base-info v-if="activeLabelCode === 'baseinfo'"></student-base-info>
            包括：个人基础信息、课程报名信息、缴费信息。
        </el-tab-pane>
        <el-tab-pane :name="'plan'">
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>学习计划</span> 包括：课程安排（？） 个人课程表
        </el-tab-pane>
        <el-tab-pane :name="'history'">
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>课程记录</span> 
            <!-- 包括：历史上课记录列表 （请假、签到等） -->
            <student-course-history v-if="activeLabelCode === 'history'" v-bind:student_code="'200808001'" v-bind:student_name="'旺季'"></student-course-history>
        </el-tab-pane>
        <el-tab-pane :name="'achievement'">
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>作品展示</span> 以卡片形式展示作品
        </el-tab-pane>
        <el-tab-pane :name="'otherinfo'">
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>其他信息</span> 生日礼物？
        </el-tab-pane>
    </el-tabs>
    <keep-alive>
        <router-view></router-view>
    </keep-alive>
</div>
</template>

<script>
import studentDetailBaseInfo from '@/page/studentDetailBaseInfo'
import studentDetailCourseHistory from '@/page/studentDetailCourseHistory'

export default {
    data() {
        return {
            activeLabelCode: 'baseinfo'
        }
    },
    created() {
        //this.$router.addRoutes(asyncRouterMap);
        // this.$router.addRoutes([{
        //     path: '/studentDetailMain',
        //     // 在/foo下设置一个子路由
        //     children: [
        //         {
        //             path: '/baseInfo',
        //                 // 当匹配到/foo/bar时，会在Foo's <router-view>内渲染
        //                 // 一个Bar组件
        //             component: studentDetailBaseInfo
        //         },{
        //             path: '/historyCourseList',
        //                 // Baz也是一样，不同之处是匹配的路由会是/foo/baz
        //             component: studentDetailCourseHistory
        //         }]
        //     }
        // ])

    },
    components: {
        "student-base-info" : studentDetailBaseInfo,
        "student-course-history" : studentDetailCourseHistory

    },
    methods: {
        toggleTabs(roomCode) {
            this.selectedRoomCode = roomCode;
        },
        handleTabClick() {
            console.log('activeLabelCode: '+ this.activeLabelCode)

        },
        handleChange() {

        }
    },
    mounted() {
    },
}
</script>

<style lang="less" scoped>

</style>
