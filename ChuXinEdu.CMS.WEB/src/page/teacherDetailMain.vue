<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">

        <el-tab-pane :name="'baseinfo'">
            <span slot="label"><i class="fa fa-info-circle"></i> 基础信息</span>
            <teacher-base-info v-if="activeLabelCode === 'baseinfo'" v-bind:teacherCode="teacherCode"></teacher-base-info>
        </el-tab-pane>

        <el-tab-pane :name="'history'">
            <span slot="label"><i class="fa fa-list"></i> 上课记录</span> 
            <teacher-course-history v-if="activeLabelCode === 'history'" v-bind:teacherCode="teacherCode"></teacher-course-history>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import teacherDetailBaseInfo from '@/page/teacherDetailBaseInfo'
import teacherDetailCourseHistory from '@/page/teacherDetailCourseHistory'

export default {
    name: 'teacherDetailMain',
    data() {
        return { 
            activeLabelCode: 'baseinfo',
            teacherCode: ''
        }
    },
    created() {    
        this.teacherCode = this.$route.query.teacherCode;
    },
    components: {
        "teacher-base-info" : teacherDetailBaseInfo,
        "teacher-course-history" : teacherDetailCourseHistory,
    }
}
</script>

<style lang="less" scoped>

</style>
