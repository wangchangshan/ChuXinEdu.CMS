<template>
    <div class="fillcontain">
        <el-tabs v-model="courseScheduleMap" type="card" @tab-click="handleTabClick">
            <el-tab-pane label="room1" name="room1">
                <span slot="label"><i class="fa fa-bank"></i> 1号教室</span>
                <el-row>
                    <el-col :span="3" class="week-panel">
                        星期一
                    </el-col>
                    <el-col :span="3">
                        星期二
                    </el-col>
                    <el-col :span="3">
                        星期三
                    </el-col>
                    <el-col :span="3">
                        星期四
                    </el-col>
                    <el-col :span="3">
                        星期五
                    </el-col>
                    <el-col :span="3">
                        星期六
                    </el-col>
                    <el-col :span="3">
                        星期日
                    </el-col>
                    <el-col :span="3">
                        buttons
                    </el-col>
                </el-row>
            </el-tab-pane>
            <el-tab-pane label="room2" name="room2">
                <span slot="label"><i class="fa fa-bank"></i> 2号教室</span>
                this is room 2
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script>
    export default {
        data(){
            return {
                courseScheduleMap:{

                }
            }
        },
        methods: {
            handleTabClick(){
                //
            }
        }
    }
</script>

<style lang="less" scoped>
    .week-panel{
        height: 400px;
        border: 1px solid green;
    }
</style>

