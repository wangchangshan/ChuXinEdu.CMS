<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" :rules="search_form_rules" ref="searchField" class="demo-form-inline search-form">
            <el-form-item prop='student_name' label="学生姓名：">
                <el-input type="text" size="small" v-model="searchField.student_name" placeholder="请输入学生姓名"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchStudent("searchField")'>查询</el-button>
            </el-form-item>

            <el-form-item class="btnRight">
                <el-button type="primary" icon="el-icon-plus" size="small" @click='showAddStudent()'>添加</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="studentsList" v-loading="loading" style="width: 100%" border stripe align="center" size="mini" :max-height="tableHeight">
            <el-table-column type="index" width="50" fixed></el-table-column>
            <el-table-column prop="studentName" label="姓名" align='center' min-width="90" fixed>
            </el-table-column>
            <el-table-column prop="studentSex" label="性别" align='center' width="60">
            </el-table-column>
            <el-table-column prop="studentBirthday" label="出生日期" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="studentPhone" label="联系电话" align='center' width="120">
            </el-table-column>
            <el-table-column prop="studentAddress" label="家庭地址" align='left' min-width="240">
            </el-table-column>
            <el-table-column prop="studentCourseCategory" label="学习课程" align='left' min-width="150">
                <template slot-scope="scope">
                    <el-tag :type="courseCategoryTag(item.code)" v-for="item in scope.row.studentCourseCategory" :key="item.id" :disable-transitions="false">
                        {{item.name}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="studentStatus" label="学生状态" align='center' width="100" :filters="dicList.studentStatusList" :filter-method="filterStudentStatus">
                <template slot-scope="scope">
                    <el-tag :type="studentStatusTag(scope.row.studentStatus)" :disable-transitions="false">
                        {{scope.row.studentStatusDesc}}
                    </el-tag>
                    <!-- <span style="color:#00d053">{{ scope.row.studentStatusDesc }}</span> -->
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                <template slot-scope='scope'>
                    <el-button type="success" icon='edit' size="mini" @click='showStudentDetail(scope.row.studentCode,scope.row.studentName)'>查看详细</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="paginations.page_size" :layout="paginations.layout" :total="paginations.total" :current-page="paginations.current_page_index" @current-change='handlePageCurrentChange' @size-change='handlePageSizeChange'>

                    </el-pagination>
                </div>
            </el-col>
        </el-row>
    </div>
    <el-dialog :title="studentDialog.title" :width="studentDialog.width" :visible.sync="studentDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="baseInfo" :model="studentDialog.baseInfo" :rules="studentDialog.baseInfoRules" :label-width="studentDialog.formLabelWidth" :label-position='studentDialog.labelPosition' size="mini">
                <el-form-item prop="studentName" label="姓名">
                    <el-input v-model="studentDialog.baseInfo.studentName"></el-input>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="studentDialog.baseInfo.studentSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="studentDialog.baseInfo.studentBirthday" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="身份证号码">
                    <el-input v-model="studentDialog.baseInfo.studentIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item prop="studentPhone" label="联系电话">
                    <el-input v-model="studentDialog.baseInfo.studentPhone"></el-input>
                </el-form-item>
                <el-form-item prop="studentAddress" label="家庭地址">
                    <el-input v-model="studentDialog.baseInfo.studentAddress"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="studentDialog.baseInfo.studentRemark"></el-input>
                </el-form-item>
                <el-form-item label="报名时间">
                    <el-date-picker v-model="studentDialog.baseInfo.studentRegisterDate" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="studentDialog.isShow = false">取 消</el-button>
                    <el-button size="small" type="primary" @click="submitAddStudent('baseInfo')">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    data() {
        return {
            studentsList: [],
            dicList: {
                studentStatusList: [{
                    text: '试听',
                    value: '00'
                }, {
                    text: '正常在学',
                    value: '01'
                }, {
                    text: '中途退费',
                    value: '02'
                }, {
                    text: '结束未续费',
                    value: '03'
                }],
            },
            searchField: {
                student_name: ''
            },
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 63,
            search_form_rules: {
                student_name: [{
                    required: false,
                    message: '学生姓名不能为空',
                    trigger: 'blur'
                }]
            },
            paginations: {
                current_page_index: 1,
                total: 3,
                page_size: 15,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
            studentDialog: {
                width: '600px',
                isShow: false,
                title: '添加学生基本信息',
                labelPosition: 'right',
                formLabelWidth: '90px',
                baseInfo: {
                    studentName: "",
                    studentSex: "",
                    studentBirthday: "",
                    studentIdentityCardNum: "",
                    studentPhone: "",
                    studentAddress: "",
                    studentRemark: "",
                    studentRegisterDate: "",
                    studentStatus: "01"
                },
                baseInfoRules: {
                    studentPhone: [{
                        required: true,
                        message: '请输入联系电话',
                        trigger: 'blur'
                    }],
                    studentAddress: [{
                        required: true,
                        message: '请输入家庭地址',
                        trigger: 'blur'
                    }],
                    studentName: [{
                        required: true,
                        message: '请输入学生姓名',
                        trigger: 'blur'
                    }]
                }
            }
        }
    },
    created() {
        this.getList();
    },
    methods: {
        /**
         * 改变页码和当前页时需要拼装的路径方法
         * @param {string} field 参数字段名
         * @param {string} value 参数字段值
         */
        setPath(field, value) {
            var path = this.$route.path,
                query = Object.assign({}, this.$route.query);
            if (typeof field === 'object') {
                query = field;
            } else {
                query[field] = value;
            }
            this.$router.push({
                path,
                query
            });
        },
        getList({
            page,
            pageSize,
            where,
            fun
        } = {}) {
            var _this = this;
            var query = this.$route.query;
            this.paginations.current_page_index = page || parseInt(query.page) || 1;
            this.paginations.page_size = pageSize || parseInt(query.page_size) || this.paginations.page_size;
            var data = {
                pageIndex: this.paginations.current_page_index,
                pageSize: this.paginations.page_size
            }
            if (where) {
                data = Object.assign(data, where || {});
            }
            axios({
                type: 'get',
                path: '/api/student/getstudentlist',
                data: data,
                fn: function (result) {
                    _this.paginations.total = result.length;
                    result.forEach((item) => {
                        item.studentStatusDesc = _this.getStudentStatusDesc(item.studentStatus);
                        item.studentBirthday = item.studentBirthday.split('T')[0];
                    })
                    _this.studentsList = result;
                    fun && fun();
                }
            })
        },
        filterStudentStatus(value, row, column) {
            return row['studentStatus'] === value;
        },
        courseCategoryTag(categoryCode) {
            let type = '';
            switch (categoryCode) {
                case 'meishu':
                    type = 'success'
                    break;
                case 'shufa':
                    type = ''
                    break;
            }
            return type;
        },
        studentStatusTag(statusCode) {
            let type = '';
            switch (statusCode) {
                case '00': // 试听
                    type = ''
                    break;
                case '01': // 正常在学
                    type = 'success'
                    break;
                case '02': // 中途退费
                    type = 'danger'
                    break;
                case '03': // 结束未续费
                    type = 'info'
                    break;
            }
            return type;
        },

        // will replace this to util function. GetLabelByValue
        getStudentStatusDesc(statusCode) {
            let statusDesc = '';
            for (let obj of this.dicList.studentStatusList) {
                if (obj['value'] == statusCode) {
                    statusDesc = obj['text'];
                    break;
                }
            }
            return statusDesc;
        },
        handlePageSizeChange(pageSize) {
            this.getList({
                pageSize,
                fun: () => {
                    this.setPath('page_size', pageSize);
                }
            });
            console.log(`每页 ${pageSize} 条`);
        },
        handlePageCurrentChange(page) {
            this.getList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
            //console.log(`当前页: ${page}`);
        },
        showAddStudent() {
            this.studentDialog.isShow = true;
        },

        submitAddStudent(studentForm) {
            var _this = this;
            this.$refs[studentForm].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'post',
                        path: '/api/student/addstudent',
                        data: _this.studentDialog.baseInfo,
                        fn: function (result) {
                            if (result != "") {
                                _this.$message({
                                    message: '添加学生成功',
                                    type: 'success'
                                });
                                _this.studentDialog.isShow = false;
                                _this.$router.push({
                                    path: '/studentDetailMain',
                                    query: {
                                        studentcode: result
                                    }
                                });
                            }
                        }
                    });
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
            })
        },
        searchStudent() {
            alert('待开发')
        }
    }
}
</script>

<style lang="less" scoped>
.btnRight {
    float: right;
    margin-right: 10px !important;
}

.search_container {
    height: 36px;
    line-height: 36px;
    margin-bottom: 10px;
}

.search-form {
    width: 100%;
    min-width: 750px;
}

.pagination {
    text-align: left;
    margin-top: 10px
}
</style>
