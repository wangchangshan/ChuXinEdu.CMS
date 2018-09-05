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
                                            <a class="student-item-left" @click="getStudentCourseList(student.studentCode, day.dayCode, period.periodName)">{{student.studentName}} </a>
                                        </div>
                                    </el-popover>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">{{student.courseCategoryName + student.courseRestCount}}节</el-tag>
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer">
                            <el-button type="warning" plain @click="removeStudent()" size="mini">试听</el-button>
                            <el-button type="primary" plain @click="addStudent(day.dayName,period.periodName)" size="mini">正式</el-button>
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-col>
        <!-- <el-col :span="3">
            <div class="week-panel">
                <p class="title">按钮区域</p>
                <el-button type="success" size="mini">假期安排</el-button>
                <el-date-picker type="dates" v-model="holidays" placeholder="选择放假日期">
                </el-date-picker>
            </div>
        </el-col> -->
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
                    <el-date-picker v-model="scope.row.first_start_date" type="date" size="small" placeholder="选择日期" :picker-options="selectStudentDialog.pickerStartDateOptions">
                    </el-date-picker>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button @click="submitStudents()" type="success" size="small">确定</el-button>
            <el-button @click="selectStudentDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog title="学生排课列表  李世民  星期一  16:00-17:30" :visible.sync="studentCourseDialog.isShow" :modal-append-to-body="false" :width="studentCourseDialog.width">
        <el-table :data="studentCourseDialog.courseList" max-height="400" @selection-change="handleStudentSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="CourseDate" label="上课日期" width="100"></el-table-column>
            <el-table-column property="CourseCategoryName" label="课程类别" width="120" align='center'></el-table-column>
            <el-table-column property="attendance_status_code" label="状态" width="120" align='center'>
                <template slot-scope="scope">
                    <el-tag :disable-transitions="false">
                        {{scope.row.AttendanceStatusName}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" min-width="300">
                <template slot-scope='scope'>
                    <el-button type="warning" icon='edit' size="small" @click='removeCurrentCourse(scope.row)'>请假顺延</el-button>
                    <el-button type="primary" icon='edit' size="small" @click='removeCurrentCourse(scope.row)'>调课</el-button>
                    <el-button type="danger" icon='edit' size="small" @click='removeCurrentCourse(scope.row)'>删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button type="success" size="small">确定</el-button>
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
                    activePeriods:[]
                },
                {
                    dayCode: 'day2',
                    dayName: '星期二',
                    periods: [],
                    activePeriods:[]
                },
                {
                    dayCode: 'day3',
                    dayName: '星期三',
                    periods: [],
                    activePeriods:[]
                },
                {
                    dayCode: 'day4',
                    dayName: '星期四',
                    periods: [],
                    activePeriods:[]
                },
                {
                    dayCode: 'day5',
                    dayName: '星期五',
                    periods: [],
                    activePeriods:[]
                },
                {
                    dayCode: 'day6',
                    dayName: '星期六',
                    periods: [],
                    activePeriods:[]
                },
                {
                    dayCode: 'day7',
                    dayName: '星期日',
                    periods: [],
                    activePeriods:[]
                },
            ],
            holidays: [],
            selectStudentDialog: {
                title: '',
                isShow: false,
                width: '850px',
                studentList: [],
                selectedStudents: [],
                pickerStartDateOptions: {
                    disabledDate(time) {
                        return time.getDay() !== 1;
                    }
                }
            },
            studentCourseDialog: {
                titile:'',
                isShow: false,
                width: '750px',
                student_code: "",
                courseList: [{
                        course_date: "2008-08-06",
                        course_type: "国画",
                        course_status: "已上课"
                    },
                    {
                        course_date: "2008-08-13",
                        course_type: "国画",
                        course_status: "请假"
                    },
                    {
                        course_date: "2008-08-20",
                        course_type: "国画",
                        course_status: "待上课"
                    },
                    {
                        course_date: "2008-08-27",
                        course_type: "国画",
                        course_status: "待上课"
                    }
                ]
            }
        };
    },
    created() {
        this.getTemplatePeriod();
    },
    methods: {
        getTemplatePeriod() {
            var _this = this;
            var templateCode = 'at-001';

            axios({
                type: 'get',
                path: '/api/coursearrange',
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
        getPickCourseStudentsList(){
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getstudentstoselectCourse',
                fn: function (result) {
                    //console.log(result);
                    _this.selectStudentDialog.studentList = result;
                    console.log(_this.selectStudentDialog);
                }
            })
        },
        
        getStudentCourseList(studentCode, dayCode, periodName) {
            // 显示当前学生当前时间段的所有课程列表'
            var _this = this;
            axios({
                type: 'get',
                path: 'api/course/getarrangedcourselist',
                data:{
                    studentCode: studentCode,
                    dayCode: dayCode,
                    coursePeriod: periodName
                },
                fn: function (result) {
                    //console.log(result);
                    _this.studentCourseDialog.courseList = result;
                    console.log(_this.studentCourseDialog);
                }
            })
            this.studentCourseDialog.isShow = true;
        },
        addStudent(dayName, periodName) {
            this.selectStudentDialog.title = '选择学生 ['+dayName+' '+periodName+']';
            this.getPickCourseStudentsList();
            //alert("添加页面，展示还没有排课，或者没有排完全部课的学生列表。列表中展示学生姓名，（照片？），未排课程类型， 未排课时数目")
            this.selectStudentDialog.isShow = true;
        },
        removeStudent() {
            alert("待开发");
        },
        handleStudentSelectionChange(allItems) {
            this.selectStudentDialog.selectedStudents = allItems;
        },
        submitStudents() {
            alert("待开发");
        },
        removeCurrentCourse() {
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
