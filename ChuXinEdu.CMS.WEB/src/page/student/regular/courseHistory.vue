<template>
<div class="info_container">
    <div class="table_container">
        <el-table :data="courseList" :span-method="objectSpanMethod" v-loading="loading" size="mini" align="left" border :max-height="tableHeight">
            <el-table-column type="index" width="40"></el-table-column>
            <el-table-column prop="courseDate" label="上课日期" align='center' min-width="140">
                <template slot-scope='scope'>
                    {{ scope.row.courseDate + " " + scope.row.weekName }}
                </template>
            </el-table-column>
            <el-table-column prop="coursePeriod" label="上课时间段" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="100">
                <template slot-scope='scope'>
                    {{ scope.row.courseCategoryName + " / " + scope.row.courseFolderName }}
                </template>
            </el-table-column>
            <el-table-column prop="courseSubject" label="课程主题" align='center' min-width="160">
            </el-table-column>
            <el-table-column prop="teacherName" label="上课教师" align='center' min-width="85">
            </el-table-column>
            <el-table-column prop="courseType" label="课程标识" align='center' min-width="85">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="225">
                <template slot-scope='scope'>
                    <el-button type="warning" size="small" @click='showUploadDialog(scope.row)'>上传作品<i class="el-icon-upload el-icon--right"></i></el-button>
                    <el-button type="success" icon='edit' size="small" @click='viewArtwork(scope.row.studentCourseId)'>查看作品</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <div class="footer_container">
        <el-button type="primary" size="small" @click='supplementCourse()' :loading="downloadLoading"><i class="fa fa-book" aria-hidden="true"></i> 补录课程</el-button>
        <el-button type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
    </div>

    <el-dialog :title="supplementDialog.title" :visible.sync="supplementDialog.isShow" :width="supplementDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form :inline="true" size="mini" class="demo-form-inline">
                <el-form-item label="">
                    <el-select placeholder="请选择套餐" v-model="supplementDialog.curPackageId" @change="pacakgeChanged" size='mini' style="width:360px">
                        <el-option v-for="item in supplementDialog.packageList" :key="item.id" :label="item.packageName" :value="item.id">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="剩余课时数：">
                    {{ supplementDialog.selectedPackage.flexCourseCount || 0 }} 节
                </el-form-item>
            </el-form>
            <el-table :data="supplementDialog.newCourseList" size="mini" align="left" border stripe :max-height="supplementDialog.tableHeight">
                <el-table-column property="courseDate" label="上课日期" align='center' width="160">
                    <template slot-scope="scope">
                        <el-date-picker class="date-mini" v-model="scope.row.courseDate" @change="calculateWeek" type="date" size="mini" value-format="yyyy-MM-dd" placeholder="选择日期">
                        </el-date-picker>
                    </template>
                </el-table-column>
                <el-table-column prop="coursePeriod" label="时间段" align='center' min-width="130">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.coursePeriod" placeholder="选择时间段" size='mini'>
                            <el-option v-for="item in coursePeriodList[scope.row.courseWeekDay]" :key="item" :label="item" :value="item">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column prop="courseFolderCode" label="课程类别" align='center' min-width="130">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.courseFolderCode" placeholder="请选择" size='mini'>
                            <el-option v-for="item in $store.getters['course_folder_' + scope.row.courseCategoryCode]" :key="item.value" :label="item.label" :value="item.value">
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
                <el-button size="small" type="primary" @click="btnSubmitSupplement()">确 定</el-button>
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
                    <el-button size="small" type="primary" @click="btnSubmitUpload()">确 定</el-button>
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
} from '@/utils/index'

export default {
    name: 'student-course-history',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            studentName: '',
            courseList: [],
            dateRowSpanArray: [],
            loading: false,
            downloadLoading: false,
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
                selectedPackage: {

                },
                newCourseList: [],
                firstCourse: {}, //用于数据自动填充
                previousCourseDate:'',
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
    created() {
        this.studentName = this.$route.query.studentname;
        this.getHistoryCourseList();
    },
    methods: {
        getHistoryCourseList() {
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
                    });
                    this.courseList = result;
                    this.getRowSpanInfo();
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
                imgDesc: "",
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

        supplementCourse() {
            this.supplementDialog.curPackageId = '';
            this.supplementDialog.packageList = [];
            this.supplementDialog.newCourseList = [];
            this.supplementDialog.selectedPackage = {};

            if(Object.keys(this.courseTeachers).length == 0) {
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
                        if (this.coursePeriodList['day1'].length == 0) {
                            this.getPeriodList();
                        }
                    }
                }
            });
        },
        getPeriodList() {
            axios({
                type: 'get',
                path: '/api/coursearrange/getpriodlist/at-001',
                fn: (result) => {
                    result.forEach(item => {
                        this.coursePeriodList[item.courseWeekDay].push(item.coursePeriod);
                    });
                }
            });
        },
        pacakgeChanged(selectedValue) {
            for (let p of this.supplementDialog.packageList) {
                if (p.id == selectedValue) {
                    this.supplementDialog.selectedPackage = p;
                    break;
                }
            }

            this.supplementDialog.canCreate = true;
            // 初始化第一行
            this.supplementDialog.firstCourse = {
                index: new Date().getTime(),
                studentCoursePackageId: this.supplementDialog.selectedPackage.id,
                arrangeTemplateCode: 'at-001',
                classroom: 'room1',
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
                attendanceStatusCode:'01',
                attendanceStatusName:'上课销课'
            };
            this.supplementDialog.newCourseList = [this.supplementDialog.firstCourse];
        },
        createNewLine() {
            this.supplementDialog.newCourseList.push({
                index: new Date().getTime(),
                studentCoursePackageId: this.supplementDialog.selectedPackage.id,
                arrangeTemplateCode: 'at-001',
                classroom: 'room1',
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
                attendanceStatusCode:'01',
                attendanceStatusName:'上课销课'
            });
            if (this.supplementDialog.newCourseList.length >= this.supplementDialog.selectedPackage.flexCourseCount) {
                this.supplementDialog.canCreate = false;
            }
        },
        removeLine(index) {
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
            if(this.supplementDialog.newCourseList.length == 0) {
                return;
            }
            for(let item of this.supplementDialog.newCourseList) {
                if(!item.teacherCode || !item.courseDate || !item.courseWeekDay){
                    this.$message({
                        type: "warning",
                        message: "请为所有的课程选择上课日期、时间段、教师！"
                    });
                    return;
                }
                item.courseFolderName = dicHelper.getLabelByValue(this.$store.getters['course_folder_' + item.courseCategoryCode], item.courseFolderCode);
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
            for(let key in this.courseTeachers){
                for (let item of this.courseTeachers[key]) {
                    if (item.teacherCode == teacherCode) {
                        name = item.teacherName;
                        break;
                    }
                }
            }
            return name;
        },
        export2Excle() {
            if (this.courseList.length == 0) {
                this.$message({
                    message: '没有数据需要导出！',
                    type: 'success'
                });
                return;
            }
            var filename = this.courseList[0].studentName + "上课记录";
            this.downloadLoading = true
            import('@/vendor/Export2Excel').then(excel => {
                const tHeader = ['上课日期', '上课时间', '课程类别', '课程主题', '上课教师'];
                const filterVal = ['courseDate', 'coursePeriod', 'courseFolderName', 'courseSubject', 'teacherName']
                const data = this.formatJson(filterVal, this.courseList)
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
