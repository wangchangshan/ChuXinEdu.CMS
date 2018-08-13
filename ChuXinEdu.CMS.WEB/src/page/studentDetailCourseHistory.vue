<template>
<div class="fillcontain">
    {{'学号：'+ student_code + '; 姓名：'+ student_name}}
    <div class="table_container">
        <el-table :data="courseHistoryList" :span-method="objectSpanMethod" stripe v-loading="loading" style="width: 100%" align="center" border :max-height="tableHeight">
            <el-table-column prop="student_course_date" label="上课日期" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="student_course_time" label="上课时间段" align='center' min-width="140">
            </el-table-column>
            <el-table-column prop="student_course_content" label="课程类别" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="course_teacher_code" label="上课教师" align='center' min-width="140">
            </el-table-column>
            <el-table-column prop="course_desc" label="内容简介" align='center' min-width="140">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" min-width="215">
                <template slot-scope='scope'>
                    <el-button type="warning" size="small" @click='uploadAchievement(scope.row)'>上传作品<i class="el-icon-upload el-icon--right"></i></el-button>
                    <el-button type="success" icon='edit' size="small" @click='viewAchievement(scope.row)'>查看作品</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <el-dialog :title="uploadDialog.title" :visible.sync="uploadDialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="studentCourseInfo" :model="uploadDialog.studentCourseInfo" :rules="uploadDialog.studentCourseInfoRules" :label-width="uploadDialog.formLabelWidth" :label-position='uploadDialog.labelPosition' size="mini" style="margin:10px;width:auto;" label-suffix='：'>
                <el-form-item label="姓名">
                    {{student_name}} {{" [" + uploadDialog.studentCourseInfo.student_course_date + " " + uploadDialog.studentCourseInfo.student_course_time + "]"}}
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
    </el-dialog>
</div>
</template>

<script>
export default {
    name: 'student-course-history',
    // props:['student_code', 'student_name'],
    props: {
        'student_code': String,
        'student_name': String
    },
    data() {
        return {
            courseHistoryList: [{
                    student_course_date: '2018-08-09',
                    student_course_time: '16:00-17:30',
                    student_course_content: '国画',
                    course_teacher_code: '唐得红',
                    course_desc: '山水画'
                },
                {
                    student_course_date: '2018-08-09',
                    student_course_time: '17:30-19:00',
                    student_course_content: '国画',
                    course_teacher_code: '唐得红',
                    course_desc: '山水画'
                },
                {
                    student_course_date: '2018-08-10',
                    student_course_time: '16:00-17:30',
                    student_course_content: '西画',
                    course_teacher_code: '马朝',
                    course_desc: '山水画'
                },
                {
                    student_course_date: '2018-08-11',
                    student_course_time: '16:00-17:30',
                    student_course_content: '西画',
                    course_teacher_code: '马朝',
                    course_desc: '山水画'
                },
            ],
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
        this.getRowSpanInfo();
        console.log('秋天的酒')
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
            let courseDate = '';
            let dateIndex = 0;
            this.courseHistoryList.forEach((item, index, array) => {
                this.dateRowSpanArray.push(1);
                if (index === 0) {
                    courseDate = item.student_course_date;
                } else {
                    if (item.student_course_date === courseDate) {
                        this.dateRowSpanArray[dateIndex] += 1;
                        this.dateRowSpanArray[index] = 0;
                    } else {
                        courseDate = item.student_course_date;
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
.el-carousel__item h3 {
    color: #475669;
    font-size: 18px;
    opacity: 0.75;
    line-height: 300px;
    margin: 0;
}

.el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
}

.el-carousel__item:nth-child(2n+1) {
    background-color: #d3dce6;
}
</style>
