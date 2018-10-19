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

        <el-tab-pane :name="'achievement'">
            <span slot="label"><i class="fa fa-image"></i> 作品展示</span> 
            <student-achievements 
                v-if="activeLabelCode === 'achievement'" 
                v-bind:studentCode="studentCode" 
                v-bind:style="{height: pageHeight + 'px'}">
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
import studentDetailBaseInfo from '@/page/studentDetailBaseInfo'
import studentDetailCourseHistory from '@/page/studentDetailCourseHistory'
import studentDetailAchievements from '@/page/studentDetailAchievements'
import studentDetailFeeBack from '@/page/studentDetailFeeBack'
import studentDetailAdditionalInfo from '@/page/studentDetailAdditionalInfo'

export default {
    name: 'studentDetailMain',
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height - 65 ,  
            activeLabelCode: 'baseinfo',
            studentCode: ''
        }
    },
    created() {    
        this.studentCode = this.$route.query.studentcode;
    },
    components: {
        "student-base-info" : studentDetailBaseInfo,
        "student-course-history" : studentDetailCourseHistory,
        "student-achievements": studentDetailAchievements,
        "student-fee-back": studentDetailFeeBack,
        "student-additional-info":studentDetailAdditionalInfo
    },
    methods: {
        handleTabClick() {
            //console.log('activeLabelCode: '+ this.activeLabelCode)
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
