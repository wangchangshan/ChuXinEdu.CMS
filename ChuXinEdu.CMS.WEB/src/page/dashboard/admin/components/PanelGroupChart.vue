<template>
<el-row>
    <div :class="className" :id="id" :style="{height:height,width:width}"></div>
</el-row>
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
                this.setOptions(val);
            }
        }
    },
    mounted() {
        this.initChart();
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
            this.chart = echarts.init(document.getElementById(this.id), 'macarons');
            this.setOptions(this.chartData)
        },
        setOptions({
            xMonth,
            courseCategory,
            yTotal
        } = {}) {
            if (courseCategory.length == 0) {
                return;
            }
            let mySeries = [];
            let myLegend = [];
            let colors = ['#7eb00a', '#5ab1ef', '#2ec7c9', '#b6a2de', '#d87a80', '#9a7fd1', '#c9ab00', '#c05050', '#7eb00a', '#5ab1ef', '#2ec7c9', '#b6a2de', '#d87a80', '#9a7fd1', '#c9ab00', '#c05050', '#7eb00a', '#5ab1ef', '#2ec7c9', '#b6a2de', '#d87a80', '#9a7fd1', '#c9ab00', '#c05050']
            for (let i = 0; i < courseCategory.length; i++) {
                let category = courseCategory[i];
                myLegend.push(category.name)
                let obj = {
                    name: category.name,
                    type: 'bar',
                    stack: 'MonthStack',
                    barMaxWidth: 35,
                    barGap: '10%',
                    itemStyle: {
                        normal: {
                            color: colors[i],
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
                    data: category.sum,
                    animationDuration: animationDuration
                };
                mySeries.push(obj);
            }
            
            let totalSeries = {
                name: '总数',
                type: 'line',
                stack: 'MonthStack',
                symbolSize: 10,
                symbol: 'circle',
                itemStyle: {
                    normal: {
                        color: '#d87a80',
                        barBorderRadius: 0,
                        label: {
                            show: true,
                            position: 'top'
                        }
                    }
                },
                data: yTotal,
                animation: true,
                animationThreshold: false,
                animationDuration: animationDuration
            };
            myLegend.push('总数');
            mySeries.push(totalSeries);

            let monthZoomStart = 0;
            let totalMonthCount = xMonth.length;
            //默认视图中最多出现一年的数据
            if(totalMonthCount <= 12) {
                monthZoomStart = 0;
            }
            else {
                monthZoomStart = (totalMonthCount - 12.0) / totalMonthCount * 100;
            }

            this.chart.setOption({
                title: {
                    show: false,
                },
                tooltip: {
                    trigger: 'axis',
                },
                legend: {
                    x: 20,
                    top: 20,
                    data: myLegend
                },
                toolbox: {
                    show: true,
                    feature: {
                        mark: {
                            show: true
                        },
                        dataView: {
                            show: true,
                            readOnly: false,
                            optionToContent: function (opt) {
                                var axisData = opt.xAxis[0].data;
                                var series = opt.series;
                                var table = '<table style="width:100%;text-align:center;border:1px solid #CCC"><tbody><tr>' +
                                    '<td>月份</td>' +
                                    '<td>' + series[0].name + '</td>' +
                                    '<td>' + series[1].name + '</td>' +
                                    '<td>' + series[2].name + '</td>' +
                                    '</tr>';
                                for (var i = 0, l = axisData.length; i < l; i++) {
                                    table += '<tr>' +
                                        '<td>' + axisData[i] + '</td>' +
                                        '<td>' + series[0].data[i] + '</td>' +
                                        '<td>' + series[1].data[i] + '</td>' +
                                        '<td>' + series[2].data[i] + '</td>' +
                                        '</tr>';
                                }
                                table += '</tbody></table>';
                                return table;
                            }
                        },
                        magicType: {
                            show: true,
                            type: ['line', 'bar']
                        },
                        restore: {
                            show: true
                        },
                        saveAsImage: {
                            show: true
                        }
                    }
                },
                grid: {
                    borderWidth: 0,
                    top: 70,
                    left: 60,
                    right: 38,
                    bottom: 68
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
                    type: 'slider',
                    show: true,
                    height: 20,
                    xAxisIndex: [
                        0
                    ],
                    bottom: 10,
                    start: monthZoomStart,
                    end: 100,
                    handleIcon: 'path://M306.1,413c0,2.2-1.8,4-4,4h-59.8c-2.2,0-4-1.8-4-4V200.8c0-2.2,1.8-4,4-4h59.8c2.2,0,4,1.8,4,4V413z',
                    handleSize: '110%'
                }, {
                    type: 'inside',
                    disable: true,
                    show: true,
                    height: 15,
                    start: 1,
                    end: 35,
                    zoomLock: true
                }],
                series: mySeries
            });
        }
    }
}
</script>

<style lang="scss" scoped>
.color-panel {
    color: #2ec7c9;
    color: #b6a2de;
    color: #5ab1ef;
    color: #ffb980;
    color: #d87a80;
    color: #8d98b3;
    color: #e5cf0d;
    color: #97b552;
    color: #95706d;
    color: #dc69aa;
    color: #07a2a4;
    color: #9a7fd1;
    color: #588dd5;
    color: #f5994e;
    color: #c05050;
    color: #59678c;
    color: #c9ab00;
    color: #7eb00a;
    color: #6f5553;
    color: #c14089;
}
</style>
