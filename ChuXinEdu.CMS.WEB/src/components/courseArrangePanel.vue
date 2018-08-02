<template>
<div class="schedule-panel">
    <el-row class="schedule-area row">
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期一</p>
                <el-collapse v-model="active_titles_Monday">
                    <el-collapse-item name="1">
                        <span slot="title" class="sub-title"><a>16:00-17:30   </a> <a style="color:green;float:right; padding-right:5px">共2人</a></span>
                        <div class="student-list-content">
                            <div class="student-item">熊大 <el-tag type="success" size="mini">国画</el-tag> 10</div>
                            <div>熊二 <el-tag size="mini">西画</el-tag> 9</div>
                        </div>
                    </el-collapse-item>
                    <el-collapse-item title="反馈" name="2">
                        <span slot="title" class="sub-title"><a>17:30-19:00</a> <a style="color:green;float:right; padding-right:5px">共4人</a></span>
                        <div class="student-list-content">
                            <div>海绵宝宝 | 国画 | 10</div>
                            <div>光头强   | 国画 | 9</div>
                            <div>熊二   | 国画 | 9</div>
                            <div>欧阳蛋蛋   | 国画 | 20</div>
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期二</p>
                {{testContent}}
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期三</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期四</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期五</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期六</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期日</p>
            </div>
        </el-col>
        <el-col :span="2">
            <div class="week-panel">
                <p class="title">按钮以及待排课学生信息区域</p>
            </div>
        </el-col>
    </el-row>
</div>
</template>

<script>
export default {
    data() {
        return {
            testContent: '',
            active_titles_Monday: ['1','2']
        }
    },
    props: {
        currentRoomCode: [String]
    },
    watch: {
        // 监听属性的变化，可以接收参数;
        currentRoomCode(roomCode) {
            this.initRoomCourseArrangeInfo(roomCode);
        }
    },
    methods: {
        initRoomCourseArrangeInfo(roomCode) {
            // axios get data info            
            this.testContent = 'this is ' + roomCode;
        }
    }
}
</script>

<style lang="less" scoped>
.schedule-panel {
    overflow-x: auto;
    .schedule-area {
        min-width: 1150px;
        overflow-y:auto;
        .week-panel {
            height: 550px;
            max-height: 600px; // border: 1px solid #dfdfdf;
            border-left: 1px solid #dfdfdf;
            border-bottom: 1px solid #dfdfdf;
            border-top: 1px solid #dfdfdf;
            font-size: 14px;
            padding: 0;
            .title {
                width: 100%;
                text-align: center;
                font-weight: 600;
                height: 30px;
                line-height: 30px;
                background-color: #3bc5ff;
                border: 1px solid #3bc5ff;
                color: #fff;
                display: block;
                cursor: pointer;
            }
            .sub-title{
                width: 100%;
                margin-left: 5px;
                padding-left: 5px;
                font-weight: 600;
            }
            .student-list-content{
                padding-left: 5px;
                margin-left: 5px;
            }
        }
    }
}
</style>
