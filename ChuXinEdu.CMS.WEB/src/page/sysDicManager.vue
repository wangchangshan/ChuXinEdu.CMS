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
            <el-table :data="dicList" v-loading="loading" style="width: 100%" align="center" :max-height="tableHeight">
                <el-table-column prop="student_code" label="字典名称" align='center' width="100" sortable fixed>
                </el-table-column>
                <el-table-column prop="student_name" label="唯一编码" align='center' min-width="100" fixed>
                </el-table-column>
                <el-table-column prop="student_sex" label="排序" align='center' width="80">
                </el-table-column>
                <el-table-column prop="student_birthday" label="是否启用" align='center' min-width="120">
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
                dicList: [
                    {
                        dic_type_code: 'coure_category',
                        dic_type_name: '课程类别',
                        dic_sort_weight: '1',
                        dic_enabled: 'Y',                        
                    },
                    {
                        dic_type_code: 'coure_category',
                        dic_type_name: '课程类别',
                        dic_sort_weight: '1',
                        dic_enabled: 'Y',   
                    },
                    {
                        dic_type_code: 'coure_category',
                        dic_type_name: '课程类别',
                        dic_sort_weight: '1',
                        dic_enabled: 'Y',   
                    },
                ],
                searchField: {
                    student_name: ''
                },
                loading:false,
                tableHeight: this.$store.state.page.win_content.height-63,   
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

