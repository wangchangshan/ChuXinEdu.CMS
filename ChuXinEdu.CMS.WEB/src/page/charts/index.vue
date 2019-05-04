<template>
<div class="dashboard-editor-container">

    <el-row :gutter="32">
      <el-col :xs="12" :sm="12" :lg="12">
        <div class="chart-wrapper">
          <pie-chart :chart-data="totalSignUpIncome"/>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="12">
        <div class="chart-wrapper">
          <pie-chart :chart-data="totalActualIncome"/>
        </div>
      </el-col>
    </el-row>

</div>
</template>

<script>
import PieChart from './components/PieChart'
import {
    axios
} from '@/utils/index'

export default {
    data() {
        return {
            totalSignUpIncome: {
                legendData: [],
                seriesData: {}
            },
            totalActualIncome: {
                legendData: [],
                seriesData: {}
            }
        }
    },
    created() {
        this.getTotalSignUpIncome();
        this.getTotalActualIncome();
    },
    components: {
        PieChart
    },
    methods: {
        getTotalSignUpIncome() {
            axios({
                type: 'get',
                path: '/api/statistics/totalsignupincome',
                fn: (result) => {
                    this.totalSignUpIncome.legendData = [];
                    for(let obj of result){
                        this.totalSignUpIncome.legendData.push(obj.name)
                        obj.value = obj.value.toFixed(2);
                    }
                    this.totalSignUpIncome.seriesData = result;
                }
            })
        },
        getTotalActualIncome(){
            axios({
                type: 'get',
                path: '/api/statistics/totalactualincome',
                fn: (result) => {
                    this.totalActualIncome.legendData = [];
                    for(let obj of result){
                        this.totalActualIncome.legendData.push(obj.name)
                        obj.value = obj.value.toFixed(2);
                    }
                    this.totalActualIncome.seriesData = result;
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
    padding: 10px;
    margin-bottom: 10px;
}

.dashboard-editor-container {
    padding: 10px;
    // background-color: rgb(240, 242, 245);
}
</style>
