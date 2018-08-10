<template>
<div class="fillcontain">
    <el-tabs type="card" @tab-click="handleTabClick">
        <el-tab-pane>
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i><router-link to="/studentDetailMain/baseInfo">基础信息</router-link></span>
            <router-view></router-view>
            包括：个人基础信息、课程报名信息、缴费信息。
        </el-tab-pane>
        <el-tab-pane>
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>学习计划</span> 包括：课程安排（？） 个人课程表
        </el-tab-pane>
        <el-tab-pane>
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i><router-link to="/studentDetailMain/historyCourseList">课程记录</router-link></span> 包括：历史上课记录列表 （请假、签到等）
        </el-tab-pane>
        <el-tab-pane>
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>作品展示</span> 以卡片形式展示作品
        </el-tab-pane>
        <el-tab-pane>
            <span slot="label" @click="toggleTabs()"><i class="fa fa-bank"></i>其他信息</span> 生日礼物？
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import studentDetailBaseInfo from '@/page/studentDetailMain'
import studentDetailCourseHistory from '@/page/studentDetailCourseHistory'

export default {
    data() {
        return {}
    },
    created() {
        //this.$router.addRoutes(asyncRouterMap);
        this.$router.addRoutes([{
            path: '/studentDetailMain',
            // 在/foo下设置一个子路由
            children: [
                {
                    path: '/baseInfo',
                        // 当匹配到/foo/bar时，会在Foo's <router-view>内渲染
                        // 一个Bar组件
                    component: studentDetailBaseInfo
                },{
                    path: '/historyCourseList',
                        // Baz也是一样，不同之处是匹配的路由会是/foo/baz
                    component: studentDetailCourseHistory
                }]
            }
        ])

    },
    components: {
        studentDetailBaseInfo
    },
    methods: {
        toggleTabs(roomCode) {
            this.selectedRoomCode = roomCode;
        },
        handleTabClick() {

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
