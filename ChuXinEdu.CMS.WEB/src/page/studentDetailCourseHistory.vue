<template>
<div class="fillcontain">
    {{'学号：'+ student_code + '; 姓名：'+ student_name}}
    <div class="table_container">
        <el-table :data="courseHistoryList" :span-method="objectSpanMethod" stripe v-loading="loading" style="width: 100%" align="center" border :max-height="tableHeight" @selection-change="handleSelectionChange">
            <el-table-column prop="student_course_date" label="上课日期" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="student_course_time" label="上课时间段" align='center' min-width="140">
            </el-table-column>
            <el-table-column prop="student_course_content" label="课程内容" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="course_teacher_code" label="上课教师" align='center' min-width="140">
            </el-table-column>
            <!-- <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                <template slot-scope='scope'>
                    <el-button type="warning" icon='edit' size="small" @click='singleLeave(scope.row.student_code)'>请假</el-button>
                    <el-button type="success" icon='edit' size="small" @click='singleSignIn(scope.row)'>签到</el-button>
                </template>
            </el-table-column> -->
        </el-table>
    </div>
    <!-- <div class="footer_container">
        <el-button size="small" type="warning" icon="" @click='batchLeave()'>批量请假</el-button>
        <el-button size="small" type="success" icon="" @click='batchSignIn()'>批量签到</el-button>
    </div> -->
</div>
</template>

<script>
export default {
    name: 'student-course-history',
    // props:['student_code', 'student_name'],
    props:{
        'student_code' : String,
        'student_name' : String
    },
    data() {
        return {
            courseHistoryList: [{
                    student_course_date: '2018-08-09',
                    student_course_time: '16:00-17:30',
                    student_course_content: '国画',
                    course_teacher_code: '唐得红',
                },
                {
                    student_course_date: '2018-08-09',
                    student_course_time: '17:30-19:00',
                    student_course_content: '国画',
                    course_teacher_code: '唐得红',
                },
                {
                    student_course_date: '2018-08-10',
                    student_course_time: '16:00-17:30',
                    student_course_content: '西画',
                    course_teacher_code: '马朝',
                },
                {
                    student_course_date: '2018-08-11',
                    student_course_time: '16:00-17:30',
                    student_course_content: '西画',
                    course_teacher_code: '马朝',
                },
            ],
            dateRowSpanArray: [],
            // 以后可以缓存
            teacherList: [{
                teacher_code: '001',
                teacher_name: '唐得红',
            }, {
                teacher_code: '002',
                teacher_name: '马朝',
            }],
            selectedStudents: [],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 128,
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
        addStudent() {
            this.dialog.title = '新增学生';
            this.dialog.show = true;
        },
        batchLeave() {
            alert('批量请假')
            if (this.selectedStudents.length === 0) {
                this.$message({
                    message: '请选择至少一条记录',
                    type: 'warning'
                });
                return false;
            }
        },
        batchSignIn() {
            alert('批量签到')
            if (this.selectedStudents.length === 0) {
                this.$message({
                    message: '请选择至少一条记录',
                    type: 'warning'
                });
                return false;
            }
        },
        singleLeave() {
            alert("只需要修改状态为请教，未排课时数+1");
        },
        handleSelectionChange(allItems) {
            this.selectedStudents = allItems;
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
        handleImgPreview(file) {

        },
        handleImgRemove(file, fileList) {
            //console.log(file, fileList);
        },
    }
}
</script>
