<template>
<div class="dashboard-editor-container">

    <panel-group v-bind:overView="overView" @handleSetChartData="handleSetChartData"></panel-group>

    <el-row class="chart-container">
        <panel-group-chart v-bind:height="'460px'" :chart-data="panelGroupChartData"></panel-group-chart>
    </el-row>

    <el-row :gutter="10">
        <el-table :data="arrangeToday" style="width: 100%" border align="center" size="small">
            <el-table-column prop="coursePeriod" label="上课时间" align='center' width="180">
            </el-table-column>
            <el-table-column prop="guohua" label="国 画" align='center'>
            </el-table-column>
            <el-table-column prop="xihua" label="西 画" align='center'>
            </el-table-column>
            <el-table-column prop="shufa" label="书 法" align='center'>
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
            }
        }
    },
    created() {
        this.getPanelGroupChartData('student');
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
                path: '/coursearrange/getcoursearrangedtoday',
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
        }
    }
}
</script>

<style lang="scss" scoped>
.chart-container {
    position: relative;
    width: 100%;
    background: #fff;
    padding: 16px;
    margin-bottom: 32px;
}

.dashboard-editor-container {
    padding: 20px;
    background-color: rgb(240, 242, 245);

    .chart-wrapper {
        background: #fff;
        padding: 16px 16px 0;
        margin-bottom: 32px;
    }
}

.panel-group {
    margin-bottom: 20px;

    .card-panel-col {
        margin-bottom: 10px;
    }

    .card-panel {
        height: 108px;
        cursor: pointer;
        font-size: 12px;
        position: relative;
        overflow: hidden;
        color: #666;
        background: #fff;
        box-shadow: 4px 4px 40px rgba(0, 0, 0, .05);
        border-color: rgba(0, 0, 0, .05);

        &:hover {
            .card-panel-icon-wrapper {
                color: #fff;
            }

            .icon-people {
                background: #40c9c6;
            }

            .icon-message {
                background: #36a3f7;
            }

            .icon-money {
                background: #f4516c;
            }

            .icon-shoppingCard {
                background: #34bfa3
            }
        }

        .icon-people {
            color: #40c9c6;
        }

        .icon-message {
            color: #36a3f7;
        }

        .icon-money {
            color: #f4516c;
        }

        .red {
            color: #f4516c;
        }

        .green {
            color: #34bfa3
        }

        .icon-shoppingCard {
            color: #34bfa3
        }

        .card-panel-icon-wrapper {
            float: left;
            margin: 14px 0 0 14px;
            padding: 16px;
            transition: all 0.38s ease-out;
            border-radius: 6px;
        }

        .card-panel-icon {
            float: left;
            font-size: 48px;
        }

        .card-panel-description {
            float: right;
            font-weight: bold;
            margin: 26px;
            margin-left: 0px;

            .card-panel-text {
                line-height: 18px;
                color: rgba(0, 0, 0, 0.45);
                font-size: 16px;
                margin-bottom: 12px;
            }

            .card-panel-num {
                font-size: 20px;
            }
        }
    }
}
</style>
