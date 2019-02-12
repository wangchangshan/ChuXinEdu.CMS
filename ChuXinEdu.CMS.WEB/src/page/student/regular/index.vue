<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">

        <el-tab-pane :name="'baseinfo'">
            <span slot="label"><i class="fa fa-info-circle"></i> 基础信息</span>
            <student-base-info v-if="activeLabelCode === 'baseinfo'" v-bind:studentCode="studentCode"></student-base-info>
        </el-tab-pane>

        <el-tab-pane :name="'history'">
            <span slot="label"><i class="fa fa-list"></i> 课程记录</span> 
            <student-course-history v-if="activeLabelCode === 'history'" v-bind:studentCode="studentCode"></student-course-history>
        </el-tab-pane>

        <el-tab-pane :name="'comment'">
            <span slot="label"><i class="fa fa-pencil"></i> 课堂评语</span> 
            <teacher-comment v-if="activeLabelCode === 'comment'" v-bind:studentCode="studentCode"></teacher-comment>
        </el-tab-pane>

        <el-tab-pane :name="'dayoff'">
            <span slot="label"><i class="fa fa-fax"></i> 请假记录</span> 
            <student-offday-history v-if="activeLabelCode === 'dayoff'" v-bind:studentCode="studentCode"></student-offday-history>
        </el-tab-pane>        

        <el-tab-pane :name="'achievement'">
            <span slot="label"><i class="fa fa-image"></i> 宝贝画集</span> 
            <student-achievements 
                v-if="activeLabelCode === 'achievement'" 
                v-bind:studentCode="studentCode">
            </student-achievements>
        </el-tab-pane>

        <el-tab-pane :name="'feeback'">
            <span slot="label"><i class="fa fa-exclamation-triangle"></i> 退 费</span> 
            <student-fee-back v-if="activeLabelCode === 'feeback'" v-bind:studentCode="studentCode"></student-fee-back>
        </el-tab-pane>

        <el-tab-pane :name="'otherinfo'">
            <span slot="label"><i class="fa fa-handshake-o"></i> 其他信息</span> 
            <student-additional-info v-if="activeLabelCode === 'otherinfo'" v-bind:studentCode="studentCode"></student-additional-info>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
// import studentDetailBaseInfo from '@/page/studentDetailBaseInfo'
import baseInfo from './baseInfo'
import courseHistory from './courseHistory'
import dayoffHistory from './dayoffHistory'
import achievements from './achievements'
import feeBack from './feeBack'
import comment from './teacherComment'
import additionalInfo from './additionalInfo'

export default {
    data() {
        return { 
            activeLabelCode: 'baseinfo',
            studentCode: ''
        }
    },
    created() {    
        this.studentCode = this.$route.query.studentcode;
    },
    components: {
        "student-base-info" : baseInfo,
        "student-course-history" : courseHistory,
        "student-offday-history" : dayoffHistory,
        "student-achievements": achievements,
        "student-fee-back": feeBack,
        "teacher-comment": comment,
        "student-additional-info":additionalInfo
    },
    methods: {
        handleTabClick() {
            //console.log('activeLabelCode: '+ this.activeLabelCode)
        }
    },
    mounted() {
    },
}
</script>

<style lang="less" scoped>

</style>
