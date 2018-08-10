<template>
     <div class="fillcontain">
        <div class="search_container">
            <el-form :inline="true" :model="searchField" :rules="search_form_rules" ref="searchField" class="demo-form-inline search-form">
                <el-form-item prop='student_name' label="学生姓名：">
                    <el-input type="text" v-model="searchField.student_name" placeholder="请输入学生姓名"></el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="el-icon-search" @click='searchStudent("searchField")'>筛选</el-button>
                </el-form-item>

                <el-form-item class="btnRight">
                    <el-button type="primary" icon="el-icon-plus" @click='addStudent()'>添加</el-button>
                </el-form-item>
            </el-form>
        </div>
        <div class="table_container">
            <el-table :data="studentsList" v-loading="loading" style="width: 100%" align="center" :max-height="tableHeight">
                <el-table-column prop="student_code" label="学号" align='center' width="100" sortable fixed>
                </el-table-column>
                <el-table-column prop="student_name" label="姓名" align='center' min-width="100" fixed>
                </el-table-column>
                <el-table-column prop="student_sex" label="性别" align='center' width="80">
                </el-table-column>
                <el-table-column prop="student_birthday" label="出生日期" align='center' min-width="120">
                </el-table-column>
                <el-table-column prop="student_identity_card_num" label="身份证号码" align='center' width="170">
                </el-table-column>
                <el-table-column prop="student_phone" label="联系电话" align='center' width="120">
                </el-table-column>
                <el-table-column prop="student_register_date" label="报名时间" align='center' min-width="110" sortable>
                </el-table-column>
                <el-table-column prop="student_address" label="家庭地址" align='center' min-width="240">
                </el-table-column>                
                <el-table-column prop="student_courses" label="学习课程" align='left' min-width="150">
                    <template slot-scope="scope">
                        <el-tag :type="courseTag(item.course)" v-for="item in scope.row.student_courses" :key="item.course" :disable-transitions="false">
                            {{item.course}}
                        </el-tag>
                    </template> 
                </el-table-column>
                <el-table-column prop="student_status" label="学生状态" align='center' width="100">
                     <template slot-scope="scope">  
                        <span style="color:#00d053">{{ scope.row.student_status }}</span>
                    </template>
                </el-table-column>
                <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                    <template slot-scope='scope'>
                        <!-- <el-button type="warning" icon='edit' size="small" @click='editStudent(scope.row)'>编辑</el-button> -->
                        <el-button type="success" icon='edit' size="small" @click='showStudentDetail(scope.row.student_code)'>查看详细</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <el-row>
                <el-col :span="24">
                    <div class="pagination">
                        <el-pagination v-if="paginations.total > 0" 
                            :page-sizes="paginations.page_sizes" 
                            :page-size="paginations.page_size"
                            :layout="paginations.layout"
                            :total="paginations.total"
                            :current-page="paginations.current_page_index"
                            @current-change='handleCurrentChange'
                            @size-change='handleSizeChange'>

                        </el-pagination>
                    </div>
                </el-col>
            </el-row>
        </div>
        <el-dialog :title="dialog.title" :visible.sync="dialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
            <div class="form">
                <el-form ref="studentInfo" :model="studentInfo" :rules="studentInfoRules" :label-width="dialog.formLabelWidth" :label-position='dialog.labelPosition' style="margin:10px;width:auto;">
                    <el-form-item label="姓名">
                        <el-input v-model="studentInfo.student_name"></el-input>
                    </el-form-item>
                    <el-form-item label="性别">
                        <el-radio-group v-model="studentInfo.student_sex">
                            <el-radio label="男"></el-radio>
                            <el-radio label="女"></el-radio>
                        </el-radio-group>
                    </el-form-item>
                    <el-form-item label="出生日期">
                        <el-date-picker v-model="studentInfo.student_birthday" type="date" placeholder="选择日期"> </el-date-picker>
                    </el-form-item>
                    <el-form-item label="身份证号码">
                        <el-input v-model="studentInfo.student_identity_card_num"></el-input>
                    </el-form-item>
                    <el-form-item label="联系电话">
                        <el-input v-model="studentInfo.student_phone"></el-input>
                    </el-form-item>
                    <el-form-item label="家庭地址">
                        <el-input v-model="studentInfo.student_address"></el-input>
                    </el-form-item>
                    <el-form-item label="备注">
                        <el-input type="textarea" v-model="studentInfo.student_remark"></el-input>
                    </el-form-item>
                    <el-form-item label="报名时间">
                        <el-date-picker v-model="studentInfo.student_register_date" type="date" placeholder="选择日期"> </el-date-picker>
                    </el-form-item>
                    <el-form-item  class="text_right">
                        <el-button @click="dialog.show = false">取 消</el-button>
                        <el-button type="primary">提  交</el-button>
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
                studentsList: [
                    {
                        student_code: '201807001',
                        student_name: '杨子铭',
                        student_sex: '男',
                        student_birthday: '2008-06-19',
                        student_identity_card_num: '110108200806191431',
                        student_phone: '13901253064',
                        student_courses: [{course:'国画'},{course:'西画'}],
                        student_register_date: '2018-07-01',
                        student_address: '北京市昌平区天巢园小区',
                        student_status: '正常在学',
                    },
                    {
                        student_code: '201807002',
                        student_name: '欧阳蛋蛋',
                        student_sex: '女',
                        student_birthday: '2018-07-31',
                        student_identity_card_num: '110108200006191431',
                        student_phone: '13901253061',
                        student_courses: [{course:'国画'}],
                        student_register_date: '2018-07-16',
                        student_address: '北京市昌平区龙兴园小区',
                        student_status: '正常在学',
                    },
                    {
                        student_code: '201806001',
                        student_name: '金佳义',
                        student_sex: '男',
                        student_birthday: '2018-07-31',
                        student_identity_card_num: '110108200006191431',
                        student_phone: '13901253060',
                        student_courses: [{course:'书法'},{course:'西画'}],
                        student_register_date: '2018-07-16',
                        student_address: '北京市昌平区龙兴园小区',
                        student_status: '正常在学',
                    },
                ],
                searchField: {
                    student_name: ''
                },
                loading:false,
                tableHeight: this.$store.state.page.win_content.height-128,   
                search_form_rules:{
                    student_name: [{
                        required: false,
                        message : '学生姓名不能为空',
                        trigger : 'blur'
                    }]
                },
                paginations: {
                    current_page_index: 1,
                    total: 3,
                    page_size: 15,
                    page_sizes: [10, 15, 20, 30],
                    layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
                },
                dialog: {
                    width: '500px',
                    show: false,
                    title: '添加学生',
                    labelPosition: 'right',
                    formLabelWidth: '120px'
                },
                studentInfo:{

                },
                studentInfoRules:{

                }
            }
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
            showStudentDetail(){
                alert('待开发')
            },
            searchStudent() {
                alert('待开发')
            }
        }
    }
</script>

<style lang="less" scoped>
    .btnRight{
        float: right;
        margin-right: 10px !important;
    }
    .search_container{
        height: 36px;
        line-height: 36px;
        margin-bottom: 10px;
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

