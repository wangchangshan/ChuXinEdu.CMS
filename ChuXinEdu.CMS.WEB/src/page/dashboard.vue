<template>
<div class="dashboard-editor-container">

    <!-- panel-group -->
    <el-row class="panel-group" :gutter="20">
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class='card-panel' @click="handleSetLineChartData('newVisitis')">
                <div class="card-panel-icon-wrapper icon-people">
                    <i class="fa fa-users fa-4x" aria-hidden="true"></i>
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">学生总数</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="100" :duration="3000"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('messages')">
                <div class="card-panel-icon-wrapper icon-message">
                    <i class="fa fa-image fa-4x" aria-hidden="true"></i>
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">作品数目</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="122" :duration="2000"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('purchases')">
                <div class="card-panel-icon-wrapper icon-money">
                    <i class="fa fa-rmb fa-4x" aria-hidden="true"></i>
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本月收入</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="19280" :duration="3200"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('shoppings')">
                <div class="card-panel-icon-wrapper icon-shoppingCard">
                    <i class="fa fa-rmb fa-4x" aria-hidden="true"></i>
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本月支出</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="-3600" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>
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
import CountTo from 'vue-count-to'
import PieChart from './components/PieChart'
import BarChart from './components/BarChart'

export default {
    components: {
        CountTo,
        BarChart,
        PieChart
    },
    methods: {
        handleSetLineChartData(type) {
            this.$emit('handleSetLineChartData', type)
        }
    }
}
</script>

<style lang="scss" scoped>
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
