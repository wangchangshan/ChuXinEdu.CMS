<template>
<div class="info_container">
    <div class="table_container">
        <el-table :data="CourseList" 
                  :span-method="objectSpanMethod" 
                  v-loading="loading" 
                  size="mini"  
                  align="left"
                  border 
                  stripe 
                  :max-height="tableHeight">
            <el-table-column prop="courseDate" label="上课日期" align='center' min-width="130">
            </el-table-column>
            <el-table-column prop="coursePeriod" label="上课时间段" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="courseFolderName" label="课程类别" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="teacherName" label="上课教师" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="courseType" label="课程标识" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="225">
                <template slot-scope='scope'>
                    <el-button type="warning" size="small" @click='uploadAchievement(scope.row)'>上传作品<i class="el-icon-upload el-icon--right"></i></el-button>
                    <el-button type="success" icon='edit' size="small" @click='viewAchievement(scope.row)'>查看作品</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <!-- <el-dialog :title="uploadDialog.title" :visible.sync="uploadDialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="studentCourseInfo" :model="uploadDialog.studentCourseInfo" :rules="uploadDialog.studentCourseInfoRules" :label-width="uploadDialog.formLabelWidth" :label-position='uploadDialog.labelPosition' size="mini" style="margin:10px;width:auto;" label-suffix='：'>
                <el-form-item label="姓名">
                    {{ "test" }} {{" [" + uploadDialog.studentCourseInfo.student_course_date + " " + uploadDialog.studentCourseInfo.student_course_time + "]"}}
                </el-form-item>
                <el-form-item label="作品描述">
                    <el-input v-model="uploadDialog.studentCourseInfo.img_desc"></el-input>
                </el-form-item>
                <el-form-item label="作品花费课时">
                    <el-input v-model="uploadDialog.studentCourseInfo.img_cost"></el-input>
                </el-form-item>
                <el-form-item label="作品上传">
                    <el-upload class="upload-demo" action="https://jsonplaceholder.typicode.com/posts/" :on-preview="handleImgPreview" :on-remove="handleImgRemove" list-type="picture">
                        <el-button size="mini" type="primary">点击上传</el-button>
                    </el-upload>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="uploadDialog.show = false">取 消</el-button>
                    <el-button type="primary">确 定</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>

    <el-dialog :title="viewDialog.title" :visible.sync="viewDialog.show" :close-on-press-escape='false' :modal-append-to-body="false">
        <el-carousel indicator-position="outside">
            <el-carousel-item v-for="item in 4" :key="item">
                <h3>{{ item }}</h3>
            </el-carousel-item>
        </el-carousel>
    </el-dialog> -->
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
            CourseList:[],
            dateRowSpanArray: [],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 128,

            uploadDialog: {
                width: '500px',
                show: false,
                title: '上传作品',
                labelPosition: 'right',
                formLabelWidth: '120px',
                studentCourseInfo: {
                    student_course_date: '2018-09-09',
                    student_course_time: '17:30-19:00',
                    img_desc: "",
                    img_cost: "",
                },
                studentCourseInfoRules: {}
            },
            viewDialog: {
                width: '600px',
                show: false,
                title: '课程作品展示',
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
                });
                _this.CourseList = result;
                _this.getRowSpanInfo();
            }
        });
        console.log("table height: " + _this.tableHeight);
    },
    methods: {
        courseTag(course) {
            let basic = '';
            switch (course) {
                case '国画':
                    basic = 'success'
                    break;
                case '西画':
                    basic = ''
                    break;
                case '书法':
                    basic = 'info'
                    break;
                default:
                    basic = 'danger'
            }
            return basic;
        },
        handleSizeChange(val) {
            console.log(`每页 ${val} 条`);
        },
        handleCurrentChange(val) {
            console.log(`当前页: ${val}`);
        },
        getRowSpanInfo() {
            this.dateRowSpanArray = [];
            let cDate = '';
            let dateIndex = 0;
            this.CourseList.forEach((item, index, array) => {
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
            if (columnIndex === 0) {
                return {
                    rowspan: this.dateRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
        },
        uploadAchievement() {
            this.uploadDialog.show = true;
        },
        viewAchievement() {
            this.viewDialog.show = true;
        },

        handleImgPreview(file) {

        },
        handleImgRemove(file, fileList) {
            //console.log(file, fileList);
        },
    }
}
</script>

<style lang="less" scoped>
</style>
