<template>
<div class="dashboard-editor-container">

    <el-row :gutter="32">
        <el-col :xs="12" :sm="12" :lg="12">
            <div class="chart-panel">
                <pie-chart :chart-data="totalSignUpIncome" :height="'300px'"/>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="12">
            <div class="chart-panel">
                <pie-chart :chart-data="totalActualIncome" :height="'300px'"/>
            </div>
        </el-col>
    </el-row>
    <el-row :gutter="32">
        <el-col :xs="24" :sm="24" :lg="24">
            <div class="chart-panel">
                <el-row type="flex" class="row-bg" justify="end" style="padding-right:20px">
                    <el-col :span="6"> 
                        <el-date-picker v-model="monthRange" :editable="false" value-format="yyyy-MM" @change="getXiaoKeDistribution" style="float:right" z-index="9999" size="small" type="monthrange" align="right" unlink-panels range-separator="至" start-placeholder="开始月份" end-placeholder="结束月份" :picker-options="pickerOptions">
                        </el-date-picker>
                    </el-col>
                </el-row>
                <bar-chart :chart-data="xiaoketongji" :height="'300px'"/>
            </div>
        </el-col>
    </el-row>
</div>
</template>

<script>
import PieChart from './components/PieChart'
import BarChart from './components/BarChart'
import {
    axios,
    dateHelper
} from '@/utils/index'

export default {
    data() {
        return {
            totalSignUpIncome: {
                title: "报名收入分布图",
                legendData: [],
                seriesData: {}
            },
            totalActualIncome: {
                title: "销课收入分布图",
                legendData: [],
                seriesData: {}
            },
            xiaoketongji: {
                xMonth: [],
                courseFolder: []
            },
            pickerOptions: {
                shortcuts: [{
                    text: '本月',
                    onClick(picker) {
                        picker.$emit('pick', [new Date(), new Date()]);
                    }
                }, {
                    text: '今年至今',
                    onClick(picker) {
                        const end = new Date();
                        const start = new Date(new Date().getFullYear(), 0);
                        picker.$emit('pick', [start, end]);
                    }
                }, {
                    text: '最近六个月',
                    onClick(picker) {
                        const end = new Date();
                        const start = new Date();
                        start.setMonth(start.getMonth() - 6);
                        picker.$emit('pick', [start, end]);
                    }
                }]
            },
            monthRange: dateHelper.getDefaultMonthRange()
        }
    },
    created() {
        this.getTotalSignUpIncome();
        this.getTotalActualIncome();
        this.getXiaoKeDistribution();
    },
    components: {
        PieChart,
        BarChart
    },
    methods: {
        // 获取所有的报名收入
        getTotalSignUpIncome() {
            axios({
                type: 'get',
                path: '/api/statistics/totalsignupincome',
                fn: (result) => {
                    this.totalSignUpIncome.legendData = [];
                    let total = 0;
                    for (let obj of result) {
                        this.totalSignUpIncome.legendData.push(obj.name)
                        total += obj.value;
                        obj.value = obj.value.toFixed(2);
                    }
                    this.totalSignUpIncome.title = "报名收入分布图 " + total + "元"
                    this.totalSignUpIncome.seriesData = result;
                }
            })
        },
        // 获取实际的销课收入
        getTotalActualIncome() {
            axios({
                type: 'get',
                path: '/api/statistics/totalactualincome',
                fn: (result) => {
                    this.totalActualIncome.legendData = [];
                    let total = 0;
                    for (let obj of result) {
                        this.totalActualIncome.legendData.push(obj.name)
                        total += obj.value;
                        obj.value = obj.value.toFixed(2);
                    }
                    this.totalActualIncome.title = "销课收入分布图 " + total.toFixed(2) + "元"
                    this.totalActualIncome.seriesData = result;
                }
            })
        },
        // 获取销课课程种类分布图
        getXiaoKeDistribution() {
            axios({
                type: 'get',
                path: '/api/statistics/coursedistribution?range=' + this.monthRange,
                fn: (result) => {
                    this.xiaoketongji = result;
                }
            })
        }
    }
}
</script>

<style lang="scss" scoped>
.chart-panel {
    position: relative;
    width: 100%;
    background: #fff;
    padding: 10px;
    margin-bottom: 6px;
}

.dashboard-editor-container {
    padding: 6px 0 0 0;
    background-color: rgb(240, 242, 245);
}
</style>
