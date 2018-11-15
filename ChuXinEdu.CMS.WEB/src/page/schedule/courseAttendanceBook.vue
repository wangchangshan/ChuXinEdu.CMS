<template>
<!-- <div>展示今日以及过往没有签到的学生上课列表</div> -->
<div class="fillcontain">
    <div class="table_container">
        <el-table :data="attendanceList" :span-method="objectSpanMethod" v-loading="loading" border :max-height="tableHeight" @selection-change="handleSelectionChange" size='mini'>
            <el-table-column prop="courseDate" label="上课日期" align='center' min-width="140">
                <template slot-scope='scope'>
                    {{ scope.row.courseDate + ' ' + scope.row.weekName }}
                </template>
            </el-table-column>
            <el-table-column prop="coursePeriod" label="上课时间段" align='center' min-width="140">
            </el-table-column>
            <el-table-column type="selection" label="选择" align='center' width="60"></el-table-column>
            <el-table-column prop="studentName" label="姓名" align='center' min-width="120">
                <template slot-scope='scope'>
                    {{ scope.row.studentName + ' (' + scope.row.courseType + ')' }}
                </template>
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="100">
                <template slot-scope="scope">
                    <el-select v-model="scope.row.courseFolderCode" size='mini'>
                        <el-option v-for="item in $store.getters['course_folder_' + scope.row.courseCategoryCode]" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column prop="" label="上课教师" align='center' min-width="140">
                <template slot-scope='scope'>
                    <el-select v-model="scope.row.teacherCode" placeholder="选择上课教师" size='mini'>
                        <el-option v-for="item in courseTeachers[scope.row.courseFolderCode]" :key="item.teacherCode" :label="item.teacherName" :value="item.teacherCode">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column prop="" label="课程主题" align='center' min-width="140">
                <template slot-scope='scope'>
                    <el-input size="mini" v-model="scope.row.courseSubject" placeholder="请输入课程主题"></el-input>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='left' label="操作" fixed="right" width="160">
                <template slot-scope='scope'>
                    <el-button type="warning" icon='edit' size="mini" @click='qingJiaCourse(scope.row.studentCourseId)'>请假</el-button>
                    <el-button type="success" icon='edit' size="mini" @click='showSingleSignInDialog(scope.row)'>签到</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <div class="footer_container">
        <el-button size="small" type="success" icon="" @click='submitBatchSignIn()'>批量签到</el-button>
    </div>

    <el-dialog :title="signInDialog.title" :visible.sync="signInDialog.isShow" :width="signInDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="courseInfo" :model="signInDialog.courseInfo" :rules="signInDialog.studentCourseInfoRules" :label-width="signInDialog.formLabelWidth" :label-position='signInDialog.labelPosition' size="mini" label-suffix='：' style="margin-right:20px">
                <el-form-item label="姓名">
                    {{signInDialog.courseInfo.studentName}}
                </el-form-item>
                <el-form-item label="上课时间">
                    {{signInDialog.courseInfo.courseDate + " " + signInDialog.courseInfo.weekName + "  [" + signInDialog.courseInfo.coursePeriod+ "]" }}
                </el-form-item>
                <el-form-item label="课程类别">
                    <el-select v-model="signInDialog.courseInfo.courseFolderCode" size='mini'>
                        <el-option v-for="item in $store.getters['course_folder_' + signInDialog.courseInfo.courseCategoryCode]" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item prop="teacherCode" label="上课教师">
                    <el-select v-model="signInDialog.courseInfo.teacherCode" placeholder="选择上课教师" size='mini'>
                        <el-option v-for="item in courseTeachers[signInDialog.courseInfo.courseFolderCode]" :key="item.teacherCode" :label="item.teacherName" :value="item.teacherCode">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item prop="imgDesc" label="课程主题">
                    <el-input v-model="signInDialog.courseInfo.imgDesc"></el-input>
                </el-form-item>
                <el-form-item label="作品花费课时">
                    <el-input-number v-model="signInDialog.courseInfo.imgCost" :min="1" size="mini"></el-input-number>
                </el-form-item>
                <el-form-item label="作品上传">
                    <el-upload class="upload-demo" :multiple="uploadPanel.multiple" :action="uploadPanel.actionUrl" :data="uploadPanel.params" :on-preview="handleImgPreview" :on-remove="handleImgRemove" :before-upload="beforeUpload" :on-success="uploadSuccess" :file-list="uploadPanel.thumbnailList" list-type="picture">
                        <el-button size="mini" type="primary">点击上传</el-button>
                    </el-upload>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="cancelSignIn()" size="small">取 消</el-button>
                    <el-button type="primary" @click="submitSignIn('courseInfo')" size="small">签 到</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios,
    dicHelper,
    dateHelper
} from '@/utils/index'

export default {
    data() {
        return {
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 63,

            attendanceList: [],
            selectedCourses: [],

            signInDialog: {
                title: '签到详细信息',
                isShow: false,
                labelPosition: 'right',
                formLabelWidth: '120px',
                width: '600px',
                courseInfo: {
                    courseId: 0,
                    studentCode: '',
                    studentName: '',
                    courseDate: '',
                    weekName: '',
                    coursePeriod: '',
                    courseFolderCode: '',
                    courseCategoryCode: '',
                    teacherCode: '',
                    imgDesc: '',
                    imgCost: ''
                },
                studentCourseInfoRules: {
                    teacherCode: [{
                        required: true,
                        message: '请选择上课教师',
                        trigger: 'change'
                    }],
                    imgDesc: [{
                        required: true,
                        message: '请输入课程主题',
                        trigger: 'blur'
                    }]
                }
            },
            uploadPanel: {
                multiple: true,
                actionUrl: '/api/upload/uploadartwork',
                params: {
                    courseId: '',
                    studentCode: '',
                    studentName: '',
                    uid: ''
                },
                fileCount: 0,
                thumbnailList: [],
                fileUIds: []
            },
            courseTeachers: {},
            dateRowSpanArray: [],
            timeRowSpanArray: []
        }
    },
    created() {
        this.getCourseTeacherList();
        this.getAttendanceList();
    },
    methods: {
        getCourseTeacherList(){
            axios({
                type: 'get',
                path: '/api/teacher/getcourseteacherlist',
                fn: result => {
                    for(let t of result) {
                        if(this.courseTeachers.hasOwnProperty(t.role_code)){
                            this.courseTeachers[t.role_code].push({
                                teacherCode: t.teacher_code,
                                teacherName: t.teacher_name
                            });
                        }
                        else{
                            this.courseTeachers[t.role_code] = [{
                                teacherCode: t.teacher_code,
                                teacherName: t.teacher_name
                            }];
                        }
                    }
                }
            });
        },
        getAttendanceList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/course/getattendancelist',
                fn: function (result) {
                    for (let item of result) {
                        item.courseDate = item.courseDate.split('T')[0];
                        item.weekName = dateHelper.getWeekNameByCode(item.courseWeekDay);
                    }
                    _this.attendanceList = result;
                    _this.$store.commit('SET_TO_RECORD', result.length);
                    _this.getRowSpanInfo();
                }
            });
        },

        cancelSignIn() {
            this.uploadPanel.thumbnailList = [];
            this.signInDialog.isShow = false;
        },        

        showSingleSignInDialog(row) {
            this.signInDialog.isShow = true;
            this.signInDialog.courseInfo = {
                courseId: row.studentCourseId,
                studentCode: row.studentCode,
                studentName: row.studentName,
                courseDate: row.courseDate,
                weekName: row.weekName,
                courseFolderCode: row.courseFolderCode,
                courseCategoryCode: row.courseCategoryCode,
                coursePeriod: row.coursePeriod,
                teacherCode: row.teacherCode,
                imgDesc: row.courseSubject
            };
            this.uploadPanel.params = {
                courseId: row.studentCourseId,
                studentCode: row.studentCode,
                studentName: row.studentName,
            }
        },

        submitSignIn(courseForm) {
            let _this = this;
            this.$refs[courseForm].validate((valid) => {
                if (valid) {
                    if (_this.uploadPanel.fileCount > 0) {
                        if (!_this.signInDialog.courseInfo.imgCost) {
                            _this.$message({
                                message: '请填写作品描述和花费课时数',
                                type: 'warning'
                            });
                            return;
                        }
                    }

                    let courseId = _this.signInDialog.courseInfo.courseId;
                    let studentCode = _this.signInDialog.courseInfo.studentCode;
                    let teacherCode = _this.signInDialog.courseInfo.teacherCode;
                    let folderCode = _this.signInDialog.courseInfo.courseFolderCode;
                    let folderName = dicHelper.getLabelByValue(this.$store.getters['course_folder'], folderCode);
                    let teacherName = _this.getTeacherNameByCode(folderCode, teacherCode);
                    let fileUIds = _this.uploadPanel.fileUIds;
                    let imgCost = _this.signInDialog.courseInfo.imgCost;
                    let title = _this.signInDialog.courseInfo.imgDesc;

                    let course = {
                        CourseListId: courseId,
                        StudentCode: studentCode,
                        TeacherCode: teacherCode,
                        TeacherName: teacherName,
                        FileUIds: fileUIds,
                        CostCount: imgCost,
                        Title: title,
                        CourseFolderCode: folderCode,
                        CourseFolderName: folderName
                    }

                    axios({
                        type: 'put',
                        path: '/api/course/putsignin',
                        data: course,
                        fn: function (result) {
                            if (result == 1200) {
                                _this.$message({
                                    message: '签到成功',
                                    type: 'success'
                                });
                                _this.uploadPanel.thumbnailList = [];
                                _this.signInDialog.isShow = false;
                                _this.removeTableRow(courseId);
                                _this.$store.commit('REDUCE_TO_RECORD', 1);
                            }
                        }
                    });
                }
            });

        },

        submitBatchSignIn() {
            if (this.selectedCourses.length === 0) {
                this.$message({
                    message: '请选择至少一条记录',
                    type: 'warning'
                });
                return;
            }
            let courseList = [];
            for (let item of this.selectedCourses) {
                if (!item.teacherCode) {
                    this.$message({
                        message: '请为选中的课时选择上课教师',
                        type: 'warning'
                    });
                    return;
                }
                let teacherName = this.getTeacherNameByCode(item.courseFolderCode, item.teacherCode);
                let folderCode = item.courseFolderCode;
                let folderName = dicHelper.getLabelByValue(this.$store.getters['course_folder'], folderCode);
                courseList.push({
                    CourseListId: item.studentCourseId,
                    StudentCode: item.studentCode,
                    TeacherCode: item.teacherCode,
                    TeacherName: teacherName,
                    Title: item.courseSubject,
                    CourseFolderCode: folderCode,
                    CourseFolderName: folderName
                });
            }

            let _this = this;
            axios({
                type: 'put',
                path: '/api/course/putsigninbatch',
                data: courseList,
                fn: function (result) {
                    if (result === 1200) {
                        _this.$message({
                            message: '批量签到成功！',
                            type: 'success'
                        });
                        _this.getAttendanceList();
                        // courseList.forEach(item => {
                        //     _this.removeTableRow(item.CourseListId);
                        // });
                        // _this.$store.commit('REDUCE_TO_RECORD', courseList.length);
                    }
                }
            });
        },
        // 请假
        qingJiaCourse(studentCourseId) {
            var _this = this;
            this.$confirm('是否确定该学员已请假?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'put',
                    path: '/api/coursearrange/putqingjiasingle',
                    data: {
                        StudentCourseId: studentCourseId
                    },
                    fn: function (result) {
                        if (result === 1200) {
                            _this.$message({
                                message: '请假成功！',
                                type: 'success'
                            });
                            _this.removeTableRow(studentCourseId);
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        // 请假或签到后 删除当前行 
        removeTableRow(courseId) {
            let index = -1;
            for (let i = 0; i < this.attendanceList.length; i++) {
                if (this.attendanceList[i].studentCourseId == courseId) {
                    index = i;
                    break;
                }
            }

            this.attendanceList.splice(index, 1);
            this.getRowSpanInfo();
        },

        handleSelectionChange(allItems) {
            this.selectedCourses = allItems;
        },

        beforeUpload(file) {
            this.uploadPanel.params.uid = file.uid;
        },
        uploadSuccess(response, file, fileList) {
            if (response != -1 && response != -2) {
                // -1 文件存储错误； -2 数据库插入错误                
                this.uploadPanel.fileCount = fileList.length;
                this.uploadPanel.fileUIds = [];
                for (let f of fileList) {
                    this.uploadPanel.fileUIds.push(f.uid);
                }
            }
        },

        handleImgPreview(file) {
            //alert('点击文件列表中已上传的文件时的钩子');
        },

        handleImgRemove(file, fileList) {
            this.uploadPanel.fileCount = fileList.length;
            this.uploadPanel.fileUIds = [];
            for (let f of fileList) {
                this.uploadPanel.fileUIds.push(f.uid);
            }

            var courseId = this.signInDialog.courseInfo.courseId;
            axios({
                type: 'delete',
                path: '/api/upload/deltempfile',
                data: {
                    courseId: courseId,
                    uid: file.uid
                },
                fn: function (result) {}
            });

        },

        // 合并行
        getRowSpanInfo() {
            this.dateRowSpanArray = [];
            this.timeRowSpanArray = [];
            let courseDate = '';
            let courseTime = '';
            let dateIndex = 0;
            let timeIndex = 0;
            this.attendanceList.forEach((item, index, array) => {
                this.dateRowSpanArray.push(1);
                this.timeRowSpanArray.push(1);
                if (index === 0) {
                    courseDate = item.courseDate;
                    courseTime = item.coursePeriod;
                } else {
                    if (item.courseDate === courseDate) {
                        this.dateRowSpanArray[dateIndex] += 1;
                        this.dateRowSpanArray[index] = 0;

                        if (item.coursePeriod === courseTime) {
                            this.timeRowSpanArray[timeIndex] += 1;
                            this.timeRowSpanArray[index] = 0;
                        } else {
                            courseTime = item.coursePeriod;
                            timeIndex = index;
                        }
                    } else {
                        courseDate = item.courseDate;
                        dateIndex = index;

                        courseTime = item.coursePeriod;
                        timeIndex = index;
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
            if (columnIndex === 0) {
                return {
                    rowspan: this.dateRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
            if (columnIndex === 1) {
                return {
                    rowspan: this.timeRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
        },

        getTeacherNameByCode(courseFolderCode, teacherCode) {
            let name = '';
            // for (let item of this.courseTeachers[courseFolderCode]) {
            //     if (item.teacherCode == teacherCode) {
            //         name = item.teacherName;
            //         break;
            //     }
            // }
            for(let key in this.courseTeachers){
                for (let item of this.courseTeachers[key]) {
                    if (item.teacherCode == teacherCode) {
                        name = item.teacherName;
                        break;
                    }
                }
            }
            return name;
        }
    }
}
</script>

<style lang="less" scoped>
.footer_container {
    height: 36px;
    line-height: 36px;
    margin-top: 10px;
    text-align: center;
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
