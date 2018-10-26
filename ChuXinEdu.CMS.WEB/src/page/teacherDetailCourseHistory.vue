<template>
<div class="info_container">
    <div class="table_container">
        <el-table :data="courseList" :span-method="objectSpanMethod" v-loading="loading" size="mini" align="left" border stripe :max-height="tableHeight">
            <el-table-column type="index" width="40"></el-table-column>
            <el-table-column prop="courseDate" label="上课日期" align='center' min-width="140">
                <template slot-scope='scope'>
                    {{ scope.row.courseDate + " " + scope.row.weekName }}
                </template>
            </el-table-column>
            <el-table-column prop="coursePeriod" label="上课时间段" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="courseSubject" label="课程主题" align='center' min-width="160">
            </el-table-column>
            <el-table-column prop="courseType" label="课程标识" align='center' min-width="85">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="225">
                <template slot-scope='scope'>
                    <el-button type="success" icon='edit' size="small" @click='viewArtwork(scope.row.studentCourseId)'>查看作品</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    
    <div class="footer_container">
        <el-button type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
    </div>

    <el-dialog :title="viewDialog.title" :visible.sync="viewDialog.isShow" :width="viewDialog.width" :close-on-press-escape='false' :modal-append-to-body="false">
        <el-carousel indicator-position="outside" :autoplay="false" height="550px">
            <el-carousel-item v-for="item in viewDialog.artWorkList" :key="item.artworkId">
                <div style="overflow:auto;width:100%; height:100%">
                    <img :src="item.showURL">
                </div>
            </el-carousel-item>
        </el-carousel>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    name: 'teacher-course-history',
    props: {
        'teacherCode': String,
    },
    data() {
        return {
            courseList: [],
            dateRowSpanArray: [],
            loading: false,
            downloadLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 100,

            viewDialog: {
                width: '900px',
                isShow: false,
                title: '课程作品展示',
                artWorkList: []
            }
        }
    },
    created() {
        var _this = this;
        axios({
            type: 'get',
            path: '/api/teacher/getcourselist/' + _this.teacherCode,
            fn: function (result) {
                result.forEach(item => {
                    item.courseDate = item.courseDate.split('T')[0];
                    item.weekName = _this.getWeekNameByCode(item.courseWeekDay);
                });
                _this.courseList = result;
                _this.getRowSpanInfo();
            }
        });
    },
    methods: {
        viewArtwork(courseId) {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/course/getcourseartwork',
                data: {
                    courseId: courseId
                },
                fn: function (result) {
                    _this.viewDialog.artWorkList = result;
                    if (result.length > 0) {
                        _this.viewDialog.isShow = true;
                    } else {
                        _this.$message({
                            message: '还没有上传作品',
                            type: 'warning'
                        });
                    }
                }
            });
        },

        getRowSpanInfo() {
            this.dateRowSpanArray = [];
            let cDate = '';
            let dateIndex = 0;
            this.courseList.forEach((item, index, array) => {
                this.dateRowSpanArray.push(1);
                if (index === 0) {
                    cDate = item.courseDate;
                } else {
                    if (item.courseDate === cDate) {
                        this.dateRowSpanArray[dateIndex] += 1;
                        this.dateRowSpanArray[index] = 0;
                    } else {
                        cDate = item.courseDate;
                        dateIndex = index;
                    }
                }
            });
        },
        objectSpanMethod({
            row,
            column,
            rowIndex,
            columnIndex
        }) {
            if (columnIndex === 1) {
                return {
                    rowspan: this.dateRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
        },

        getWeekNameByCode(code) {
            let week = '';
            switch (code) {
                case 'day1':
                    week = '星期一';
                    break;
                case 'day2':
                    week = '星期二';
                    break;
                case 'day3':
                    week = '星期三';
                    break;
                case 'day4':
                    week = '星期四';
                    break;
                case 'day5':
                    week = '星期五';
                    break;
                case 'day6':
                    week = '星期六';
                    break;
                case 'day7':
                    week = '星期日';
                    break;
                default:
                    break;
            }
            return week;
        },

        export2Excle() {
            if (this.courseList.length == 0) {
                this.$message({
                    message: '没有数据需要导出！',
                    type: 'success'
                });
                return;
            }
            var filename = this.courseList[0].teacherName + "销课记录";
            this.downloadLoading = true
            import('@/vendor/Export2Excel').then(excel => {
                const tHeader = ['上课日期', '上课时间', '课程类别', '课程主题', '上课教师'];
                const filterVal = ['courseDate', 'coursePeriod', 'courseFolderName', 'courseSubject', 'teacherName']
                const data = this.formatJson(filterVal, this.courseList)
                excel.export_json_to_excel({
                    header: tHeader,
                    data,
                    filename: filename,
                    autoWidth: true,
                    bookType: 'xlsx'
                })
                this.downloadLoading = false;
            })
        },
        formatJson(filterVal, jsonData) {
            return jsonData.map(v => filterVal.map(j => {
                if (j === 'courseDate') {
                    return v[j] && v[j].split('T')[0] || ''; //parseTime(v[j])
                } else {
                    return v[j]
                }
            }))
        }
    }
}
</script>

<style lang="less" scoped>
.footer_container {
    height: 36px;
    line-height: 36px;
    text-align: left;
}
</style>
