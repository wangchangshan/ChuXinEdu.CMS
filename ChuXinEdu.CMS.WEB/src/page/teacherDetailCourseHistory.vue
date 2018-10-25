<template>
<div class="info_container">
    <div class="table_container">
        <el-table :data="courseList" :span-method="objectSpanMethod" v-loading="loading" size="mini" align="left" border stripe :max-height="tableHeight">
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
        <el-button type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
    </div>

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
                    <el-upload class="upload-demo" :multiple="uploadDialog.multiple" :action="uploadDialog.actionUrl" :data="uploadDialog.params" :file-list="uploadDialog.thumbnailList" :on-preview="handleImgPreview" :on-remove="handleImgRemove" :before-upload="beforeUpload" :on-success="uploadSuccess" list-type="picture">
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
    axios
} from '@/utils/index'

export default {
    name: 'student-course-history',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            courseList: [],
            dateRowSpanArray: [],
            loading: false,
            downloadLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 100,

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
            }
        }
    },
    created() {
        var _this = this;
        axios({
            type: 'get',
            path: '/api/student/getcourselist',
            data: {
                studentCode: _this.studentCode
            },
            fn: function (result) {
                result.forEach(item => {
                    item.courseDate = item.courseDate.split('T')[0];
                    item.weekName = _this.getWeekNameByCode(item.courseWeekDay);
                });
                _this.courseList = result;
                _this.getRowSpanInfo();
            }
        });
    },
    methods: {
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
                    if (result == 200) {
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

        handleImgPreview(file) {

        },
        getWeekNameByCode(code) {
            let week = '';
            switch (code) {
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
