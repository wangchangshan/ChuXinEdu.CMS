<template>
<div class="schedule-panel">
    <el-row class="schedule-area row">
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期一</p>
                <el-collapse v-model="active_titles_Monday">
                    <el-collapse-item name="1">
                        <div slot="title" class="student-list-title">
                            <a>16:00-17:30</a>
                            <a class="student-title-right">共4人</a>
                        </div>
                        <div class="student-list-content">
                            <ul>
                                <li>
                                    <a>郭靖</a>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">国画10节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>黄蓉</a>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">国画10节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>欧阳锋</a>
                                    <a class="student-item-right">
                                        <el-tag type="" size="mini">西画20节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>令狐冲</a>
                                    <a class="student-item-right">
                                        <el-tag type="" size="mini">西画9节</el-tag>
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer">
                            <el-button type="danger" @click="removeStudent()" icon="el-icon-delete" size="mini" circle></el-button>
                            <el-button type="success" @click="addStudent()" icon="el-icon-plus" size="mini" circle></el-button>
                        </div>
                    </el-collapse-item>
                    <el-collapse-item title="反馈" name="2">
                        <div slot="title" class="student-list-title">
                            <a>17:30-19:00</a>
                            <a class="student-title-right">共5人</a>
                        </div>
                        <div class="student-list-content">
                            <ul>
                                <li>
                                    <el-popover trigger="hover" placement="top">
                                        <p>姓名: 李世民</p>
                                        <p>住址: 北京市昌平区</p>
                                        <div slot="reference" class="name-wrapper" style="display:inline">
                                            <a class="student-item-left" @click="showStudentCourseList()">李世民 </a>
                                        </div>
                                    </el-popover>

                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">国画10节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>嬴政</a>
                                    <a class="student-item-right">
                                        <el-tag type="success" size="mini">国画10节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>杨广</a>
                                    <a class="student-item-right">
                                        <el-tag type="" size="mini">西画20节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>曹操</a>
                                    <a class="student-item-right">
                                        <el-tag type="" size="mini">西画9节</el-tag>
                                    </a>
                                </li>
                                <li>
                                    <a>朱元璋</a>
                                    <a class="student-item-right">
                                        <el-tag type="" size="mini">西画9节</el-tag>
                                    </a>
                                </li>
                            </ul>
                        </div>
                        <div class="student-list-footer">
                            <el-button type="danger" @click="removeStudent()" icon="el-icon-delete" size="mini" circle></el-button>
                            <el-button type="success" @click="addStudent()" icon="el-icon-plus" size="mini" circle></el-button>
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期二</p>
                {{testContent}}
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期三</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期四</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期五</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期六</p>
            </div>
        </el-col>
        <el-col :span="3">
            <div class="week-panel">
                <p class="title">星期日</p>
            </div>
        </el-col>
        <!-- <el-col :span="2">
            <div class="week-panel">
                <p class="title">按钮以及待排课学生信息区域</p>
            </div>
        </el-col> -->
    </el-row>
   
    <el-dialog title="选择学生" :visible.sync="selectStudentDialog.isShow" :modal-append-to-body="false">
        <el-table :data="selectStudentDialog.studentList">
            <el-table-column property="student_name" label="姓名" width="100"></el-table-column>
            <el-table-column property="student_course" label="课程类别"></el-table-column>
            <el-table-column property="student_course_remain_count" label="剩余课时数"></el-table-column>
             <el-table-column property="student_selected_course_count" label="选择课时数">
                 <template slot-scope="scope">
                        <el-input-number v-model="scope.row.student_selected_course_count" :min="1" :max="80" label="描述文字"></el-input-number>
                    </template> 
             </el-table-column>
        </el-table>
    </el-dialog>
</div>
</template>

<script>
export default {
    data() {
        return {
            testContent: '',
            active_titles_Monday: ['1', '2'],
            selectStudentDialog:{
                isShow: false,
                studentList: [{
                    student_name:"段延庆",
                    student_course:"书法",
                    student_course_remain_count:20,
                    student_selected_course_count:20
                }]
            }
        }
    },
    props: {
        currentRoomCode: [String]
    },
    watch: {
        // 监听属性的变化，可以接收参数;
        currentRoomCode(roomCode) {
            this.initRoomCourseArrangeInfo(roomCode);
        }
    },
    methods: {
        initRoomCourseArrangeInfo(roomCode) {
            // axios get data info            
            this.testContent = 'this is ' + roomCode;
        },
        addStudent() {
            //alert("添加页面，展示还没有排课，或者没有排完全部课的学生列表。列表中展示学生姓名，（照片？），未排课程类型， 未排课时数目")
            this.selectStudentDialog.isShow = true;
        },
        removeStudent() {
            alert("待开发")
        },
        showStudentCourseList() {
            alert('显示当前学生当前时间段的所有课程列表')
        }
    }
}
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
                    padding-right: 3px
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
                margin-right: 20px;
                margin-top: 6px;
                margin-bottom: 6px;
            }
        }
    }
}
</style>
