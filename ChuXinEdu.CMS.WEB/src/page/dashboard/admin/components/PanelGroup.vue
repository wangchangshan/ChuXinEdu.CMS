<template>
<el-row class="panel-group" :gutter="20">
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class='card-panel' @click="handleSetChartData('student')">
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
        <div class="card-panel" @click="handleSetChartData('trialStudent')">
            <div class="card-panel-icon-wrapper icon-message">
                <i class="fa fa-user-o fa-4x" aria-hidden="true"></i>
            </div>
            <div class="card-panel-description">
                <div class="card-panel-text">本月试听</div>
                <count-to class="card-panel-num" :startVal="0" :endVal="80" :duration="2000"></count-to>
            </div>
        </div>
    </el-col>
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSetChartData('course')">
            <div class="card-panel-icon-wrapper icon-shoppingCard">
                <i class="fa fa-user fa-4x" aria-hidden="true"></i>
            </div>
            <div class="card-panel-description">
                <div class="card-panel-text">本月销课</div>
                <count-to class="card-panel-num" :startVal="0" :endVal="220" :duration="3600"></count-to>
            </div>
        </div>
    </el-col>
    <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSetChartData(income)">
            <div class="card-panel-icon-wrapper icon-money">
                <i class="fa fa-rmb fa-4x" aria-hidden="true"></i>
            </div>
            <div class="card-panel-description">
                <div class="card-panel-text">本月支出</div>
                <count-to class="card-panel-num green" :startVal="0" :endVal="1000" :duration="3000"></count-to>
            </div>
            <div class="card-panel-description">
                <div class="card-panel-text">本月收入</div>
                <count-to class="card-panel-num red" :startVal="0" :endVal="10" :duration="3000"></count-to>
            </div>
        </div>
    </el-col>
</el-row>
</template>

<script>
import CountTo from 'vue-count-to'

export default {
    components: {
        CountTo
    },
    methods: {
        handleSetChartData(type) {
            this.$emit('handleSetChartData', type)
        }
    }
}
</script>

<style lang="scss" scoped>
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
