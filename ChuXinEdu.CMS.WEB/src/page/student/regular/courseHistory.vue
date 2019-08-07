<template>
<div class="info_container">
    <div class="table_container">
        <el-table id="capture" ref="courseTable" :data="filteredCourseList" :span-method="objectSpanMethod" :row-class-name="packageColorFlag" v-loading="loading" size="mini" align="left" border :height="tableHeight">
            <el-table-column type="index" width="50" align='center'></el-table-column>
            <el-table-column prop="courseDate" label="上课日期" align='center' min-width="135">
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
            <el-table-column prop="courseSubject" label="课程主题" align='center' min-width="140">
            </el-table-column>
            <el-table-column prop="teacherName" label="上课教师" align='center' min-width="80">
            </el-table-column>
            <el-table-column prop="courseType" label="课程标识" align='center' min-width="70">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="160">
                <template slot-scope='scope'>
                    <el-button-group>
                        <el-button type="success" size="mini" icon='el-icon-upload' @click='showUploadDialog(scope.row)'></el-button>
                        <el-button type="primary" size="mini" icon='el-icon-picture' @click='viewArtwork(scope.row.studentCourseId)'></el-button>
                    </el-button-group>
                    <el-button v-noRepeatClick type="danger" size="mini" icon='el-icon-delete' @click='removeCourse(scope.row)'></el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <div class="footer_container">
        <el-radio-group v-model="curCourseCategory" size="small" type="success">
            <el-radio-button label="all">全部 <span style="font-weight:600">{{ this.badges.all }}</span>节</el-radio-button>
            <el-radio-button label="meishu">美术 <span style="font-weight:600">{{ this.badges.meishu }}</span>节</el-radio-button>
            <el-radio-button label="shufa">书法 <span style="font-weight:600">{{ this.badges.shufa }}</span>节</el-radio-button>
        </el-radio-group>
        <el-button v-noRepeatClick type="primary" size="small" @click='supplementCourse()'><i class="fa fa-book" aria-hidden="true"></i> 补录课程</el-button>
        <el-button v-noRepeatClick type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
        <el-button v-noRepeatClick type="primary" size="small" @click='captureCourse()' :loading="captureLoading"><i class="fa fa-camera" aria-hidden="true"></i> 导出长图</el-button>
        <el-select v-model="packageList1Value" @change="changePackage" size="small" clearable class="input-small" placeholder="请选择套餐">
            <el-option
                v-for="item in packageList1"
                :key="item.id"
                :label="item.packageName"
                :value="item.id">
            </el-option>
        </el-select>
    </div>

    <el-dialog :title="supplementDialog.title" :visible.sync="supplementDialog.isShow" :width="supplementDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form :inline="true" size="mini" class="demo-form-inline">
                <el-form-item label="">
                    <el-select placeholder="请选择套餐" v-model="supplementDialog.curPackageId" @change="packageChanged" size='mini' style="width:320px">
                        <el-option v-for="item in supplementDialog.packageList" :key="item.id" :label="item.packageName" :value="item.id">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="剩余课时数：">
                    {{ supplementDialog.selectedPackage.flexCourseCount || 0 }} 节
                </el-form-item>
            </el-form>
            <el-form :inline="true" size="mini" class="demo-form-inline">
                <el-form-item label="">
                    <el-select placeholder="请选择排课模板" v-model="supplementDialog.curATCode" @change="atChanged" size='mini' style="width:320px">
                        <el-option v-for="item in supplementDialog.atList" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="">
                    <el-select placeholder="请选择班级/机构" v-model="supplementDialog.curClassroom" size='mini' style="width:300px">
                        <el-option v-for="item in supplementDialog.classroomList" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
            </el-form>
            <el-table :data="supplementDialog.newCourseList" size="mini" align="left" border stripe :max-height="supplementDialog.tableHeight">
                <el-table-column property="courseDate" label="上课日期" align='center' width="160">
                    <template slot-scope="scope">
                        <el-date-picker class="date-mini" v-model="scope.row.courseDate" @change="calculateWeek" :editable="false" type="date" size="mini" value-format="yyyy-MM-dd" placeholder="选择日期">
                        </el-date-picker>
                    </template>
                </el-table-column>
                <el-table-column prop="coursePeriod" label="时间段" align='center' min-width="130">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.coursePeriod" placeholder="选择时间段" :clearable='true' size='mini'>
                            <el-option v-for="item in coursePeriodList[scope.row.courseWeekDay]" :key="item" :label="item" :value="item">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column prop="courseFolderCode" label="课程类别" align='center' min-width="130">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.courseFolderCode" placeholder="请选择" size='mini'>
                            <el-option v-for="item in $store.getters.getCourseColderByCate(scope.row.courseCategoryCode)" :key="item.value" :label="item.label" :value="item.value">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column prop="teacherCode" label="上课教师" align='center' min-width="115">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.teacherCode" placeholder="请选择" size='mini'>
                            <el-option v-for="item in courseTeachers[scope.row.courseFolderCode]" :key="item.teacherCode" :label="item.teacherName" :value="item.teacherCode">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column prop="courseSubject" label="课程主题" align='center' min-width="140">
                    <template slot-scope='scope'>
                        <el-input size="mini" v-model="scope.row.courseSubject" placeholder="请输入课程主题"></el-input>
                    </template>
                </el-table-column>
                <el-table-column prop="operation" align='center' label="操作" fixed="right" width="100">
                    <template slot-scope='scope'>
                        <el-button v-if="supplementDialog.canCreate" type="success" icon="el-icon-plus" size="mini" circle @click='createNewLine()'></el-button>
                        <el-button type="danger" icon="el-icon-minus" size="mini" circle @click='removeLine(scope.row.index)'></el-button>
                    </template>
                </el-table-column>
            </el-table>
            <div class="footer_container" style="text-align:center;margin-top:10px">
                <el-button size="small" @click="supplementDialog.isShow = false">取 消</el-button>
                <el-button v-noRepeatClick size="small" type="primary" @click="btnSubmitSupplement()">确 定</el-button>
            </div>
        </div>
    </el-dialog>

    <el-dialog :title="uploadDialog.title" :visible.sync="uploadDialog.isShow" :width="uploadDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="courseInfo" :model="uploadDialog.courseInfo" :rules="uploadDialog.studentCourseInfoRules" :label-width="uploadDialog.formLabelWidth" :label-position='uploadDialog.labelPosition' size="mini" style="margin:10px;width:auto;" label-suffix='：'>
                <el-form-item label="姓名">
                    {{ uploadDialog.courseInfo.studentName }} {{" [" + uploadDialog.courseInfo.courseDate + " " + uploadDialog.courseInfo.weekName + " " + uploadDialog.courseInfo.coursePeriod + "]"}}
                </el-form-item>
                <el-form-item label="作品描述">
                    <el-input v-model="uploadDialog.courseInfo.imgDesc"></el-input>
                </el-form-item>
                <el-form-item label="作品花费课时">
                    <el-input-number v-model="uploadDialog.courseInfo.imgCost" :min="1" size="mini"></el-input-number>
                </el-form-item>
                <el-form-item label="作品上传">
                    <el-upload class="upload-demo" :multiple="uploadDialog.multiple" :action="uploadDialog.actionUrl" :data="uploadDialog.params" :file-list="uploadDialog.thumbnailList" :on-remove="handleImgRemove" :before-upload="beforeUpload" :on-success="uploadSuccess" list-type="picture">
                        <el-button size="mini" type="primary">点击上传</el-button>
                    </el-upload>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button size="small" @click="btnCancelUpload()">取 消</el-button>
                    <el-button v-noRepeatClick size="small" type="primary" @click="btnSubmitUpload()">确 定</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>

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
    dicHelper,
    dateHelper,
    axios
} from '@/utils/index';

import html2canvas from 'html2canvas';

export default {
    name: 'student-course-history',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            studentName: '',
            courseList: [],
            filteredCourseList: [],
            packageList1: [],
            packageList1Value: '',
            badges: {
                all: 0,
                meishu: 0,
                shufa: 0
            },
            packages: [],
            curCourseCategory: 'all',
            dateRowSpanArray: [],
            loading: true,
            downloadLoading: false,
            captureLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 100,
            supplementDialog: {
                width: '850px',
                tableHeight: 450,
                isShow: false,
                title: '补录历史课程（没有完全排课的套餐）',
                labelPosition: 'right',
                formLabelWidth: '120px',
                packageList: [],
                curPackageId: '',
                atList: [],
                curATCode: '',
                classroomList: [],
                curClassroom: '',
                selectedPackage: {

                },
                newCourseList: [],
                firstCourse: {}, //用于数据自动填充
                previousCourseDate: '',
                canCreate: true,
            },
            uploadDialog: {
                width: '500px',
                isShow: false,
                title: '上传作品',
                labelPosition: 'right',
                formLabelWidth: '120px',
                multiple: true,
                actionUrl: '/api/upload/uploadartwork',
                fileCount: 0,
                fileUIds: [],
                thumbnailList: [],
                params: {
                    courseId: '',
                    studentCode: '',
                    studentName: '',
                    uid: ''
                },
                courseInfo: {
                    courseId: 0,
                    studentName: '',
                    courseDate: '',
                    weekName: '',
                    coursePeriod: '',
                    imgDesc: "",
                    imgCost: "",
                },
            },
            viewDialog: {
                width: '900px',
                isShow: false,
                title: '课程作品展示',
                artWorkList: []
            },
            courseTeachers: {},
            coursePeriodList: {
                "day1": [],
                "day2": [],
                "day3": [],
                "day4": [],
                "day5": [],
                "day6": [],
                "day7": []
            }
        }
    },
    watch: {
        'curCourseCategory'(cur) {
            this.filteredCourseList = this.filteredCourseList.filter(item => this.curCourseCategory == 'all' || item.courseCategoryCode == cur);
            this.getRowSpanInfo();
        }
    },
    created() {
        this.studentName = this.$route.query.studentname;
        this.getStudentPackageList();
        this.getHistoryCourseList();
    },
    updated() {
        this.$refs.courseTable.bodyWrapper.scrollTop =this.$refs.courseTable.bodyWrapper.scrollHeight;
    },
    methods: {
        getStudentPackageList() {
            axios({
                type: 'get',
                path: '/api/student/getpackages',
                data: {
                    studentCode: this.studentCode
                },
                fn: result => {
                    this.packageList1 = result;
                }
            });
        },
        getHistoryCourseList() {
            this.loading = true;
            axios({
                type: 'get',
                path: '/api/student/getcourselist',
                data: {
                    studentCode: this.studentCode
                },
                fn: result => {
                    result.forEach(item => {
                        item.courseDate = item.courseDate.split('T')[0];
                        item.weekName = dateHelper.getWeekNameByCode(item.courseWeekDay);
                        if (this.packages.indexOf(item.studentCoursePackageId) == -1) {
                            this.packages.push(item.studentCoursePackageId);
                        }
                    });
                    this.courseList = result;
                    this.filteredCourseList = result.filter(item => this.curCourseCategory == 'all' || item.courseCategoryCode == this.curCourseCategory);
                    this.getRowSpanInfo();
                    this.loading = false;

                    this.badges.all = this.courseList.length;
                    this.badges.shufa = this.courseList.filter(item => item.courseCategoryCode == 'shufa').length;
                    this.badges.meishu = this.badges.all - this.badges.shufa;
                }
            });
        },
        showUploadDialog(row) {
            this.uploadDialog.courseInfo = {
                courseId: row.studentCourseId,
                studentName: row.studentName,
                courseDate: row.courseDate,
                weekName: row.weekName,
                coursePeriod: row.coursePeriod,
                imgDesc: row.courseSubject,
                imgCost: null,
            }
            this.uploadDialog.params = {
                courseId: row.studentCourseId,
                studentCode: row.studentCode,
                studentName: row.studentName,
            }
            this.uploadDialog.fileCount = 0;
            this.uploadDialog.fileUIds = [];
            this.uploadDialog.thumbnailList = [];

            this.uploadDialog.isShow = true;

        },
        beforeUpload(file) {
            this.uploadDialog.params.uid = file.uid;
        },

        uploadSuccess(response, file, fileList) {
            if (response != -1 && response != -2) {
                // -1 文件存储错误； -2 数据库插入错误                
                this.uploadDialog.fileCount = fileList.length;
                this.uploadDialog.fileUIds = [];
                for (let f of fileList) {
                    this.uploadDialog.fileUIds.push(f.uid);
                }
            }
        },

        handleImgRemove(file, fileList) {
            this.uploadDialog.fileCount = fileList.length;
            this.uploadDialog.fileUIds = [];
            for (let f of fileList) {
                this.uploadDialog.fileUIds.push(f.uid);
            }

            var courseId = this.uploadDialog.courseInfo.courseId;
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
        btnSubmitUpload() {
            if (this.uploadDialog.fileCount == 0) {
                this.$message({
                    message: '请上传至少一张作品',
                    type: 'warning'
                });
                return;
            }
            if (this.uploadDialog.fileCount > 0) {
                if (!this.uploadDialog.courseInfo.imgDesc || !this.uploadDialog.courseInfo.imgCost) {
                    this.$message({
                        message: '请填写作品描述和花费课时数',
                        type: 'warning'
                    });
                    return;
                }
            }

            let courseId = this.uploadDialog.courseInfo.courseId;
            let courseDate = this.uploadDialog.courseInfo.courseDate;
            let fileUIds = this.uploadDialog.fileUIds;
            let imgCost = this.uploadDialog.courseInfo.imgCost;
            let title = this.uploadDialog.courseInfo.imgDesc;

            let course = {
                CourseListId: courseId,
                CourseDate: courseDate,
                StudentCode: '',
                TeacherCode: '',
                TeacherName: '',
                FileUIds: fileUIds,
                CostCount: imgCost,
                Title: title
            }

            let _this = this;
            axios({
                type: 'put',
                path: '/api/course/artworksupplement',
                data: course,
                fn: function (result) {
                    if (result == 1200) {
                        _this.$message({
                            message: '全部上传成功',
                            type: 'success'
                        });
                        _this.uploadDialog.thumbnailList = [];
                        _this.uploadDialog.isShow = false;
                    }
                }
            });
        },
        btnCancelUpload() {
            this.uploadDialog.isShow = false;
            let fileUIds = this.uploadDialog.fileUIds;
            var courseId = this.uploadDialog.courseInfo.courseId;

            if (fileUIds.length > 0) {
                let _this = this;
                axios({
                    type: 'delete',
                    path: '/api/upload/deltempfilebycourse/' + courseId,
                    data: fileUIds,
                    fn: function (result) {}
                });
            }

        },
        viewArtwork(courseId) {
            axios({
                type: 'get',
                path: '/api/course/getcourseartwork',
                data: {
                    courseId: courseId
                },
                fn: result => {
                    this.viewDialog.artWorkList = result;
                    if (result.length > 0) {
                        this.viewDialog.isShow = true;
                    } else {
                        this.$message({
                            message: '还没有上传作品',
                            type: 'warning'
                        });
                    }
                }
            });
        },

        removeCourse(row) {
            this.$confirm('是否确定删除该节课程[' + row.courseDate + ': ' + row.coursePeriod + ']？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/course/removecourse',
                    data: {
                        courseId: row.studentCourseId
                    },
                    fn: result => {
                        if (result == 1200) {
                            this.getHistoryCourseList();
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        getRowSpanInfo() {
            this.dateRowSpanArray = [];
            let cDate = '';
            let dateIndex = 0;
            this.filteredCourseList.forEach((item, index, array) => {
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
        packageColorFlag({
            row,
            rowIndex
        }) {
            let i = this.packages.indexOf(row.studentCoursePackageId);
            if (i > -1) {
                return 'row' + i;
            }
            return '';
        },
        getCourseTeacherList() {
            axios({
                type: 'get',
                path: '/api/teacher/getcourseteacherlist',
                fn: result => {
                    for (let t of result) {
                        if (this.courseTeachers.hasOwnProperty(t.role_code)) {
                            this.courseTeachers[t.role_code].push({
                                teacherCode: t.teacher_code,
                                teacherName: t.teacher_name
                            });
                        } else {
                            this.courseTeachers[t.role_code] = [{
                                teacherCode: t.teacher_code,
                                teacherName: t.teacher_name
                            }];
                        }
                    }
                }
            });
        },

        supplementCourse() {
            this.supplementDialog.curPackageId = '';
            this.supplementDialog.packageList = [];
            this.supplementDialog.newCourseList = [];
            this.supplementDialog.selectedPackage = {};

            if (Object.keys(this.courseTeachers).length == 0) {
                this.getCourseTeacherList();
            }
            // 获取未完成的套餐
            axios({
                type: 'get',
                path: '/api/student/getnofinishpackage/' + this.studentCode,
                fn: (result) => {
                    result.forEach(item => {
                        if (item.flexCourseCount > 0) {
                            this.supplementDialog.packageList.push(item);
                        }
                    })
                    if (this.supplementDialog.packageList.length == 0) {
                        this.$message({
                            type: "warning",
                            message: "没有需要补录的课程！"
                        });
                    } else {
                        this.supplementDialog.isShow = true;
                        this.getArrangeTemplateList();
                        this.getClassroom();
                    }
                }
            });
        },

        getArrangeTemplateList() {
            if (this.supplementDialog.atList.length == 0) {
                axios({
                    type: 'get',
                    path: '/api/arrangetemplate/getarrangetemplates',
                    fn: result => {
                        this.supplementDialog.atList = [];
                        result.forEach((item) => {
                            if (item.templateEnabled == 'Y') {
                                this.supplementDialog.atList.push({
                                    value: item.arrangeTemplateCode,
                                    label: item.arrangeTemplateName
                                });
                            }
                        });
                        if (this.supplementDialog.atList.length > 0) {
                            this.supplementDialog.curATCode = this.supplementDialog.atList[0].value;
                            if (this.coursePeriodList['day1'].length == 0) {
                                this.getPeriodList();
                            }
                        } else {
                            this.$message({
                                type: 'warning',
                                message: '没有可用的排课模板，请先在系统管理中添加排课模板！'
                            });
                        }
                    }
                })
            }

        },

        getClassroom() {
            if (this.supplementDialog.classroomList.length == 0) {
                axios({
                    type: 'get',
                    path: '/api/config/getdicbycode',
                    data: {
                        typeCode: 'classroom'
                    },
                    fn: result => {
                        this.supplementDialog.classroomList = result;
                        this.supplementDialog.curClassroom = result && result[0].value || '';
                    }
                })
            }
        },

        getPeriodList() {
            axios({
                type: 'get',
                path: '/api/coursearrange/getpriodlist/' + this.supplementDialog.curATCode,
                fn: (result) => {
                    for (let i = 1; i <= 7; i++) {
                        this.coursePeriodList['day' + i] = [];
                    }
                    result.forEach(item => {
                        this.coursePeriodList[item.courseWeekDay].push(item.coursePeriod);
                    });
                }
            });
        },

        atChanged() {
            this.getPeriodList();
        },

        packageChanged(selectedValue) {
            for (let p of this.supplementDialog.packageList) {
                if (p.id == selectedValue) {
                    this.supplementDialog.selectedPackage = p;
                    break;
                }
            }
            if (this.supplementDialog.selectedPackage.flexCourseCount == 1) {
                this.supplementDialog.canCreate = false;
            } else {
                this.supplementDialog.canCreate = true;
            }
            // 初始化第一行
            this.supplementDialog.firstCourse = {
                index: new Date().getTime(),
                studentCoursePackageId: this.supplementDialog.selectedPackage.id,
                arrangeTemplateCode: this.supplementDialog.curATCode,
                classroom: this.supplementDialog.curClassroom,
                courseWeekDay: '',
                courseDate: '',
                coursePeriod: '',
                studentCode: this.studentCode,
                studentName: this.studentName,
                teacherCode: '',
                teacherName: '',
                packageCode: this.supplementDialog.selectedPackage.packageCode,
                courseCategoryCode: this.supplementDialog.selectedPackage.courseCategoryCode,
                courseCategoryName: this.supplementDialog.selectedPackage.courseCategoryName,
                courseFolderCode: this.supplementDialog.selectedPackage.courseFolderCode,
                courseFolderName: '',
                courseSubject: '',
                courseType: '正式',
                attendanceStatusCode: '01',
                attendanceStatusName: '上课销课'
            };
            this.supplementDialog.newCourseList = [this.supplementDialog.firstCourse];
        },
        createNewLine() {
            this.supplementDialog.newCourseList.push({
                index: new Date().getTime(),
                studentCoursePackageId: this.supplementDialog.selectedPackage.id,
                arrangeTemplateCode: this.supplementDialog.curATCode,
                classroom: this.supplementDialog.curClassroom,
                courseWeekDay: '',
                courseDate: this.previousCourseDate,
                coursePeriod: this.supplementDialog.firstCourse.coursePeriod,
                studentCode: this.studentCode,
                studentName: this.studentName,
                teacherCode: this.supplementDialog.firstCourse.teacherCode,
                teacherName: '',
                packageCode: this.supplementDialog.selectedPackage.packageCode,
                courseCategoryCode: this.supplementDialog.selectedPackage.courseCategoryCode,
                courseCategoryName: this.supplementDialog.selectedPackage.courseCategoryName,
                courseFolderCode: this.supplementDialog.selectedPackage.courseFolderCode,
                courseFolderName: '',
                courseSubject: '',
                courseType: '正式',
                attendanceStatusCode: '01',
                attendanceStatusName: '上课销课'
            });
            if (this.supplementDialog.newCourseList.length >= this.supplementDialog.selectedPackage.flexCourseCount) {
                this.supplementDialog.canCreate = false;
            }
        },
        removeLine(index) {
            if (this.supplementDialog.newCourseList.length == 1) {
                this.$message({
                    type: "warning",
                    message: "请至少保留一条记录！"
                })
                return;
            }
            let removedIndex = -1;
            for (let i = 0; i < this.supplementDialog.newCourseList.length; i++) {
                if (this.supplementDialog.newCourseList[i].index == index) {
                    removedIndex = i;
                    break;
                }
            }
            this.supplementDialog.newCourseList.splice(removedIndex, 1);
            this.supplementDialog.canCreate = true;
        },
        btnSubmitSupplement() {
            if (this.supplementDialog.newCourseList.length == 0) {
                return;
            }
            for (let item of this.supplementDialog.newCourseList) {
                if (!item.teacherCode || !item.courseDate || !item.courseWeekDay) {
                    this.$message({
                        type: "warning",
                        message: "请为所有的课程选择上课日期、时间段、教师！"
                    });
                    return;
                }
                item.courseFolderName = dicHelper.getLabelByValue(this.$store.getters.getCourseColderByCate(item.courseCategoryCode), item.courseFolderCode);
                item.teacherName = this.getTeacherNameByCode(item.courseFolderCode, item.teacherCode);
            }

            axios({
                type: 'post',
                path: '/api/course/coursesupplement',
                data: this.supplementDialog.newCourseList,
                fn: result => {
                    if (result === 1200) {
                        this.$message({
                            message: '补录课程成功！',
                            type: 'success'
                        });
                        this.getHistoryCourseList();
                        this.supplementDialog.isShow = false;
                    } else if (result == 1409) {
                        this.$message({
                            message: '课程套餐数据异常！！请检查数据！',
                            type: 'error'
                        });
                    } else {
                        this.$message({
                            message: '补录出错，请查看相关日志！',
                            type: 'error'
                        });
                    }
                }
            })
        },
        calculateWeek(curDate) {
            let weekCode = dateHelper.getWeekCodeByDate(curDate);
            this.supplementDialog.newCourseList.forEach(item => {
                if (item.courseDate == curDate) {
                    item.courseWeekDay = weekCode;
                }
            });
            this.previousCourseDate = curDate;
        },
        getTeacherNameByCode(courseFolderCode, teacherCode) {
            let name = '';
            // for (let item of this.courseTeachers[courseFolderCode]) {
            //     if (item.teacherCode == teacherCode) {
            //         name = item.teacherName;
            //         break;
            //     }
            // }
            for (let key in this.courseTeachers) {
                for (let item of this.courseTeachers[key]) {
                    if (item.teacherCode == teacherCode) {
                        name = item.teacherName;
                        break;
                    }
                }
            }
            return name;
        },

        changePackage(){
            this.filteredCourseList = this.courseList.filter(item => this.packageList1Value == null || item.studentCoursePackageId == this.packageList1Value);
            this.getRowSpanInfo();

            this.badges.all = this.filteredCourseList.length;
            this.badges.shufa = this.filteredCourseList.filter(item => item.courseCategoryCode == 'shufa').length;
            this.badges.meishu = this.badges.all - this.badges.shufa;
        },

        captureCourse() {
            this.captureLoading = true;
            let dom = document.querySelector("#capture");
            let width = dom.offsetWidth;

            // clone dom
            let cloneDom = dom.cloneNode(true);
            cloneDom.style.height = 'auto';
            cloneDom.childNodes[2].style.height = 'auto';
            cloneDom.style.zIndex = "-1";
            document.body.appendChild(cloneDom);
            let height = cloneDom.offsetHeight;

            // create canvas
            let canvas = document.createElement("canvas");
            let scale = 1.8; //定义比例
            canvas.width = width * scale; //定义canvas 宽度 * 缩放
            canvas.height = height * scale; //定义canvas高度 * 缩放
            canvas.getContext("2d").scale(scale, scale); //获取context,设置比例

            let h2cOpts = {
                scale: scale, // 比例
                canvas: canvas, //自定义 canvas
                width: width - 165, // 原始宽度 - 按钮区域宽度， HTML2canvas后的canvas宽度，会 * scale
                height: height, // 原始高度
                useCORS: true, // 开启跨域配置
                dpi: window.devicePixelRatio * 2
            };

            let waterMarkOpts = {
                fontStyle: "76px 幼圆", //水印字体设置
                rotateAngle: -20 * Math.PI / 180, //水印字体倾斜角度设置
                fontColor: "#7eb00a", //水印字体颜色设置
                linePositionX: (width - 165) * scale / 2, //canvas第一行文字起始X坐标
                firstLinePositionY: height * scale - 220,
                secondLinePositionY: height * scale - 130,
                thirdLinePositionY: height * scale - 40,
            };

            html2canvas(cloneDom, h2cOpts).then(canvas => {
                var img = new Image();
                var ctx = canvas.getContext('2d');
                if (img.complete) {
                    img.src = canvas.toDataURL("image/png", 1.0);
                    img.onload = () => {
                        ctx.drawImage(img, 0, 0);
                        ctx.font = waterMarkOpts.fontStyle;
                        //文字倾斜角度
                        //ctx.rotate(waterMarkOpts.rotateAngle);
                        ctx.fillStyle = waterMarkOpts.fontColor;

                        ctx.fillText(this.studentName, waterMarkOpts.linePositionX, waterMarkOpts.firstLinePositionY);
                        ctx.fillText("初心工作室", waterMarkOpts.linePositionX, waterMarkOpts.secondLinePositionY);
                        ctx.fillText(dateHelper.getDate(), waterMarkOpts.linePositionX, waterMarkOpts.thirdLinePositionY);
                        //坐标系还原
                        //ctx.rotate(-waterMarkOpts.rotateAngle);

                        // 关闭抗锯齿
                        ctx.mozImageSmoothingEnabled = false;
                        ctx.webkitImageSmoothingEnabled = false;
                        ctx.msImageSmoothingEnabled = false;
                        ctx.imageSmoothingEnabled = false;

                        this.downloadScreenShot(canvas);
                        document.body.removeChild(cloneDom);
                        this.captureLoading = false;
                    }
                }
            });
        },

        downloadScreenShot(canvas) {
            let url = canvas.toDataURL("image/png", 1.0)
            let link = document.createElement('a')
            link.style.display = 'none'
            link.href = url
            link.setAttribute('download', this.studentName + '的上课记录.png')

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },

        export2Excle() {
            axios({
                type: 'get',
                path: '/api/download/studentcourse',
                data: {
                    studentCode: this.studentCode
                },
                responseType: 'blob',
                fn: result => {
                    this.download(result);
                    // var fileDownload = require('js-file-download');
                    // fileDownload(result, '上课记录.xlsx');                    
                }
            })
        },

        download(data) {
            if (!data) {
                return
            }
            let url = window.URL.createObjectURL(new Blob([data]))
            let link = document.createElement('a')
            link.style.display = 'none'
            link.href = url
            link.setAttribute('download', this.studentName + '的上课记录.xlsx')

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },

        // 前端导出 暂时不使用
        export2Excle1() {
            if (this.courseList.length == 0) {
                this.$message({
                    message: '没有数据（' + this.curCourseCategory + '）需要导出！',
                    type: 'success'
                });
                return;
            }
            var filename = this.courseList[0].studentName + "上课记录_" + this.curCourseCategory;
            this.downloadLoading = true;
            import('@/vendor/Export2Excel').then(excel => {
                const tHeader = ['上课日期', '上课时间', '课程类别', '课程主题', '上课教师'];
                const filterVal = ['courseDate', 'coursePeriod', 'courseFolderName', 'courseSubject', 'teacherName']
                const data = this.formatJson(filterVal, this.courseList.filter(item => this.curCourseCategory == 'all' || item.courseCategoryCode == this.curCourseCategory))
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
