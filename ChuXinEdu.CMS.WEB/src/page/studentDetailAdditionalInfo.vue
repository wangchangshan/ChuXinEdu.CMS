<template>
<div class="info_container">
    <el-card shadow="never">
        <el-collapse v-model="activeNames">
            <el-collapse-item name="1">
                <template slot="title">
                    <a class="sub-title">新科目试听</a>
                </template>
                <el-switch active-color="#13ce66" v-model="trialOtherCourse" inactive-color="#dcdfe6" active-text="开启" active-value="是" inactive-text="不开启" inactive-value="否">

                </el-switch>
            </el-collapse-item>
            <el-collapse-item name="2">
                <template slot="title">
                    <a class="sub-title">我的报名介绍人</a>
                </template>
                <div>
                    {{ myIntroducer.studentName }} 
                    <el-button v-if="myIntroducer.studentName != '无'" type="success" size="mini" @click='showStudentDetail(myIntroducer.studentCode,myIntroducer.studentName)'>查 看</el-button>
                </div>
            </el-collapse-item>

            <el-collapse-item name="3">
                <template slot="title">
                    <a class="sub-title">我介绍的新学员</a>
                </template>
                <el-table :data="myRecommendStudentList" v-loading="loading" style="width: auto" align="left" size="mini">
                    <el-table-column prop="newStudentCode" label="学号" align='center' width="150">
                    </el-table-column>
                    <el-table-column prop="newStudentName" label="姓名" align='center' width="200">
                    </el-table-column>
                    <el-table-column prop="operation" align='center' label="操作" width="180">
                        <template slot-scope='scope'>
                            <el-button type="success" size="mini" @click='showStudentDetail(scope.row.newStudentCode,scope.row.newStudentName)'>查 看</el-button>
                            <el-button type="danger" size="mini" @click='removeRecommend(scope.row.id)'>移 除</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <div style="margin-top:10px">
                    <el-button type="primary" icon="el-icon-plus" size="mini" @click="showAddRecommendPanel()">添 加</el-button>
                </div>
            </el-collapse-item>
        </el-collapse>
    </el-card>

    <el-dialog :title="searchStudentDialog.title" :width="searchStudentDialog.width" :visible.sync="searchStudentDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form :inline="true" ref="searchFrom" :model="searchStudentDialog.searchField" :rules="searchStudentDialog.searchFieldRules" class="demo-form-inline search-form">
                <el-form-item prop="studentName" label="学生姓名：">
                    <el-input type="text" size="small" v-model="searchStudentDialog.searchField.studentName" placeholder="请输入学生姓名"></el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="el-icon-search" size="small" @click='searchStudent("searchFrom")'>查 询</el-button>
                    <el-button type="" icon="el-icon-close" size="small" @click="searchStudentDialog.isShow = false">关 闭</el-button>
                </el-form-item>
            </el-form>
            <el-table :data="searchStudentDialog.studentList" v-loading="searchStudentDialog.loading" style="width: auto" align="left" size="mini">
                <el-table-column prop="student_code" label="学号" align='center' width="110">
                </el-table-column>
                <el-table-column prop="student_name" label="姓名" align='center' width="120">
                </el-table-column>
                <el-table-column prop="student_sex" label="性别" align='center' width="70">
                </el-table-column>
                <el-table-column prop="student_register_date" label="入学日期" align='center' width="120">
                </el-table-column>
                <el-table-column prop="operation" align='center' label="操作" width="100">
                    <template slot-scope='scope'>
                        <el-button type="success" size="mini" @click='addToRecommend(scope.row)'>添 加</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    name: 'student-additional-info',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            loading: false,
            activeNames: ['1', '2', '3'],
            trialOtherCourse: '否',
            myIntroducer: {
                studentCode: '',
                studentName: '无'
            },
            myRecommendStudentList: [],
            searchStudentDialog: {
                width: '580px',
                isShow: false,
                loading: false,
                title: '查询学生',
                labelPosition: 'right',
                formLabelWidth: '420px',
                searchField: {
                    studentName: "",
                },
                searchFieldRules: {
                    studentName: [{
                        required: true,
                        message: '请输入学生姓名',
                        trigger: 'blur'
                    }]
                },
                studentList: [],
            }
        }
    },
    watch: {
        '$route'(to, from) {
            this.$router.go(0);
        },
        'trialOtherCourse'(curVal,oldVal){
            axios({
                type: 'put',
                path: '/api/student/updatetrialothercourse/' + this.studentCode,
                data: {curVal : curVal},
                fn: function (result) {
                }
            });
        }
    },
    created() {
        this.getAuxiliaryInfo();
        this.getRecommendStudents();
    },
    methods: {
        getAuxiliaryInfo(){
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getauxiliaryinfo/' + _this.studentCode,
                fn: function (result) {
                    if(result.length > 0){
                        _this.myIntroducer.studentCode = result[0].origin_student_code;
                        _this.myIntroducer.studentName = result[0].origin_student_name || '无';
                        _this.trialOtherCourse = result[0].trial_other_course;
                    }
                }
            });
        },
        getRecommendStudents() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getrecommend/' + _this.studentCode,
                fn: function (result) {
                    _this.myRecommendStudentList = result;
                }
            });
        },
        showStudentDetail(studentCode, studentName) {
            this.$router.push({
                path: '/studentDetailMain',
                query: {
                    studentcode: studentCode,
                    studentname: studentName
                }
            });
        },
        showAddRecommendPanel() {
            this.searchStudentDialog.isShow = true;
            this.searchStudentDialog.searchField.studentName = "";
        },
        searchStudent(searchFrom) {
            var _this = this;
            this.$refs[searchFrom].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'get',
                        path: '/api/student/getstudentforrecommend/' + _this.searchStudentDialog.searchField.studentName,
                        fn: function (result) {
                            result.forEach(item => {
                                item.student_register_date = item.student_register_date.split('T')[0];
                            })
                            _this.searchStudentDialog.studentList = result;
                        }
                    });
                }
            });
        },
        addToRecommend(row) {
            var newRecommend = {
                NewStudentCode: row.student_code,
                NewStudentName: row.student_name,
                OriginStudentCode: this.studentCode,
                OriginStudentName: this.$route.query.studentname
            }

            var _this = this;
            axios({
                type: 'post',
                path: '/api/student/postnewrecommend',
                data: newRecommend,
                fn: function (result) {
                    if (result === 200) {
                        _this.$message({
                            message: '添加成功',
                            type: 'success'
                        });
                        _this.getRecommendStudents();
                        _this.removeAddedStudent(row.student_code);
                    }
                }
            });
        },

        removeRecommend(id) {
            var _this = this;
            this.$confirm('确定移除这个学员吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/student/delrecommend/' + id,
                    fn: function (result) {
                        if (result === 200) {
                            _this.getRecommendStudents();
                            _this.$message({
                                message: '删除成功！',
                                type: 'success'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        removeAddedStudent(studentCode) {
            let index = -1;
            for (let i = 0; i < this.searchStudentDialog.studentList.length; i++) {
                if (this.searchStudentDialog.studentList[i].student_code == studentCode) {
                    index = i;
                    break;
                }
            }
            index != -1 && this.searchStudentDialog.studentList.splice(index, 1);
        },
    }
}
</script>
<style lang="less" scoped>
.sub-title{
    font-size: 15px;
    font-weight: 600;
}
</style>