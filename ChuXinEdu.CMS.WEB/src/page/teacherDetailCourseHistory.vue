<template>
<div class="list_container">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" size="small" class="demo-form-inline search-form">
            <el-form-item label="开始日期：">
                <el-date-picker v-model="searchField.startDate" size="small" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
            </el-form-item>
            <el-form-item label="结束日期：">
                <el-date-picker v-model="searchField.endDate" size="small" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchCourse()'>查 询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetCourseList()'>重 置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
            </el-form-item>
        </el-form>
    </div>
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
            <el-table-column prop="studentName" label="学生姓名" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="courseSubject" label="课程主题" align='center' min-width="160">
            </el-table-column>
            <el-table-column prop="courseType" label="课程标识" align='center' min-width="85">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="success" icon='edit' size="small" @click='viewArtwork(scope.row.studentCourseId)'>查看作品</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <el-row>
        <el-col :span="24">
            <div class="pagination">
                <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="paginations.page_size" :layout="paginations.layout" :total="paginations.total" :current-page="paginations.current_page_index" @current-change='handlePageCurrentChange' @size-change='handlePageSizeChange'>

                </el-pagination>
            </div>
        </el-col>
    </el-row>

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
            searchField: {
                startDate: '',
                endDate: ''
            },
            paginations: {
                current_page_index: 1,
                total: 0,
                page_size: 15,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
            dateRowSpanArray: [],
            loading: false,
            downloadLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 150,

            viewDialog: {
                width: '900px',
                isShow: false,
                title: '课程作品展示',
                artWorkList: []
            }
        }
    },
    created() {
        this.getCourseList();
    },
    methods: {
        /**
         * 改变页码和当前页时需要拼装的路径方法
         * @param {string} field 参数字段名
         * @param {string} value 参数字段值
         */
        setPath(field, value) {
            var path = this.$route.path,
                query = Object.assign({}, this.$route.query);
            if (typeof field === 'object') {
                query = field;
            } else {
                query[field] = value;
            }
            this.$router.push({
                path,
                query
            });
        },
        getCourseList({
            page,
            pageSize,
            where,
            fun
        } = {}) {
            var _this = this;
            var query = this.$route.query;
            this.paginations.current_page_index = page || parseInt(query.page) || 1;
            this.paginations.page_size = pageSize || parseInt(query.page_size) || this.paginations.page_size;
            var data = {
                pageIndex: this.paginations.current_page_index,
                pageSize: this.paginations.page_size,
                q: this.searchField
            }
            if (where) {
                data = Object.assign(data, where || {});
            }

            axios({
                type: 'get',
                path: '/api/teacher/getcourselist/' + _this.teacherCode,
                data: data,
                fn: function (result) {
                    _this.paginations.total = result.totalCount;
                    result.courseList.forEach(item => {
                        item.courseDate = item.courseDate.split('T')[0];
                        item.weekName = _this.getWeekNameByCode(item.courseWeekDay);
                    });
                    _this.courseList = result.courseList;
                    _this.getRowSpanInfo();
                    fun && fun();
                }
            });
        },
        handlePageSizeChange(pageSize) {
            this.getCourseList({
                pageSize,
                fun: () => {
                    this.setPath('page_size', pageSize);
                }
            });
        },
        handlePageCurrentChange(page) {
            this.getCourseList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },

        searchCourse() {
            var page = 1;
            this.paginations.current_page_index = 1;
            this.getCourseList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },
        resetCourseList() {
            this.searchField = {
                startDate: '',
                endDate: '',
            };
            this.getCourseList();
        },

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
            let data = {
                q: this.searchField
            }
            this.downloadLoading = true
            var _this = this;
            var filename = _this.$route.query.teacherName + "销课记录";
            axios({
                type: 'get',
                path: '/api/teacher/getcourselist2export/' + _this.teacherCode,
                data: data,
                fn: function (result) {
                    import('@/vendor/Export2Excel').then(excel => {
                        const tHeader = ['上课日期', '上课时间','学生姓名', '课程类别', '课程主题'];
                        const filterVal = ['courseDate', 'coursePeriod','studentName', 'courseFolderName', 'courseSubject']
                        const data = _this.formatJson(filterVal, result)
                        excel.export_json_to_excel({
                            header: tHeader,
                            data,
                            filename: filename,
                            autoWidth: true,
                            bookType: 'xlsx'
                        })
                        _this.downloadLoading = false;
                    })
                }
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
.list_container{
    overflow-y: hidden;
}

.btnRight {
    float: right;
    margin-right: 10px !important;
}

.search_field {
    width: 140px;
}

.search_container {
    height: 36px;
    line-height: 36px;
}

.search-form {
    width: 100%;
    min-width: 750px;
}

.pagination {
    text-align: left;
    margin-top: 10px
}
</style>
