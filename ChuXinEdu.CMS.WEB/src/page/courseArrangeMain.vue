<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">
        <el-tab-pane :name="'room1'">
            <span slot="label"><i class="fa fa-bank"></i>教室一</span>
            <!-- <keep-alive> -->
                <course-arrange-panel v-if="activeLabelCode === 'room1'" v-bind:roomCode="'room1'"></course-arrange-panel>
            <!-- </keep-alive> -->
        </el-tab-pane>
        <el-tab-pane :name="'room2'">
            <span slot="label"><i class="fa fa-bank"></i>教室二</span> 
            <!-- <keep-alive> -->
                <course-arrange-panel v-if="activeLabelCode === 'room2'" v-bind:roomCode="'room2'"></course-arrange-panel>
            <!-- </keep-alive> -->
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import courseArrangePanel from '@/page/courseArrangePanel'

export default {
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height,  
            activeLabelCode: 'room1'
        }
    },
    created() {
        //console.log('pageHeight ： '+ this.pageHeight);
    },
    components: {
        "course-arrange-panel": courseArrangePanel
    },
    methods: {
        handleTabClick() {
            console.log('activeLabelCode: '+ this.activeLabelCode);
        }
    }
}
</script>

<style lang="less" scoped>

</style>
