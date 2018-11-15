<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">
        <el-tab-pane :name="'room1'">
            <span slot="label"><i class="fa fa-bank"></i>教室一</span>
                <schedule-panel v-if="activeLabelCode === 'room1'" v-bind:roomCode="'room1'"></schedule-panel>
        </el-tab-pane>
        <el-tab-pane :name="'room2'">
            <span slot="label"><i class="fa fa-bank"></i>教室二</span> 
                <schedule-panel v-if="activeLabelCode === 'room2'" v-bind:roomCode="'room2'"></schedule-panel>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import schedulePanel from './schedulePanel'

export default {
    data() {
        return {
            activeLabelCode: 'room1'
        }
    },
    created() {
    },
    components: {
        "schedule-panel": schedulePanel
    },
    methods: {
        handleTabClick() {
            //console.log('activeLabelCode: '+ this.activeLabelCode);
        }
    }
}
</script>

<style lang="less" scoped>

</style>
