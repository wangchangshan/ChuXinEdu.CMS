<template>
<div class="dashboard-editor-container">

    <panel-group @handleSetChartData="handleSetChartData"></panel-group>

    <el-row class="chart-container">
        <panel-group-chart v-bind:height="'460px'" :chart-data="panelGroupChartData"></panel-group-chart>
    </el-row>

    <el-row style="background:#fff;padding:16px 16px 0;margin-bottom:32px;">
      <bar-chart></bar-chart>
    </el-row>

    <!-- charts -->
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <pie-chart></pie-chart>
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <pie-chart></pie-chart>
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <pie-chart></pie-chart>
        </div>
      </el-col>
    </el-row>

</div>
</template>

<script>
import PanelGroup from './components/PanelGroup'
import PanelGroupChart from './components/PanelGroupChart'
import PieChart from './components/PieChart'
import BarChart from './components/BarChart'

const chartData = {
    student:{
        xMonth:['2017-12', '2018-01', '2018-02', '2018-03', '2018-04', '2018-05', '2018-06', '2018-07', '2018-08', '2018-09', '2018-10'],
        yMeishu:[20, 30, 40, 41, 40, 41, 50, 52, 51, 61, 70],
        yShufa:[0, 0, 0, 1, 1, 4, 5, 5, 5, 5, 5],
        yTotal:[20, 30, 40, 41, 40, 45, 55, 57, 61, 68, 80],
    },
    course:{
        xMonth:[],
        yMeishu:[],
        yShufa:[],
        yTotal:[]
    },
    trialStudent:{
        xMonth:[],
        yMeishu:[],
        yShufa:[],
        yTotal:[]
    },
    income:{
        xMonth:[],
        yMeishu:[],
        yShufa:[],
        yTotal:[]
    }
}
export default {
    data(){
        return{
            panelGroupChartData: chartData.student,
            purchasesVisible: false,
            
            income: 0,
            expenditure: 0,
        }
    },
    components: {
        PanelGroup,
        PanelGroupChart,
        BarChart,
        PieChart
    },
    methods: {
        handleSetChartData(type) {
            this.panelGroupChartData = chartData[type]
        },
        showPurchases(){
            if(!this.purchasesVisible){
                this.income = 20000;
                this.expenditure = -4000;
            }
            else
            {
                this.income = 0;
                this.expenditure = -0;
            }
            this.purchasesVisible = !this.purchasesVisible;
        }
    }
}
</script>

<style lang="scss" scoped>
.chart-container{
  position: relative;
  width: 100%;
  background:#fff;
  padding:16px;
  margin-bottom:32px;
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

        .green{
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
