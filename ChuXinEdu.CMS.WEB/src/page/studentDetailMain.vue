<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">

        <el-tab-pane :name="'baseinfo'">
            <span slot="label"><i class="fa fa-bank"></i>基础信息</span>
            <student-base-info v-if="activeLabelCode === 'baseinfo'" v-bind:studentCode="studentCode"></student-base-info>
        </el-tab-pane>

        <el-tab-pane :name="'history'">
            <span slot="label"><i class="fa fa-bank"></i>课程记录</span> 
            <student-course-history v-if="activeLabelCode === 'history'" v-bind:studentCode="studentCode"></student-course-history>
        </el-tab-pane>

        <el-tab-pane :name="'achievement'">
            <span slot="label"><i class="fa fa-bank"></i>作品展示</span> 
            <student-achievements 
                v-if="activeLabelCode === 'achievement'" 
                v-bind:student_code="studentCode" 
                v-bind:student_name="'王暄'" 
                v-bind:style="{height: pageHeight + 'px'}">
            </student-achievements>
        </el-tab-pane>
        
        <el-tab-pane :name="'plan'">
            <span slot="label"><i class="fa fa-bank"></i>学习计划</span> 
            略
        </el-tab-pane>
        <el-tab-pane :name="'otherinfo'">
            <span slot="label"><i class="fa fa-bank"></i>其他信息</span> 生日礼物？
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import studentDetailBaseInfo from '@/page/studentDetailBaseInfo'
import studentDetailCourseHistory from '@/page/studentDetailCourseHistory'
import studentDetailAchievements from '@/page/studentDetailAchievements'


export default {
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height - 100 ,  
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
        "student-achievements":studentDetailAchievements

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
