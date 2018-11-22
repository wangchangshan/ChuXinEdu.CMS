<template>
<div class="info_container">
    <div class="table_container">
        <el-table :data="dayoffList" :span-method="objectSpanMethod" v-loading="loading" size="mini" align="left" border :height="tableHeight">
            <el-table-column type="index" width="50" align='center'></el-table-column>
            <el-table-column prop="courseDate" label="请假日期" align='center' min-width="135">
                <template slot-scope='scope'>
                    {{ scope.row.courseDate + " " + scope.row.weekName }}
                </template>
            </el-table-column>
            <el-table-column prop="coursePeriod" label="时间段" align='center' min-width="90">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="90">
                <template slot-scope='scope'>
                    {{ scope.row.courseCategoryName + " / " + scope.row.courseFolderName }}
                </template>
            </el-table-column>
        </el-table>
    </div>
</div>
</template>

<script>
import {
    dateHelper,
    axios
} from '@/utils/index'

export default {
    name: 'student-offday-history',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            studentName: '',
            dayoffList: [],
            dateRowSpanArray: [],
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 65
        }
    },
    created() {
        this.studentName = this.$route.query.studentname;
        this.getDayoffList();
    },
    methods: {
        getDayoffList() {
            axios({
                type: 'get',
                path: '/api/student/getdayofflist',
                data: {
                    studentCode: this.studentCode
                },
                fn: result => {
                    result.forEach(item => {
                        item.courseDate = item.courseDate.split('T')[0];
                        item.weekName = dateHelper.getWeekNameByCode(item.courseWeekDay);
                    });
                    this.dayoffList = result;
                    this.getRowSpanInfo();
                    this.loading = false;
                }
            });
        },
        getRowSpanInfo() {
            this.dateRowSpanArray = [];
            let cDate = '';
            let dateIndex = 0;
            this.dayoffList.forEach((item, index, array) => {
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
        packageColorFlag({row, rowIndex}){
            let i = this.packages.indexOf(row.studentCoursePackageId);
            if(i > -1){
                return 'row' + i;
            }
            return '';
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
