<template>
<div class="dashboard-editor-container">

    <panel-group v-bind:overView="overView" @handleSetChartData="handleSetChartData"></panel-group>

    <el-row class="chart-container">
        <panel-group-chart v-bind:height="'460px'" :chart-data="panelGroupChartData"></panel-group-chart>
    </el-row>

    <el-row>
        <el-table :span-method="objectSpanMethod" :data="arrangeToday" style="width: 100%" border align="center" size="small">
            <el-table-column prop="coursePeriod" label="今 天" align='center' width="180">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程内容" width="280" align='center'>
            </el-table-column> 
            <el-table-column prop="studentName" label="学生姓名" align='center'>
            </el-table-column> 
        </el-table>
    </el-row>
</div>
</template>

<script>
import PanelGroup from './components/PanelGroup'
import PanelGroupChart from './components/PanelGroupChart'
import {
    axios
} from '@/utils/index'

let chartData = {
    student: {
        xMonth: [],
        yMeishu: [],
        yShufa: [],
        yTotal: [],
    },
    course: {
        xMonth: [],
        yMeishu: [],
        yShufa: [],
        yTotal: []
    },
    trialStudent: {
        xMonth: [],
        yMeishu: [],
        yShufa: [],
        yTotal: []
    },
    income: {
        xMonth: [],
        yMeishu: [],
        yShufa: [],
        yTotal: []
    }
}
export default {
    data() {
        return {
            arrangeToday: [],
            panelGroupChartData: chartData.student,
            overView: {
                studentCount: 0,
                trialStudentCount: 0,
                courseCount: 0,
            },
            timeRowSpanArray: [],
            folderRowSpanArray: []
        }
    },
    created() {
        this.getPanelGroupChartData('student');
        this.getArrangeToday();
    },
    components: {
        PanelGroup,
        PanelGroupChart
    },
    methods: {
        handleSetChartData(type) {
            if (chartData[type].xMonth.length > 0) {
                this.panelGroupChartData = chartData[type]
            } else {
                this.getPanelGroupChartData();
            }
        },
        getPanelGroupChartData(type) {
            axios({
                type: 'get',
                path: '/api/statistics/dashboard',
                fn: (result) => {
                    chartData = result;
                    this.overView = {
                        studentCount: result.student.yTotal[result.student.yTotal.length - 1],
                        trialStudentCount: result.trialStudent.yTotal[result.trialStudent.yTotal.length - 1],
                        courseCount: result.course.yTotal[result.course.yTotal.length - 1],
                    }
                    this.panelGroupChartData = chartData[type];
                }
            })
        },
        getArrangeToday() {
            axios({
                type: 'get',
                path: '/api/coursearrange/getcoursearrangedtoday',
                fn: (result) => {
                    this.arrangeToday = result;
                    this.getRowSpanInfo();
                }
            })
        },
        objectSpanMethod({
            row,
            column,
            rowIndex,
            columnIndex
        }) {
            if (columnIndex === 0) {
                return {
                    rowspan: this.timeRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
            if (columnIndex === 1) {
                return {
                    rowspan: this.folderRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
        },
        getRowSpanInfo() {
            this.timeRowSpanArray = [];
            this.folderRowSpanArray = [];
            let strTime = '';
            let strFolder = '';
            let timeIndex = 0;
            let folderIndex = 0;
            this.arrangeToday.forEach((item, index, array) => {
                this.timeRowSpanArray.push(1);
                this.folderRowSpanArray.push(1);
                if (index === 0) {
                    strTime = item.coursePeriod;
                    strFolder = item.courseFolderCode;
                } else {
                    if (item.coursePeriod === strTime) {
                        this.timeRowSpanArray[timeIndex] += 1;
                        this.timeRowSpanArray[index] = 0;

                        if (item.courseFolderCode === strFolder) {
                            this.folderRowSpanArray[folderIndex] += 1;
                            this.folderRowSpanArray[index] = 0;
                        } else {
                            strFolder = item.courseFolderCode;
                            folderIndex = index;
                        }
                    } else {
                        strTime = item.coursePeriod;
                        timeIndex = index;

                        strFolder = item.courseFolderCode;
                        folderIndex = index;
                    }
                }
            });
        },
    }
}
</script>

<style lang="scss" scoped>
.chart-container {
    position: relative;
    width: 100%;
    background: #fff;
    padding: 10px;
    margin-bottom: 10px;
}

.dashboard-editor-container {
    padding: 10px;
    background-color: rgb(240, 242, 245);
}
</style>
