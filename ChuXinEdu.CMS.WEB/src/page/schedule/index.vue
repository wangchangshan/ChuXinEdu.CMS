<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode" @tab-click="handleTabClick">
        <el-tab-pane v-for="item in classroom" :key="item.value" :name="item.value">
            <span slot="label">
                <i class="fa fa-cubes"></i>{{ ' ' + item.label }}
            </span>
            <schedule-panel v-if="activeLabelCode === item.value" v-bind:roomCode="item.value"></schedule-panel>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

import schedulePanel from "./schedulePanel";

export default {
    data() {
        return {
            classroom: [],
            activeLabelCode: ""
        };
    },
    created() {
        axios({
            type: 'get',
            path: '/api/config/getdicbycode',
            data: {
                typeCode: 'classroom'
            },
            fn: result => {
                this.classroom = result;
                this.activeLabelCode = result && result[0].value || '';
            }
        })
    },
    components: {
        "schedule-panel": schedulePanel
    },
    methods: {
        handleTabClick() {
            //console.log('activeLabelCode: '+ this.activeLabelCode);
        }
    }
};
</script>

<style lang="less" scoped>
</style>
