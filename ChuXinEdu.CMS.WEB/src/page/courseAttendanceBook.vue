<template>
    <!-- <div>展示今日以及过往没有签到的学生上课列表</div> -->
    <div class="fillcontain">
        <div class="table_container">
            <el-table :data="AttendanceList" :span-method="objectSpanMethod" v-loading="loading" style="width: 100%" align="center" border :max-height="tableHeight" @selection-change="handleSelectionChange">
                
                <el-table-column prop="student_course_date" label="上课日期" align='center' min-width="120">
                </el-table-column>
                <el-table-column prop="student_course_time" label="上课时间段" align='center' min-width="140" >
                </el-table-column>
                <el-table-column type="selection" label="选择" align='center' width="60"></el-table-column>   
                              
                <el-table-column prop="student_name" label="姓名" align='center' min-width="120">
                </el-table-column>
                <el-table-column prop="student_course_content" label="课程内容" align='center' width="110">
                </el-table-column>   
                <el-table-column prop="course_teacher_code" label="上课教师" align='center' min-width="140">
                    <template slot-scope='scope'>
                        <el-select v-model="scope.row.course_teacher_code" placeholder="选择上课教师" size='small'>
                            <el-option
                                v-for="item in scope.row.teachers"
                                :key="item.teacher_code"
                                :label="item.teacher_name"
                                :value="item.teacher_code">
                            </el-option>
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                    <template slot-scope='scope'>
                        <el-button type="warning" icon='edit' size="small" @click='singleLeave(scope.row.student_code)'>请假</el-button>
                        <el-button type="success" icon='edit' size="small" @click='singleSignIn(scope.row)'>签到</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>        
        <div class="footer_container">
            <el-button size="small" type="warning" icon="" @click='batchLeave()'>批量请假</el-button>
            <el-button size="small" type="success" icon="" @click='batchSignIn()'>批量签到</el-button>
        </div>

        <el-dialog :title="signInDialog.title" :visible.sync="signInDialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
            <div class="form">

                <el-form ref="studentCourseInfo" :model="signInDialog.studentCourseInfo" :rules="signInDialog.studentCourseInfoRules" :label-width="signInDialog.formLabelWidth" :label-position='signInDialog.labelPosition' size="mini" style="margin:10px;width:auto;" label-suffix='：'>
                    <el-form-item label="姓名">
                        {{signInDialog.studentCourseInfo.student_name}}
                    </el-form-item>
                    <el-form-item label="上课时间">
                        {{signInDialog.studentCourseInfo.student_course_date + " " + signInDialog.studentCourseInfo.student_course_time}}
                    </el-form-item>
                    <el-form-item label="上课教师">
                        <el-select v-model="signInDialog.studentCourseInfo.course_teacher_code" placeholder="选择上课教师" size='small'>
                            <el-option
                                v-for="item in teacherList"
                                :key="item.teacher_code"
                                :label="item.teacher_name"
                                :value="item.teacher_code">
                            </el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="作品描述">
                        <el-input></el-input>
                    </el-form-item>
                    <el-form-item label="作品上传">
                        <el-upload class="upload-demo" action="https://jsonplaceholder.typicode.com/posts/" :on-preview="handleImgPreview" :on-remove="handleImgRemove" list-type="picture">
                            <el-button size="small" type="primary">点击上传</el-button>
                        </el-upload>
                    </el-form-item>
                    <el-form-item  class="text_right">
                        <el-button @click="signInDialog.show = false">取 消</el-button>
                        <el-button type="primary">签 到</el-button>
                    </el-form-item>
                </el-form>
            </div>
        </el-dialog>
    </div>
</template>

<script>
    export default {
        data(){
            return {
                AttendanceList: [
                    {
                        student_code: '201807001',
                        student_name: '杨子铭',
                        student_course_date: '2018-09-09',
                        student_course_time: '16:00-17:30',   
                        student_course_content:'国画',
                        course_teacher_code:'',
                        teachers:[{
                            teacher_code: '001',
                            teacher_name: '唐得红',
                        },
                        {
                            teacher_code: '002',
                            teacher_name: '马朝',
                        }],                    
                    },
                    {
                        student_code: '201807001',
                        student_name: '杨子铭',
                        student_course_date: '2018-09-09',
                        student_course_time: '16:00-17:30',     
                        student_course_content:'国画',
                        course_teacher_code:'',
                        teachers:[{
                            teacher_code: '001',
                            teacher_name: '唐得红',
                        },
                        {
                            teacher_code: '002',
                            teacher_name: '马朝',
                        }],                    
                    },
                    {
                        student_code: '201807001',
                        student_name: '杨子铭',
                        student_course_date: '2018-09-09',
                        student_course_time: '17:30-19:00',     
                        student_course_content:'国画',
                        course_teacher_code:'',
                        teachers:[{
                            teacher_code: '001',
                            teacher_name: '唐得红',
                        },
                        {
                            teacher_code: '002',
                            teacher_name: '马朝',
                        }],                    
                    },
                ],
                dateRowSpanArray:[],
                timeRowSpanArray:[],
                // 以后可以缓存
                teacherList:[{
                    teacher_code: '001',
                    teacher_name: '唐得红',
                },{
                    teacher_code: '002',
                    teacher_name: '马朝',
                }],
                selectedStudents: [],
                loading:false,                
                tableHeight: this.$store.state.page.win_content.height-128,

                signInDialog:{
                    title:'签到详细信息',
                    show:false,
                    labelPosition: 'right',
                    formLabelWidth: '120px',
                    width: '400px',
                    studentCourseInfo:{
                        student_code: '201807001',
                        student_name: '杨子铭',
                        student_course_date: '2018-09-09',
                        student_course_time: '17:30-19:00',     
                        student_course_content:'国画',
                        course_teacher_code:'',
                    },
                    studentCourseInfoRules:{

                    }
                }
            }
        },
        created(){
            this.getRowSpanInfo();
        },
        methods: {
            courseTag(course) {
                let basic = '';
                switch(course) {
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
                this.dialog.show  = true;
            },
            batchLeave() {
                alert('批量请假')
                if(this.selectedStudents.length === 0){
                    this.$message({
                        message: '请选择至少一条记录',
                        type: 'warning'
                    });
                    return false;
                }
            },
            batchSignIn(){
                alert('批量签到')
                if(this.selectedStudents.length === 0){
                    this.$message({
                        message: '请选择至少一条记录',
                        type: 'warning'
                    });
                    return false;
                }
            },
            singleLeave(){
                alert("只需要修改状态为请教，未排课时数+1");
            },
            singleSignIn(row){
                this.signInDialog.show = true;
                this.signInDialog.studentCourseInfo = {
                    student_code: row.student_code,
                    student_name: row.student_name,
                    student_course_date: row.student_course_date,
                    student_course_time: row.student_course_time,     
                    student_course_content: row.student_course_content,
                    course_teacher_code: row.course_teacher_code,
                };
            },
            handleSelectionChange(allItems){
                this.selectedStudents = allItems;
            },
            getRowSpanInfo(){
                this.dateRowSpanArray = [];
                this.timeRowSpanArray = [];
                let courseDate = '';
                let courseTime = '';
                let dateIndex = 0;
                let timeIndex = 0;
                this.AttendanceList.forEach((item, index, array) => {
                    this.dateRowSpanArray.push(1);
                    this.timeRowSpanArray.push(1);
                    if(index === 0){
                        courseDate = item.student_course_date;
                        courseTime = item.student_course_time;
                    }
                    else{
                        if(item.student_course_date === courseDate){                            
                            this.dateRowSpanArray[dateIndex] += 1;
                            this.dateRowSpanArray[index] = 0;
                        } else{
                            courseDate = item.student_course_date;
                            dateIndex = index;
                        }

                        if(item.student_course_time === courseTime){
                            this.timeRowSpanArray[timeIndex] += 1;
                            this.timeRowSpanArray[index] = 0;
                        }else{
                            courseTime = item.student_course_time;
                            timeIndex = index;
                        }
                        
                    }
                });
            },
            objectSpanMethod({ row, column, rowIndex, columnIndex }) {
                if (columnIndex === 0) {
                    return {
                        rowspan: this.dateRowSpanArray[rowIndex],
                        colspan: 1
                    };
                }
                if(columnIndex === 1){
                    return {
                        rowspan: this.timeRowSpanArray[rowIndex],
                        colspan: 1
                    };
                }
            },
            handleImgPreview(file){

            },
            handleImgRemove(file, fileList) {
                //console.log(file, fileList);
            },
        }
    }
</script>

<style lang="less" scoped>
    .footer_container{
        height: 36px;
        line-height: 36px;
        margin-top: 10px;
        text-align: center;
    }
    .search-form{
        width: 100%;
        min-width: 750px;
    }
    .pagination{
        text-align: left;
        margin-top: 10px
    }
</style>


