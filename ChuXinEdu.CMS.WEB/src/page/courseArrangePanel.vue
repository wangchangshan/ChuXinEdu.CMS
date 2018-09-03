<template>
<div class="schedule-panel">
    <el-row class="schedule-area row">
        <el-col :span="3" v-for="day in coursePeriods" v-bind:key="day.dayCode">
            <div class="week-panel">
                <p class="title">{{day.dayName}}</p>
                <el-collapse v-model="active_titles_Monday">
                    <el-collapse-item v-for="period in day.periods" v-bind:key="period.code" :name="period.code">
                        <div slot="title" class="student-list-title">
                            <a>{{period.name}}</a>
                            <a class="student-title-right">共{{ period.studentList.length }}人</a>
                        </div>
                        <div class="student-list-content">
                            <ul>
                                <li v-for="student in period.studentList" v-bind:key="student.studentCode">
                                    <el-popover trigger="hover" placement="top">
                                        <p>姓名: 李世民</p>
                                        <p>住址: 北京市昌平区</p>
                                        <div slot="reference" class="name-wrapper" style="display:inline">
                                            <a class="student-item-left" @click="showStudentCourseList()">{{student.studentName}} </a>
                                        </div>
                                    </el-popover>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">{{student.courseCategoryName + student.courseRestCount}}节</el-tag>
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer">
                            <el-button type="warning" @click="removeStudent()" icon="el-icon-plus" size="mini">试听</el-button>
                            <el-button type="success" @click="addStudent()" icon="el-icon-plus" size="mini">正式</el-button>
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

    <el-dialog title="选择学生 [星期一 16:00-17:30]" :visible.sync="selectStudentDialog.isShow" :modal-append-to-body="false">
        <el-table :data="selectStudentDialog.studentList" max-height="400" @selection-change="handleStudentSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="student_code" label="学号" width="100"></el-table-column>
            <el-table-column property="student_name" label="姓名" width="90"></el-table-column>
            <el-table-column property="student_course" label="课程类别" width="120" align='center'></el-table-column>
            <el-table-column property="student_course_remain_count" label="剩余课时数" align='center' width="100"></el-table-column>
            <el-table-column property="student_selected_course_count" label="选择课时数" min-width="100">
                <template slot-scope="scope">
                    <el-input-number v-model="scope.row.student_selected_course_count" :min="1" :max="scope.row.student_course_remain_count" label="描述文字" size="small"></el-input-number>
                </template>
            </el-table-column>
            <el-table-column property="first_start_date" label="开始上课日期" min-width="200">
                <template slot-scope="scope">
                    <el-date-picker v-model="scope.row.first_start_date" type="date" size="small" placeholder="选择日期" :picker-options="selectStudentDialog.pickerStartDateOptions">
                    </el-date-picker>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button @click="submitStudents()" type="success">确定</el-button>
            <el-button @click="selectStudentDialog.isShow = false">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog title="学生排课列表 李世民 星期一 16:00-17:30" :visible.sync="editStudentDialog.isShow" :modal-append-to-body="false">
        <el-table :data="editStudentDialog.studentList" max-height="400" @selection-change="handleStudentSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="course_date" label="上课日期" width="100"></el-table-column>
            <el-table-column property="course_type" label="课程类别" width="120" align='center'></el-table-column>
            <el-table-column property="course_status" label="状态" width="120" align='center'>
                <template slot-scope="scope">
                    <el-tag :disable-transitions="false">
                        {{scope.row.course_status}}
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
            <el-button type="success">确定</el-button>
            <el-button @click="editStudentDialog.isShow = false">取消</el-button>
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
                    ]
                },
                {
                    dayCode: 'day2',
                    dayName: '星期二',
                    periods: []
                },
                {
                    dayCode: 'day3',
                    dayName: '星期三',
                    periods: []
                },
                {
                    dayCode: 'day4',
                    dayName: '星期四',
                    periods: []
                },
                {
                    dayCode: 'day5',
                    dayName: '星期五',
                    periods: []
                },
                {
                    dayCode: 'day6',
                    dayName: '星期六',
                    periods: []
                },
                {
                    dayCode: 'day7',
                    dayName: '星期日',
                    periods: []
                },
            ],
            // coursePeriods: {
            //     day1: [],
            //     day2: [],
            //     day3: [],
            //     day4: [],
            //     day5: [],
            //     day6: [],
            //     day7: [],
            // },

            holidays: [],
            active_period: {
                day1: [],
                day2: [],
                day3: [],
                day4: [],
                day5: [],
                day6: [],
                day7: [],
            },
            active_titles_Monday: ["1", "2"],
            selectStudentDialog: {
                isShow: false,
                studentList: [{
                        student_code: "200808001",
                        student_name: "段延庆",
                        student_course: "书法",
                        student_course_remain_count: 20,
                        student_selected_course_count: 20,
                        first_start_date: ""
                    },
                    {
                        student_code: "200808001",
                        student_name: "段延庆",
                        student_course: "书法",
                        student_course_remain_count: 20,
                        student_selected_course_count: 20,
                        first_start_date: ""
                    },
                    {
                        student_code: "200808001",
                        student_name: "段延庆",
                        student_course: "书法",
                        student_course_remain_count: 20,
                        student_selected_course_count: 20,
                        first_start_date: ""
                    },
                    {
                        student_code: "200808001",
                        student_name: "段延庆",
                        student_course: "书法",
                        student_course_remain_count: 20,
                        student_selected_course_count: 20,
                        first_start_date: ""
                    }
                ],
                selectedStudents: [],
                pickerStartDateOptions: {
                    disabledDate(time) {
                        return time.getDay() !== 1;
                    }
                }
            },
            editStudentDialog: {
                isShow: false,
                student_code: "",
                studentList: [{
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
                    console.log(result);
                    result.forEach((item) => {
                        // 构造coursePeriods数据
                        for (let day of _this.coursePeriods) {
                            if (day.dayCode === item.courseWeekDay) {
                                day.periods.push({
                                    periodCode: item.id,
                                    periodName: item.coursePeriod,
                                    studentList: item.studentCourseArrangeList
                                });
                                break;
                            }
                        }
                        console.log(_this.coursePeriods);
                    })
                }
            })
        },
        addStudent() {
            //alert("添加页面，展示还没有排课，或者没有排完全部课的学生列表。列表中展示学生姓名，（照片？），未排课程类型， 未排课时数目")
            this.selectStudentDialog.isShow = true;
        },
        removeStudent() {
            alert("待开发");
        },
        showStudentCourseList() {
            //alert('显示当前学生当前时间段的所有课程列表')
            this.editStudentDialog.isShow = true;
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
