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
                            <a class="student-title-right" v-bind:class="{ 'full': period.studentList.length > 6,'free': period.studentList.length <= 6 }"> {{ period.thisWeekStudentCount }}/{{ period.studentList.length }}人</a>
                        </div>
                        <div class="student-list-content">
                            <ul>
                                <li v-for="student in period.studentList" v-bind:key="student.studentCode">
                                    <el-popover trigger="hover" placement="top">
                                        <p>姓名: 测试</p>
                                        <p>住址: 北京市昌平区</p>
                                        <div slot="reference" class="name-wrapper" style="display:inline">
                                            <el-tag :type="courseCategoryTag(student.courseCategoryCode)" size="mini">{{student.courseCategoryName}}</el-tag>
                                            <a class="student-item-left" @click="getStudentCourseList(student.studentCode, student.studentName,day.dayCode, day.dayName, period.periodName)">{{student.studentName}} <i v-if="student.isThisWeek == 'Y'" class="fa fa-check-square-o"></i></a>
                                        </div>
                                    </el-popover>
                                    <a class="student-item-right" v-show="setting.isShowRestCourseCount">
                                        {{student.courseRestCount}}节
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer" v-on:mouseenter="showPeriodActions($event)" v-on:mouseleave="HidPeriodActions($event)">
                            <el-button type="success" v-show="false" @click="showStudentsListPanel(day.dayCode, day.dayName,period.periodName)" size="mini">正 式</el-button>
                            <el-button type="primary" v-show="false" @click="showTempStudentsListCourse(day.dayCode, day.dayName,period.periodName)" size="mini">试 听</el-button>
                            <!-- <el-button type="warning" plain @click="removeStudent()" size="mini">试听</el-button> -->
                            <!-- <el-button  plain circle icon="el-icon-plus"  @click="removeStudent()" size="mini"></el-button> -->

                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">其他操作</p>
                <ul class="side-button-group">
                    <li>
                        <el-button plain icon="el-icon-refresh" type="success" size="mini" @click="refreshAll()">全部刷新</el-button>
                    </li>
                    <li>
                        <el-button plain icon="el-icon-date" type="warning" size="mini" @click="showHolidaySettingPanel()">放假安排</el-button>
                    </li>
                    <li>
                        <el-button plain :icon="setting.btnTogglePeriodIcon" type="primary" size="mini" @click="togglePeriod()">{{ setting.btnTogglePeriodName }}</el-button>
                    </li>
                    <li>
                        <el-button plain icon="el-icon-tickets" type="primary" size="mini" @click="toggleRestCourseCount()">{{ setting.btnRestCourseName }}</el-button>
                    </li>
                </ul>
            </div>
        </el-col>
    </el-row>

    <el-dialog :title="selectStudentDialog.title" :visible.sync="selectStudentDialog.isShow" :modal-append-to-body="false" :width="selectStudentDialog.width">
        <el-table :data="selectStudentDialog.studentList" max-height="400" size="mini" @selection-change="handleStudentSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="studentCode" label="学号" width="120"></el-table-column>
            <el-table-column property="studentName" label="姓名" width="70"></el-table-column>
            <el-table-column property="courseCategoryName" label="课程类别" width="80" align='center'></el-table-column>
            <el-table-column property="flexCourseCount" label="可选课时数" align='center' width="90"></el-table-column>
            <el-table-column property="selectedCourseCount" label="选择课时数" min-width="140">
                <template slot-scope="scope">
                    <el-input-number v-model="scope.row.selectedCourseCount" :min="1" :max="scope.row.flexCourseCount" label="描述文字" size="mini"></el-input-number>
                </template>
            </el-table-column>
            <el-table-column property="firstCourseDate" label="开始上课日期" min-width="210">
                <template slot-scope="scope">
                    <el-date-picker v-model="scope.row.firstCourseDate" type="date" size="mini" value-format="yyyy-MM-dd" placeholder="选择日期" :picker-options="selectStudentDialog.pickerDateOptions">
                    </el-date-picker>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button @click="submitStudents_xuanke()" type="success" size="small">确定</el-button>
            <el-button @click="selectStudentDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog :title="selectStudentTempDialog.title" :visible.sync="selectStudentTempDialog.isShow" :modal-append-to-body="false" :width="selectStudentTempDialog.width">
        <el-table :data="selectStudentTempDialog.studentList" max-height="400" size="mini" @selection-change="handleStudentTempSelectionChange">
            <el-table-column type="selection" width="30"></el-table-column>
            <el-table-column property="studentName" label="姓名" width="70"></el-table-column>
            <el-table-column property="courseDetail" label="课程类别" min-width="180" align='center'>
                <template slot-scope="scope">
                    <el-cascader expand-trigger="hover" :options="courseOptions" v-model="scope.row.selectedCourseOptions" size="mini">
                    </el-cascader>
                </template>
            </el-table-column>
            <el-table-column property="firstCourseDate" label="试听日期" min-width="200">
                <template slot-scope="scope">
                    <el-date-picker v-model="scope.row.firstCourseDate" type="date" size="mini" value-format="yyyy-MM-dd" placeholder="选择日期" :picker-options="selectStudentTempDialog.pickerDateOptions">
                    </el-date-picker>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button @click="pickTempStudent()" type="success" size="small">确定</el-button>
            <el-button @click="selectStudentTempDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog :title="studentCourseDialog.title" :visible.sync="studentCourseDialog.isShow" :modal-append-to-body="false" :width="studentCourseDialog.width">
        <el-table :data="studentCourseDialog.courseList" max-height="400" size="mini" @selection-change="handleCourseListChange">
            <el-table-column :selectable='courseCheckboxControl' type="selection" width="30"></el-table-column>
            <el-table-column type="index" :index="indexGernerate" width="40"> </el-table-column>
            <el-table-column property="courseDate" label="上课日期" width="110"></el-table-column>
            <el-table-column property="courseCategoryName" label="课程类别" width="80" align='center'></el-table-column>
            <el-table-column property="attendanceStatusName" label="状态" width="80" align='center'></el-table-column>
            <el-table-column prop="operation" align='left' label="操作" fixed="right" width="190">
                <template slot-scope='scope'>
                    <el-button v-if="scope.row.attendanceStatusCode == '09'" plain type="warning" icon='edit' size="mini" @click='qingJiaCourse(scope.row.studentCourseId)'>请假</el-button>
                    <el-button v-if="scope.row.attendanceStatusCode == '09'" plain type="danger" icon='edit' size="mini" @click='removeCourse(scope.row.studentCourseId)'>删除</el-button>
                    <el-button v-if="scope.row.attendanceStatusCode == '00'" plain type="success" icon='edit' size="mini" @click='restoreQingJia(scope.row.studentCourseId)'>撤销请假</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            <el-button type="danger" size="small" @click='removeCourseBatch()'>批量删除</el-button>
            <el-button @click="studentCourseDialog.isShow = false" size="small">取消</el-button>
        </div>
    </el-dialog>

    <el-dialog :title="holidayDialog.title" :visible.sync="holidayDialog.isShow" :modal-append-to-body="false" :width="holidayDialog.width">
        <el-table :data="holidayDialog.holidayList" max-height="400" size="mini">
            <el-table-column type="index" :index="indexGernerate" width="35"> </el-table-column>
            <el-table-column property="holidayDate" label="今年放假日期" min-width="120"></el-table-column>
            <el-table-column property="dayOfWeek" label="星期 ？" min-width="90"></el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="120">
                <template slot-scope='scope'>
                    <el-button plain type="danger" v-show="scope.row.showDelete" @click="removeHoliday(scope.row.holidayDate)" icon='edit' size="mini">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer-botton-area">
            选择添加放假日期：
            <el-date-picker type="dates" value-format="yyyy-MM-dd" v-model="holidayDialog.newHolidays" :picker-options="holidayDialog.pickerDateOptions" size="small"></el-date-picker>
            <el-button type="success" size="small" @click='submitHolidays()'>确定</el-button>
            <el-button @click="holidayDialog.isShow = false" size="small">取消</el-button>
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
                        //     thisWeekStudentCount: 'int',
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
            activePeriods_bak: {
                day1: [],
                day2: [],
                day3: [],
                day4: [],
                day5: [],
                day6: [],
                day7: [],
            },
            setting: {
                isShowRestCourseCount: false,
                btnRestCourseName: '显示剩余课时数',
                btnTogglePeriodName: '全部折叠',
                btnTogglePeriodIcon: 'el-icon-arrow-up',
                hidTogglePeriodStatus: true, // true : 当前展开
                periodActionsAreaControl: {
                    //period_1:true,
                }
            },
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
                        var curDay = parseInt(this.selectStudentDialog.curDayCode.replace('day', ''));
                        if (curDay === 7) {
                            curDay = 0;
                        }
                        return time.getDay() !== curDay;
                    }
                }
            },
            selectStudentTempDialog: {
                title: '',
                isShow: false,
                width: '600px',
                curDayCode: '',
                curPeriodName: '',
                studentList: [],
                selectedStudents: [],
                pickerDateOptions: {
                    firstDayOfWeek: 1,
                    disabledDate: (time) => { // 为了this指向 使用箭头函数 
                        var curDay = parseInt(this.selectStudentTempDialog.curDayCode.replace('day', ''));
                        if (curDay === 7) {
                            curDay = 0;
                        }
                        return time.getDay() !== curDay;
                    }
                }
            },
            studentCourseDialog: {
                title: '',
                isShow: false,
                width: '600px',
                curDayCode: '',
                curPeriodName: '',
                selectedCourses: [],
                courseList: []
            },
            holidayDialog: {
                title: "放假安排",
                isShow: false,
                width: '550px',
                holidayList: [],
                newHolidays: [],
                pickerDateOptions: {
                    firstDayOfWeek: 1,
                    disabledDate: (time) => {
                        return time.getTime() < Date.now();
                    }
                }
            },
            courseOptions: [{
                value: 'meishu',
                label: '美术',
                children: [{
                    value: 'meishu_00',
                    label: '国画'
                }, {
                    value: 'meishu_01',
                    label: '西画'
                }]
            }, {
                value: 'shufa',
                label: '书法',
                children: [{
                    value: 'shufa_00',
                    label: '毛笔'
                }, {
                    value: 'shufa_01',
                    label: '硬笔'
                }]
            }]
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
                    console.log(result);
                    result.forEach((item) => {
                        // 构造coursePeriods数据
                        for (let day of _this.coursePeriods) {
                            if (day.dayCode === item.courseWeekDay) {
                                _this.activePeriods_bak[day.dayCode].push(item.id);
                                _this.setting.periodActionsAreaControl["" + item.id] = false;
                                day.activePeriods.push(item.id); //默认展开的时间段
                                day.periods.push({
                                    periodCode: item.id,
                                    periodName: item.coursePeriod,
                                    thisWeekStudentCount: item.thisWeekStudentCount,
                                    studentList: item.studentCourseArrangeList
                                });
                                break;
                            }
                        }
                    })
                }
            })
        },

        // 刷新时间段内的排课信息
        refreshPeriodInfo(dayCode, periodName) {
            var _this = this;
            var templateCode = 'at-001';
            console.log("开始局部刷新...");
            axios({
                type: 'get',
                path: '/api/coursearrange/getarrangedinfobyperiod',
                data: {
                    templateCode: templateCode,
                    roomCode: _this.roomCode,
                    dayCode: dayCode,
                    periodName: periodName
                },
                fn: function (result) {
                    //console.log(result);
                    let count = 0;
                    result.forEach(item => {
                        item.isThisWeek == 'Y' ? count++ : '';
                    });
                    // 构建局部数据
                    for (let day of _this.coursePeriods) {
                        if (day.dayCode === dayCode) {
                            for (let p of day.periods) {
                                if (p.periodName === periodName) {
                                    p.thisWeekStudentCount = count;
                                    p.studentList = result;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            })
        },

        // 刷新所有
        refreshAll() {
            // 初始化数据
            for (let day of this.coursePeriods) {
                day.periods = [];
                day.activePeriods = [];
            }
            this.activePeriods_bak = {
                    day1: [],
                    day2: [],
                    day3: [],
                    day4: [],
                    day5: [],
                    day6: [],
                    day7: [],
                },

                this.getTemplatePeriod();
        },

        // 试听课程
        showTempStudentsListCourse(dayCode, dayName, periodName) {
            this.selectStudentTempDialog.title = '选择试听学生 [' + dayName + ' ' + periodName + ']';
            this.selectStudentTempDialog.curDayCode = dayCode;
            this.selectStudentTempDialog.curPeriodName = periodName;
            this.getShiTingStudentList();
            this.selectStudentTempDialog.isShow = true;
        },

        // 试听学生列表
        getShiTingStudentList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/gettempstudentstoselectCourse',
                fn: function (result) {
                    _this.selectStudentTempDialog.studentList = result;
                }
            })
        },

        // 提交试听学生选课信息 ？？
        submitStudents_shiting() {
            if (this.selectStudentTempDialog.selectedStudents.length === 0) {
                this.$message({
                    message: '请至少选择一位待试听学生！',
                    type: 'warning'
                });
                return;
            }
            var caInfo = {
                'templateCode': 'at-001',
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
            var dayCode = _this.selectStudentDialog.curDayCode;
            var periodName = _this.selectStudentDialog.curPeriodName;
            axios({
                type: 'post',
                path: '/api/coursearrange/postcoursearrange',
                data: caInfo,
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '排课成功！',
                            type: 'success'
                        });
                        _this.selectStudentDialog.isShow = false;
                    }
                }
            })
        },

        // 添加排课学生入口
        showStudentsListPanel(dayCode, dayName, periodName) {
            this.selectStudentDialog.title = '选择学生 [' + dayName + ' ' + periodName + ']';
            this.selectStudentDialog.curDayCode = dayCode;
            this.selectStudentDialog.curPeriodName = periodName;
            this.getPickCourseStudentList(dayCode, periodName);
            this.selectStudentDialog.isShow = true;
        },

        // 待排课学生列表
        getPickCourseStudentList(dayCode, periodName) {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getstudentstoselectCourse',
                data: {
                    dayCode: dayCode,
                    periodName: periodName
                },
                fn: function (result) {
                    _this.selectStudentDialog.studentList = result;
                }
            })
        },

        // 提交排课（正式）
        submitStudents_xuanke() {
            if (this.selectStudentDialog.selectedStudents.length === 0) {
                this.$message({
                    message: '请至少选择一位学生！',
                    type: 'warning'
                });
                return;
            }
            var caInfo = {
                'templateCode': 'at-001',
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
            var dayCode = _this.selectStudentDialog.curDayCode;
            var periodName = _this.selectStudentDialog.curPeriodName;
            axios({
                type: 'post',
                path: '/api/coursearrange/postcoursearrange',
                data: caInfo,
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '排课成功！',
                            type: 'success'
                        });
                        _this.selectStudentDialog.isShow = false;
                    }
                }
            })
        },

        // 查看学生已选课程列表
        getStudentCourseList(studentCode, studentName, dayCode, dayName, periodName) {
            this.studentCourseDialog.curDayCode = dayCode;
            this.studentCourseDialog.curPeriodName = periodName;
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

        showPeriodActions(event) {
            for (let child of event.target.children) {
                child.style.display = "block";
            }
        },
        HidPeriodActions(event) {
            for (let child of event.target.children) {
                child.style.display = "none";
            }
        },

        togglePeriod() {
            if (this.setting.hidTogglePeriodStatus) {
                // 折叠
                for (let day of this.coursePeriods) {
                    day.activePeriods = []
                }
                this.setting.hidTogglePeriodStatus = false;
                this.setting.btnTogglePeriodName = "全部展开";
                this.setting.btnTogglePeriodIcon = "el-icon-arrow-down";
            } else {
                // 展开
                for (let day of this.coursePeriods) {
                    day.activePeriods = this.activePeriods_bak[day.dayCode];
                }
                this.setting.hidTogglePeriodStatus = true;
                this.setting.btnTogglePeriodName = "全部折叠";
                this.setting.btnTogglePeriodIcon = "el-icon-arrow-up";
            }
        },

        toggleRestCourseCount() {
            if (this.setting.isShowRestCourseCount) {
                this.setting.isShowRestCourseCount = false;
                this.setting.btnRestCourseName = "显示剩余课时数";
            } else {
                this.setting.isShowRestCourseCount = true;
                this.setting.btnRestCourseName = "隐藏剩余课时数";
            }
        },

        courseCheckboxControl(row, index) {
            if (row.attendanceStatusCode == '09') {
                return 1;
            } else {
                return 0;
            }
        },

        // 选择学生(正式) checkbox
        handleStudentSelectionChange(allSelectedStudents) {
            this.selectStudentDialog.selectedStudents = allSelectedStudents;
        },
        // 选择学生(试听) checkbox
        handleStudentTempSelectionChange(allSelectedStudents) {
            this.selectStudentTempDialog.selectedStudents = allSelectedStudents;
        },
        // 选择课程 checkbox
        handleCourseListChange(allSelectedCourses) {
            this.studentCourseDialog.selectedCourses = allSelectedCourses;
        },
        // 请假
        qingJiaCourse(studentCourseId) {
            var _this = this;
            let dayCode = _this.studentCourseDialog.curDayCode;
            let periodName = _this.studentCourseDialog.curPeriodName;
            axios({
                type: 'put',
                path: '/api/coursearrange/putqingjiasingle',
                data: {
                    StudentCourseId: studentCourseId
                },
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '请假成功！',
                            type: 'success'
                        });
                        _this.studentCourseDialog.isShow = false;
                    }
                }
            });
        },

        restoreQingJia(studentCourseId) {
            var _this = this;
            let dayCode = _this.studentCourseDialog.curDayCode;
            let periodName = _this.studentCourseDialog.curPeriodName;
            axios({
                type: 'put',
                path: '/api/coursearrange/putrestoreqingjiasingle',
                data: {
                    StudentCourseId: studentCourseId
                },
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '撤销成功！',
                            type: 'success'
                        });
                        _this.studentCourseDialog.isShow = false;
                    }
                }
            });
        },

        // 删除课程
        removeCourse(studentCourseId) {
            var _this = this;
            let dayCode = _this.studentCourseDialog.curDayCode;
            let periodName = _this.studentCourseDialog.curPeriodName;
            axios({
                type: 'put',
                path: '/api/coursearrange/removecoursesingle',
                data: {
                    StudentCourseId: studentCourseId
                },
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '删除成功！',
                            type: 'success'
                        });
                        _this.studentCourseDialog.isShow = false;
                    }
                }
            });
        },

        // 批量删除课程
        removeCourseBatch() {
            var arrStudentCourseId = [];
            this.studentCourseDialog.selectedCourses.forEach((item) => {
                arrStudentCourseId.push(item.studentCourseId);
            })
            if (arrStudentCourseId.length === 0) {
                this.$message({
                    message: '请至少选择一节课程！',
                    type: 'warning'
                });
                return;
            }
            var _this = this;
            let dayCode = _this.studentCourseDialog.curDayCode;
            let periodName = _this.studentCourseDialog.curPeriodName;
            axios({
                type: 'put',
                path: '/api/coursearrange/removecoursebatch',
                data: arrStudentCourseId,
                fn: function (result) {
                    if (result === 200) {
                        _this.refreshPeriodInfo(dayCode, periodName);
                        _this.$message({
                            message: '全部删除成功！',
                            type: 'success'
                        });
                        _this.studentCourseDialog.isShow = false;
                    }
                }
            });
        },

        // 放假安排
        showHolidaySettingPanel() {
            var _this = this;
            var today = _this.getNowFormatDate();
            _this.holidayDialog.newHolidays = [];
            axios({
                type: 'get',
                path: '/api/coursearrange/getholidays',
                fn: function (result) {
                    result.forEach(item => {
                        item.holidayDate = item.holidayDate.split('T')[0];
                        if (item.holidayDate <= today) {
                            item.showDelete = false;
                        } else {
                            item.showDelete = true;
                        }
                    });
                    _this.holidayDialog.holidayList = result;
                }
            });
            _this.holidayDialog.isShow = true;
        },

        indexGernerate(index) {
            return index + 1;
        },

        submitHolidays() {
            var _this = this;
            if (_this.holidayDialog.newHolidays.length == 0) {
                _this.$message({
                    message: '请选择放假日期',
                    type: 'warning'
                });
                return;
            }
            let listHolidays = [];
            let weekday = '';
            _this.holidayDialog.newHolidays.forEach(item => {
                weekday = _this.getDayOfWeek(item);
                listHolidays.push({
                    HolidayDate: item,
                    DayOfWeek: weekday
                });
            });
            axios({
                type: 'post',
                path: '/api/coursearrange/addholidays',
                data: listHolidays,
                fn: function (result) {
                    if (result === 200) {
                        _this.$message({
                            message: '添加放假日期成功',
                            type: 'success'
                        });
                        _this.refreshAll();
                        _this.holidayDialog.isShow = false;
                    }
                }
            });
        },

        removeHoliday(day) {
            var _this = this;
            axios({
                type: 'delete',
                path: '/api/coursearrange/removeholiday',
                data: {
                    strDay: day
                },
                fn: function (result) {
                    console.log("remove holiday result: " + result);
                    if (result === 200) {
                        _this.$message({
                            message: '删除假期成功',
                            type: 'success'
                        });
                        _this.refreshAll();
                        _this.holidayDialog.isShow = false;
                    }
                }
            });
        },

        getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var seperator2 = ":";
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate;
            return currentdate;
        },

        getDayOfWeek(theDay) {
            let week = '';
            let code = new Date(theDay).getDay();
            switch (code) {
                case 0:
                    week = '星期日';
                    break;
                case 1:
                    week = '星期一';
                    break;
                case 2:
                    week = '星期二';
                    break;
                case 3:
                    week = '星期三';
                    break;
                case 4:
                    week = '星期四';
                    break;
                case 5:
                    week = '星期五';
                    break;
                case 6:
                    week = '星期六';
                    break;
                default:
                    break;
            }
            return week;
        },

        // handleCommand(command) {
        //     //this.$message('click on item ' + command);
        //     if (command === "正式") {
        //         this.selectStudentDialog.isShow = true;
        //     }
        // }
        courseCategoryTag(categoryCode) {
            let type = '';
            switch (categoryCode) {
                case 'meishu':
                    type = 'success'
                    break;
                case 'shufa':
                    type = ''
                    break;
            }
            return type;
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
                    float: right;
                    padding-right: 3px;
                }
                .full {
                    color: #f56c6c;
                }
                .free {
                    color: #E6A23C;
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
                margin-right: 8px;
                margin-top: 3px;
                margin-bottom: 3px;
                height: 28px;
                min-width: 160px;
                button {
                    float: right;
                    margin-left: 10px;
                }
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

.side-button-group {
    li {
        margin-top: 10px;
        margin-left: 5px
    }
}
</style>
