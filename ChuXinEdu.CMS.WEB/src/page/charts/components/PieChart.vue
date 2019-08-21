<template>
<div :class="className" :style="{height:height,width:width}"></div>
</template>

<script>
//import echarts from 'echarts'
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
            this.setOptions(this.chartData)
        },
        setOptions({
            title,
            legendData,
            seriesData
        } = {}) {
            this.chart.setOption({
                title: {
                    text: title,
                    left: 'left'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: '{a} <br/>{b} : {c} ({d}%)'
                },
                legend: {
                    left: 'center',
                    bottom: '10',
                    data: legendData
                },
                calculable: true,
                series: [{
                    name: '分布图',
                    type: 'pie',
                    radius: [50, 70],
                    label: {
                        normal: {
                            formatter: '{a|{a}}{abg|}\n{hr|}\n  {b|{b}：}{c}  {per|{d}%}  ',
                            backgroundColor: '#eee',
                            borderColor: '#aaa',
                            borderWidth: 0,
                            borderRadius: 4,
                            rich: {
                                a: {
                                    color: '999',
                                    lineHeight: 22,
                                    align: 'center'
                                },
                                //abg: {
                                //    backgroundColor: '#333',
                                //    width: '100%',
                                //    align: 'right',
                                //    height: 22,
                                //    borderRadius: [4, 4, 0, 0]
                                //},
                                hr: {
                                    borderColor: '#aaa',
                                    width: '100%',
                                    borderWidth: 0.4,
                                    height: 0
                                },
                                b: {
                                    fontSize: 14,
                                    lineHeight: 33
                                },
                                per: {
                                    color: '#eee',
                                    backgroundColor: '#67c23a',
                                    padding: [2, 4],
                                    borderRadius: 2
                                }
                            }
                        }
                    },
                    center: ['50%', '48%'],
                    data: seriesData,
                    animationEasing: 'cubicInOut',
                    animationDuration: 2600
                }]
            })
        }
    }
}
</script>
