<template>
<el-row>
    <div :class="className" :id="id" :style="{height:height,width:width}"></div>
</el-row>
</template>

<script>
import echarts from 'echarts'
import resize from './js/resize'

export default {
    mixins: [resize],
    props: {
        className: {
            type: String,
            default: 'chart'
        },
        id: {
            type: String,
            default: 'chart'
        },
        width: {
            type: String,
            default: '100%'
        },
        height: {
            type: String,
            default: '200px'
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
    },
    beforeDestroy() {
        if (!this.chart) {
            return
        }
        this.chart.dispose()
        this.chart = null
    },
    methods: {
        initChart() {
            this.chart = echarts.init(document.getElementById(this.id));
            this.setOptions(this.chartData)
        },
        setOptions({
            xMonth,
            yMeishu,
            yShufa,
            yTotal
        } = {}) {
            this.chart.setOption({
                backgroundColor: '#344b58', //#344b58
                title: {
                    show: false,
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        textStyle: {
                            color: '#fff'
                        }
                    }
                },
                legend: {
                    x: 20,
                    top: 20,
                    textStyle: {
                        color: '#90979c'
                    },
                    data: ['美术', '书法', '总人数']
                },
                grid: {
                    borderWidth: 0,
                    top: 70,
                    left: 60,
                    right: 38,
                    bottom: 68,
                    textStyle: {
                        color: '#fff'
                    }
                },
                calculable: true,
                xAxis: [{
                    type: 'category',
                    axisLine: {
                        lineStyle: {
                            color: '#90979c'
                        }
                    },
                    splitLine: {
                        show: false
                    },
                    axisTick: {
                        show: false
                    },
                    splitArea: {
                        show: false
                    },
                    axisLabel: {
                        interval: 0

                    },
                    data: xMonth
                }],
                yAxis: [{
                    type: 'value',
                    nameGap: 10,
                    splitLine: {
                        show: false
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#90979c'
                        }
                    },
                    axisTick: {
                        show: false
                    },
                    axisLabel: {
                        interval: 0
                    },
                    splitArea: {
                        show: false
                    }
                }],
                dataZoom: [{
                    show: true,
                    height: 20,
                    xAxisIndex: [
                        0
                    ],
                    bottom: 10,
                    start: 50,
                    end: 100,
                    handleIcon: 'path://M306.1,413c0,2.2-1.8,4-4,4h-59.8c-2.2,0-4-1.8-4-4V200.8c0-2.2,1.8-4,4-4h59.8c2.2,0,4,1.8,4,4V413z',
                    handleSize: '110%',
                    handleStyle: {
                        color: '#d3dee5'

                    },
                    textStyle: {
                        color: '#fff'
                    },
                    borderColor: '#90979c'

                }, {
                    type: 'inside',
                    show: true,
                    height: 15,
                    start: 1,
                    end: 35,
                    zoomLock: true
                }],
                series: [{
                        name: '美术',
                        type: 'bar',
                        stack: 'total',
                        barMaxWidth: 35,
                        barGap: '10%',
                        itemStyle: {
                            normal: {
                                color: 'rgba(255,144,128,1)',
                                label: {
                                    show: true,
                                    textStyle: {
                                        color: '#fff'
                                    },
                                    position: 'insideTop',
                                    formatter(p) {
                                        return p.value > 0 ? p.value : ''
                                    }
                                }
                            }
                        },
                        data: yMeishu
                    },

                    {
                        name: '书法',
                        type: 'bar',
                        stack: 'total',
                        itemStyle: {
                            normal: {
                                color: 'rgba(0,191,183,1)',
                                barBorderRadius: 0,
                                label: {
                                    show: true,
                                    position: 'top',
                                    formatter(p) {
                                        return p.value > 0 ? p.value : ''
                                    }
                                }
                            }
                        },
                        data: yShufa
                    }, {
                        name: '总人数',
                        type: 'line',
                        stack: 'total',
                        symbolSize: 10,
                        symbol: 'circle',
                        itemStyle: {
                            normal: {
                                color: 'rgba(252,230,48,1)',
                                barBorderRadius: 0,
                                label: {
                                    show: true,
                                    position: 'top',
                                    formatter(p) {
                                        return p.value > 0 ? p.value : ''
                                    }
                                }
                            }
                        },
                        data: yTotal
                    }
                ]
            })
        }
    }
}
</script>
