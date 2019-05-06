<template>
<div :class="className" :style="{height:height,width:width}"></div>
</template>

<script>
import echarts from 'echarts'
import resize from './js/resize'
import 'echarts/theme/macarons.js'
import {
    debounce
} from '@/utils/index'

const animationDuration = 3000
export default {
    mixins: [resize],
    props: {
        className: {
            type: String,
            default: 'chart'
        },
        width: {
            type: String,
            default: '100%'
        },
        height: {
            type: String,
            default: '350px'
        },
        chartData: {
            type: Object
        }
    },
    data() {
        return {
            chart: null
        }
    },
    watch: {
        chartData: {
            deep: true,
            handler(val) {
                this.setOptions(val)
            }
        }
    },
    mounted() {
        this.initChart()
        this.__resizeHanlder = debounce(() => {
            if (this.chart) {
                this.chart.resize()
            }
        }, 100)
        window.addEventListener('resize', this.__resizeHanlder)
    },
    beforeDestroy() {
        if (!this.chart) {
            return
        }
        window.removeEventListener('resize', this.__resizeHanlder)
        this.chart.dispose()
        this.chart = null
    },
    methods: {
        initChart() {
            this.chart = echarts.init(this.$el, 'macarons')
            this.setOptions(this.chartData);
        },
        setOptions({
            xMonth,
            guohua,
            xihua,
            ruanbi,
            yingbi
        } = {}) {
            var labelOption = {
                normal: {
                    show: true,
                    position: 'top',
                    distance: 10,
                    align: 'center',
                    verticalAlign: 'middle',
                    rotate: 0,
                    formatter: '{c}'
                }
            };
            this.chart.setOption({
                title: {
                    text: '销课分布',
                    left: 'left'
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: { // 坐标轴指示器，坐标轴触发有效
                        type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
                    }
                },
                legend: {
                    data: ['国画', '西画', '软笔', '硬笔']
                },
                grid: {
                    top: 40,
                    left: '2%',
                    right: '2%',
                    bottom: '3%',
                    containLabel: true
                },
                xAxis: [{
                    type: 'category',
                    data: xMonth,
                    axisTick: {
                        alignWithLabel: true
                    }
                }],
                yAxis: [{
                    type: 'value',
                    axisTick: {
                        show: false
                    }
                }],
                series: [{
                    name: '国画',
                    type: 'bar',
                    barGap: 0,
                    barWidth: '20%',
                    label: labelOption,
                    data: guohua,
                    animationDuration
                }, {
                    name: '西画',
                    type: 'bar',
                    barWidth: '20%',
                    label: labelOption,
                    data: xihua,
                    animationDuration
                }, {
                    name: '软笔',
                    type: 'bar',
                    // stack: 'shufa',
                    barWidth: '20%',
                    label: labelOption,
                    data: ruanbi,
                    animationDuration
                }, {
                    name: '硬笔',
                    type: 'bar',
                    // stack: 'shufa',
                    barWidth: '20%',
                    label: labelOption,
                    data: yingbi,
                    animationDuration
                }]
            })
        }
    }
}
</script>
