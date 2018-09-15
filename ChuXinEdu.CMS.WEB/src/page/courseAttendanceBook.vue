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
            <el-table-column prop="courseFolderName" label="课程内容" align='center' width="100">
            </el-table-column>
            <el-table-column prop="" label="上课教师" align='center' min-width="140">
                <template slot-scope='scope'>
                    <el-select v-model="scope.row.teacherCode" placeholder="选择上课教师" size='mini'>
                        <el-option v-for="item in teacherList[scope.row.courseFolderCode]" :key="item.teacherCode" :label="item.teacherName" :value="item.teacherCode">
                        </el-option>
                    </el-select>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
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
                <el-form-item label="上课教师">
                    <el-select v-model="signInDialog.courseInfo.teacherCode" placeholder="选择上课教师" size='mini'>
                        <el-option v-for="item in teacherList[signInDialog.courseInfo.courseFolderCode]" :key="item.teacherCode" :label="item.teacherName" :value="item.teacherCode">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="作品描述">
                    <el-input v-model="signInDialog.courseInfo.imgDesc"></el-input>
                </el-form-item>
                <el-form-item label="作品花费课时">
                    <el-input-number v-model="signInDialog.courseInfo.imgCost" :min="1" size="mini"></el-input-number>
                </el-form-item>
                <el-form-item label="作品上传">
                    <el-upload class="upload-demo" :multiple="uploadPanel.multiple" :action="uploadPanel.actionUrl" :data="uploadPanel.courseData" :on-preview="handleImgPreview" :on-remove="handleImgRemove" :before-upload="beforeUpload" :on-success="uploadSuccess" :file-list="uploadPanel.thumbnailList" list-type="picture">
                        <el-button size="mini" type="primary">点击上传</el-button>
                    </el-upload>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="cancelSignIn()" size="small">取 消</el-button>
                    <el-button type="primary" @click="submitSignIn()" size="small">签 到</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    data() {
        return {
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 128,

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
                    teacherCode: '',
                    imgDesc: '',
                    imgCost: ''
                },
                studentCourseInfoRules: {}
            },
            uploadPanel: {
                multiple: true,
                actionUrl: '/api/course/uploadartwork',
                courseData: {
                    courseId: '',
                    studentCode: '',
                    studentName: '',
                    uid: ''
                },
                fileCount: 0,
                thumbnailList: []
            },
            teacherList: {
                "meishu_00": [{
                    teacherCode: '001',
                    teacherName: '唐得红',
                }],
                "meishu_01": [{
                    teacherCode: '002',
                    teacherName: '马朝',
                }]
            },
            dateRowSpanArray: [],
            timeRowSpanArray: []
        }
    },
    created() {
        this.getAttendanceList();
    },
    methods: {
        getAttendanceList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/course/getattendancelist',
                fn: function (result) {
                    for (let item of result) {
                        item.courseDate = item.courseDate.split('T')[0];
                        item.weekName = _this.getWeekNameByCode(item.courseWeekDay);
                    }
                    _this.attendanceList = result;
                    _this.getRowSpanInfo();
                }
            });
        },

        cancelSignIn() {
            this.uploadPanel.thumbnailList = [];
            this.signInDialog.isShow = false;
        },

        submitSignIn() {
            if (!this.signInDialog.courseInfo.teacherCode) {
                this.$message({
                    message: '请选择上课教师',
                    type: 'warning'
                });
                return;
            }
            if (this.uploadPanel.fileCount > 0) {
                if (!this.signInDialog.courseInfo.imgDesc || !this.signInDialog.courseInfo.imgCost) {
                    this.$message({
                        message: '请填写作品描述和花费课时数',
                        type: 'warning'
                    });
                    return;
                }
            }

            let courseId = this.signInDialog.courseInfo.courseId;
            let studentCode = this.signInDialog.courseInfo.studentCode;
            let teacherCode = this.signInDialog.courseInfo.teacherCode;
            let courseFolderCode = this.signInDialog.courseInfo.courseFolderCode;
            let teacherName = this.getTeacherNameByCode(courseFolderCode, teacherCode);
            let fileUIds = this.uploadPanel.fileUIds;
            let imgCost = this.signInDialog.courseInfo.imgCost;
            let title = this.signInDialog.courseInfo.imgDesc;

            let course = {
                CourseListId: courseId,
                StudentCode: studentCode,
                TeacherCode: teacherCode,
                TeacherName: teacherName,
                FileUIds: fileUIds,
                CostCount: imgCost,
                Title: title
            }

            let _this = this;
            axios({
                type: 'put',
                path: '/api/course/putsignin',
                data: course,
                fn: function (result) {
                    if (result == 200) {
                        _this.$message({
                            message: '签到成功',
                            type: 'success'
                        });
                        _this.uploadPanel.thumbnailList = [];
                        _this.signInDialog.isShow = false;
                        _this.removeTableRow(courseId);
                    }
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
                courseList.push({
                    CourseListId: item.studentCourseId,
                    StudentCode: item.studentCode,
                    TeacherCode: item.teacherCode,
                    TeacherName: teacherName
                });
            }

            let _this = this;
            axios({
                type: 'put',
                path: '/api/course/putsigninbatch',
                data: courseList,
                fn: function (result) {
                    if (result === 200) {
                        _this.$message({
                            message: '批量签到成功！',
                            type: 'success'
                        });
                        courseList.forEach(item => {
                            _this.removeTableRow(item.CourseListId);
                        });
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
                        if (result === 200) {
                            _this.$message({
                                message: '请假成功！',
                                type: 'success'
                            });
                            _this.removeTableRow(studentCourseId)
                        }
                    }
                });
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

        showSingleSignInDialog(row) {
            this.signInDialog.isShow = true;
            this.signInDialog.courseInfo = {
                courseId: row.studentCourseId,
                studentCode: row.studentCode,
                studentName: row.studentName,
                courseDate: row.courseDate,
                weekName: row.weekName,
                courseFolderCode: row.courseFolderCode,
                coursePeriod: row.coursePeriod,
                teacherCode: row.teacherCode,
            };
            this.uploadPanel.courseData = {
                courseId: row.studentCourseId,
                studentCode: row.studentCode,
                studentName: row.studentName,
            }
        },

        handleSelectionChange(allItems) {
            this.selectedCourses = allItems;
        },

        beforeUpload(file) {
            this.uploadPanel.courseData.uid = file.uid;
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
                path: '/api/course/deltempfile',
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
            for (let item of this.teacherList[courseFolderCode]) {
                if (item.teacherCode == teacherCode) {
                    name = item.teacherName;
                    break;
                }
            }
            return name;
        },

        getWeekNameByCode(theDay) {
            let week = '';
            switch (theDay) {
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
