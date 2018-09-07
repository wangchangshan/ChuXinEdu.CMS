<template>
<div class="schedule-panel">
    <el-row class="schedule-area row">
        <el-col :span="3" v-for="day in coursePeriods" v-bind:key="day.dayCode">
            <div class="week-panel">
                <p class="title">{{day.dayName}}</p>
                <el-collapse v-model="day.activePeriods">
                    <el-collapse-item v-for="period in day.periods" v-bind:key="period.periodCode" :name="period.periodCode">
                        <div slot="title" class="student-list-title">
                            <a>{{period.periodName}}</a>
                            <a class="student-title-right">共{{ period.studentList.length }}人</a>
                        </div>
                        <div class="student-list-content">
                            <ul>
                                <li v-for="student in period.studentList" v-bind:key="student.studentCode">
                                    <el-popover trigger="hover" placement="top">
                                        <p>姓名: 李世民</p>
                                        <p>住址: 北京市昌平区</p>
                                        <div slot="reference" class="name-wrapper" style="display:inline">
                                            <a class="student-item-left" @click="getStudentCourseList(student.studentCode, student.studentName,day.dayCode, day.dayName, period.periodName)">{{student.studentName}} </a>
                                        </div>
                                    </el-popover>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">{{student.courseCategoryName + student.courseRestCount}}节</el-tag>
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer">
                            <!-- <el-button type="warning" plain @click="removeStudent()" size="mini">试听</el-button> -->
                            <!-- <el-button  plain circle icon="el-icon-plus"  @click="removeStudent()" size="mini"></el-button> -->
                            <el-button plain @click="addStudent(day.dayCode, day.dayName,period.periodName)" icon="el-icon-plus" size="mini">添加</el-button>
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">其他操作</p>
                <ul>
                    <li style="margin-top:10px;margin-left:5px">
                        <el-button plain icon="el-icon-refresh" type="success" size="mini">刷   新</el-button>
                    </li>
                    <li style="margin-top:10px;margin-left:5px">
                        <el-button plain type="primary" size="mini" @click="showAllPeriod()">全部展开</el-button>
                    </li>
                     <li style="margin-top:10px;margin-left:5px">
                        <el-button plain type="primary" size="mini" @click="closeAllPeriod()">全部折叠</el-button>
                    </li>
                </ul>
                <!-- <el-button type="success" size="mini">假期安排</el-button> -->
                <!-- <el-date-picker type="dates" v-model="holidays" placeholder="选择放假日期">
                </el-date-picker> -->
            </div>
        </el-col>
    </el-row>

    <el-dialog :title="selectStudentDialog.title" :visible.sync="selectStudentDialog.isShow" :modal-append-to-body="false" :width="selectStudentDialog.width">
        <el-table :data="selectStudentDialog.studentList" max-height="400" @selection-change="handleStudentSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="studentCode" label="学号" width="120"></el-table-column>
            <el-table-column property="studentName" label="姓名" width="70"></el-table-column>
            <el-table-column property="courseCategoryName" label="课程类别" width="80" align='center'></el-table-column>
            <el-table-column property="flexCourseCount" label="剩余课时数" align='center' width="100"></el-table-column>
            <el-table-column property="selectedCourseCount" label="选择课时数" min-width="140">
                <template slot-scope="scope">
                    <el-input-number v-model="scope.row.selectedCourseCount" :min="1" :max="scope.row.flexCourseCount" label="描述文字" size="small"></el-input-number>
                </template>
            </el-table-column>
            <el-table-column property="firstCourseDate" label="开始上课日期" min-width="210">
                <template slot-scope="scope">
                    <el-date-picker v-model="scope.row.firstCourseDate" type="date" size="small" value-format="yyyy-MM-dd" placeholder="选择日期" :picker-options="selectStudentDialog.pickerDateOptions">
                    </el-date-picker>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button @click="pickStudents()" type="success" size="small">确定</el-button>
            <el-button @click="selectStudentDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog :title="studentCourseDialog.title" :visible.sync="studentCourseDialog.isShow" :modal-append-to-body="false" :width="studentCourseDialog.width">
        <el-table :data="studentCourseDialog.courseList" max-height="400" @selection-change="handleCourseListChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="courseDate" label="上课日期" width="100"></el-table-column>
            <el-table-column property="courseCategoryName" label="课程类别" width="120" align='center'></el-table-column>
            <el-table-column property="attendanceStatusName" label="状态" width="120" align='center'></el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" min-width="200">
                <template slot-scope='scope'>
                    <el-button plain type="warning" icon='edit' size="small" @click='qingJiaCourse(scope.row.studentCourseId)'>请假</el-button>
                    <el-button plain type="danger" icon='edit' size="small" @click='removeCourse(scope.row.studentCourseId)'>删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button type="danger" size="small" @click='removeCourseBatch()'>批量删除</el-button>
            <el-button @click="studentCourseDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    props: {
        'roomCode': String
    },
    data() {
        return {
            coursePeriods: [{
                    dayCode: 'day1',
                    dayName: '星期一',
                    periods: [
                        // {
                        //     periodCode: '',
                        //     periodName: '',
                        //     studentList:[],
                        // },
                    ],
                    activePeriods: []
                },
                {
                    dayCode: 'day2',
                    dayName: '星期二',
                    periods: [],
                    activePeriods: []
                },
                {
                    dayCode: 'day3',
                    dayName: '星期三',
                    periods: [],
                    activePeriods: []
                },
                {
                    dayCode: 'day4',
                    dayName: '星期四',
                    periods: [],
                    activePeriods: []
                },
                {
                    dayCode: 'day5',
                    dayName: '星期五',
                    periods: [],
                    activePeriods: []
                },
                {
                    dayCode: 'day6',
                    dayName: '星期六',
                    periods: [],
                    activePeriods: []
                },
                {
                    dayCode: 'day7',
                    dayName: '星期日',
                    periods: [],
                    activePeriods: []
                },
            ],
            activePeriods_bak:{
                day1:[],
                day2:[],
                day3:[],
                day4:[],
                day5:[],
                day6:[],
                day7:[],
            },
            holidays: [],
            selectStudentDialog: {
                title: '',
                isShow: false,
                width: '830px',
                curDayCode: '',
                curPeriodName: '',
                studentList: [],
                selectedStudents: [],
                pickerDateOptions: {
                    firstDayOfWeek: 1,
                    disabledDate: (time) => { // 为了this指向 使用箭头函数 
                    //debugger
                        var curDay = parseInt(this.selectStudentDialog.curDayCode.replace('day',''));
                        if (curDay === 7){
                            curDay = 0;
                        }
                        return time.getDay() !== curDay;
                    }
                }
            },
            studentCourseDialog: {
                title: '',
                isShow: false,
                width: '660px',
                selectedCourses: [],
                courseList: []
            }
        };
    },
    created() {
        this.getTemplatePeriod();
    },
    methods: {

        // 排课模板展示信息
        getTemplatePeriod() {
            var _this = this;
            var templateCode = 'at-001';

            axios({
                type: 'get',
                path: '/api/coursearrange/getcoursearranged',
                data: {
                    templateCode: templateCode,
                    roomCode: _this.roomCode
                },
                fn: function (result) {
                    //console.log(result);
                    result.forEach((item) => {
                        // 构造coursePeriods数据
                        for (let day of _this.coursePeriods) {
                            if (day.dayCode === item.courseWeekDay) {
                                _this.activePeriods_bak[day.dayCode].push(item.id);
                                day.activePeriods.push(item.id); //默认展开的时间段
                                day.periods.push({
                                    periodCode: item.id,
                                    periodName: item.coursePeriod,
                                    studentList: item.studentCourseArrangeList
                                });
                                break;
                            }
                        }
                    })
                    console.log(_this.coursePeriods);
                }
            })
        },

        // 添加排课学生入口
        addStudent(dayCode, dayName, periodName) {
            this.selectStudentDialog.title = '选择学生 [' + dayName + ' ' + periodName + ']';
            this.selectStudentDialog.curDayCode = dayCode;
            this.selectStudentDialog.curPeriodName = periodName;
            this.getPickCourseStudentsList();
            this.selectStudentDialog.isShow = true;
        },

        // 待排课学生列表
        getPickCourseStudentsList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getstudentstoselectCourse',
                fn: function (result) {
                    //console.log(result);
                    _this.selectStudentDialog.studentList = result;
                    //console.log(_this.selectStudentDialog);
                }
            })
        },

        // 提交排课（正式）
        pickStudents() {
            if (this.selectStudentDialog.selectedStudents.length === 0) {
                this.$message({
                    message: '请至少选择一位学生！',
                    type: 'warning'
                });
                return;
            }
            var caInfo = {
                'templateCode':'at-001',
                'roomCode': this.roomCode,
                'DayCode': this.selectStudentDialog.curDayCode,
                'PeriodName': this.selectStudentDialog.curPeriodName,
                'StudentList': []
            }
            caInfo.StudentList = [];
            for (let selectedItem of this.selectStudentDialog.selectedStudents) {
                //debugger
                if (!selectedItem.selectedCourseCount || !selectedItem.firstCourseDate) {
                    this.$message({
                        message: '请填写选中学生的课时数以及上课开始日期！',
                        type: 'warning'
                    });
                    return;
                }
                caInfo.StudentList.push({
                    'StudentCode': selectedItem.studentCode,
                    'StudentName': selectedItem.studentName,
                    'PackageCode': selectedItem.packageCode,
                    'CourseCategoryCode': selectedItem.courseCategoryCode,
                    'CourseCategoryName': selectedItem.courseCategoryName,
                    'CourseCount': selectedItem.selectedCourseCount,
                    'StartDate': selectedItem.firstCourseDate
                });
            }
            //console.log(caInfo);
            var _this = this;
            axios({
                type: 'post',
                path: '/api/coursearrange/postcoursearrange',
                data: caInfo,
                fn: function (result) {
                    if(result === 200)                  
                    {
                        _this.$message({
                            message: '排课成功！',
                            type: 'success'
                        });
                        _this.selectStudentDialog.isShow = false;
                    }
                }
            })
            //alert("待开发");
        },

        // 查看学生已选课程列表
        getStudentCourseList(studentCode, studentName, dayCode, dayName, periodName) {
            var _this = this;
            axios({
                type: 'get',
                path: 'api/course/getarrangedcourselist',
                data: {
                    studentCode: studentCode,
                    dayCode: dayCode,
                    coursePeriod: periodName
                },
                fn: function (result) {
                    result.forEach((item) => {
                        item.courseDate = item.courseDate.split('T')[0];
                    })
                    _this.studentCourseDialog.courseList = result;
                }
            })
            this.studentCourseDialog.isShow = true;
            this.studentCourseDialog.title = "学生排课列表  [" + studentName + "    " + dayName + "    " + periodName + "]";
        },

        closeAllPeriod(){
            for (let day of this.coursePeriods){
                day.activePeriods = []
            }
        },

        showAllPeriod(){
            for (let day of this.coursePeriods){
                day.activePeriods = this.activePeriods_bak[day.dayCode];
            }
        },

        removeStudent() {
            alert("待开发");
        },

        // 选择学生 checkbox
        handleStudentSelectionChange(allSelectedStudents) {
            this.selectStudentDialog.selectedStudents = allSelectedStudents;
        },
        // 选择课程 checkbox
        handleCourseListChange(allSelectedCourses){
            this.studentCourseDialog.selectedCourses = allSelectedCourses;
        },
        // 请假
        qingJiaCourse(studentCourseId) {
            var _this = this;
            axios({
                type: 'put',
                path: '/api/coursearrange/putqingjiasingle',
                data: {StudentCourseId: studentCourseId},
                fn: function (result) {
                    if(result === 200)                  
                    {
                        _this.$message({
                            message: '请假成功！',
                            type: 'success'
                        });
                    }
                }
            })
        },
        // 删除课程
        removeCourse(studentCourseId) {
            alert("待开发");
        },
        // 删除课程
        removeCourseBatch() {
            alert("待开发");
        },

        handleCommand(command) {
            //this.$message('click on item ' + command);
            if (command === "正式") {
                this.selectStudentDialog.isShow = true;
            }
        }
    }
};
</script>

<style lang="less" scoped>
.schedule-panel {
    overflow-x: auto;
    .schedule-area {
        min-width: 1200px;
        overflow-y: auto;
        overflow-x: hidden;
        .week-panel {
            height: 550px;
            max-height: 600px; // border: 1px solid #dfdfdf;
            border-left: 1px solid #dfdfdf;
            border-bottom: 1px solid #dfdfdf;
            border-top: 1px solid #dfdfdf;
            font-size: 14px;
            padding: 0;
            .title {
                width: 100%;
                text-align: center;
                font-weight: 600;
                height: 30px;
                line-height: 30px;
                background-color: #3bc5ff;
                border: 1px solid #3bc5ff;
                color: #fff;
                display: block;
                cursor: pointer;
            }
            .student-list-title {
                width: 100%;
                margin-left: 2px;
                padding-left: 5px;
                font-weight: 600;
                .student-title-right {
                    color: red;
                    float: right;
                    padding-right: 3px;
                }
            }
            .student-list-content {
                padding-left: 5px;
                margin-left: 3px;
                .student-item-left {
                    cursor: pointer;
                }
                .student-item-right {
                    float: right;
                    margin-right: 15px;
                }
            }
            .student-list-footer {
                float: right;
                margin-right: 15px;
                margin-top: 6px;
                margin-bottom: 6px;
            }
        }
    }
}

.footer-botton-area {
    width: 100%;
    height: 25px;
    margin-right: 20px;
    margin-top: 10px;
    .el-button {
        float: right;
        margin-right: 10px;
    }
}
</style>
