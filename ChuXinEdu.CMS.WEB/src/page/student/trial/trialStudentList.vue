<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" class="demo-form-inline search-form">
            <el-form-item label="姓名">
                <el-input type="text" size="small" v-model="searchField.studentName" placeholder="请输入学生姓名" style="width:150px"></el-input>
            </el-form-item>
            <el-form-item label="状态">
                <el-select size="small" v-model="searchField.studentTempStatus" multiple placeholder="请选择试听状态" style="width:230px">
                    <el-option v-for="item in $store.getters['student_temp_status']" :key="item.value" :label="item.label" :value="item.value">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="结果">
                <el-select size="small" v-model="searchField.result" placeholder="请选择试听结果"  style="width:150px">
                    <el-option :key="'成功'" :label="'试听成功'" :value="'成功'"></el-option>
                    <el-option :key="'失败'" :label="'试听失败'" :value="'失败'"></el-option>
                    <el-option :key="'待定'" :label="'待定'" :value="'待定'"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchStudent()'>查询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetStudentList()'>重置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" @click='showAddStudent()'><i class="fa fa-user-plus" aria-hidden="true"></i> 添加</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="studentsList" v-loading="loading" style="width: 100%" border stripe align="center" size="mini" :height="tableHeight">
            <el-table-column type="index" align='center' width="40" fixed></el-table-column>
            <el-table-column prop="studentName" label="姓名" align='center' min-width="80" fixed>
            </el-table-column>
            <el-table-column prop="studentSex" label="性别" align='center' width="50">
            </el-table-column>
            <!-- <el-table-column prop="studentBirthday" label="出生日期" align='center' min-width="100"> 
                not show for ipad UI
            </el-table-column> -->
            <el-table-column prop="studentPhone" label="联系电话" align='center' width="110">
            </el-table-column>
            <!-- <el-table-column prop="studentAddress" label="家庭地址" align='left' min-width="150"> 
                not show for ipad UI
            </el-table-column> -->
            <el-table-column prop="trialFolderName" label="试听课程" align='left' min-width="90">
            </el-table-column>
            <el-table-column prop="studentStatusDesc" label="当前状态" align='left' width="140" :filters="$store.getters['student_temp_status']" :filter-method="filterStudentStatus">
            </el-table-column>
            <el-table-column prop="result" label="试听结果" align='center' width="90">
                <template slot-scope="scope">
                    <el-tag :type="handleResultTag(scope.row.result)" :disable-transitions="false">
                        {{scope.row.result}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="120">
                <template slot-scope='scope'>
                    <el-button v-if="scope.row.result != '成功'" @click="showEditPanel(scope.row)" type="primary" size="mini">编辑</el-button>
                    <el-button v-if="scope.row.result == '成功'" @click="showStudentDetail(scope.row.studentCode, scope.row.studentName)" type="success" size="mini">查看</el-button>
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
                    <el-date-picker v-model="studentDialog.baseInfo.studentBirthday" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item prop="studentPhone" label="联系电话">
                    <el-input v-model="studentDialog.baseInfo.studentPhone"></el-input>
                </el-form-item>
                <el-form-item prop="studentAddress" label="家庭地址">
                    <el-input v-model="studentDialog.baseInfo.studentAddress"></el-input>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="studentDialog.isShow = false">取 消</el-button>
                    <el-button v-noRepeatClick size="small" v-if="studentDialog.isUpdate == true" type="primary" @click="submitUpdateStudent('baseInfo')">保 存</el-button>
                    <el-button v-noRepeatClick size="small" v-if="studentDialog.isUpdate == false" type="primary" @click="submitAddStudent('baseInfo')">提 交</el-button>                    
                    <el-button v-noRepeatClick v-if="studentDialog.studentTempStatus != '01' && studentDialog.result != '成功' && studentDialog.isUpdate == true" @click="submitTrialSuccess(studentDialog.curId)" size="small" type="success">试听成功</el-button>
                    <el-button v-noRepeatClick v-if="studentDialog.studentTempStatus == '02' && studentDialog.result == '待定'" @click="submitTrialFail(studentDialog.curId)" size="small" type="info">试听失败</el-button>
                    <el-button v-noRepeatClick v-if="studentDialog.studentTempStatus == '00'" @click="removeTempStudent(studentDialog.curId)" size="small" type="danger">删 除</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios,
    dicHelper,
    tagTypeHelper
} from '@/utils/index'

export default {
    data() {
        return {
            studentsList: [],
            searchField: {
                studentName: '',
                studentTempStatus:[],
                result:'待定'
            },
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 106,
            paginations: {
                current_page_index: 1,
                total: 0,
                page_size: 15,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
            studentDialog: {
                width: '600px',
                isShow: false,
                isUpdate: false,
                title: '',
                labelPosition: 'right',
                formLabelWidth: '90px',
                curId: '',
                studentTempStatus: '',
                result: '',
                baseInfo: {
                    studentName: "",
                    studentSex: "",
                    studentBirthday: "",
                    studentIdentityCardNum: "",
                    studentPhone: "",
                    studentAddress: "",
                    studentTempStatus: "00"
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
            this.loading = true;
            var query = this.$route.query;
            this.paginations.current_page_index = page || parseInt(query.page) || 1;
            this.paginations.page_size = pageSize || parseInt(query.page_size) || this.paginations.page_size;
            var data = {
                pageIndex: this.paginations.current_page_index,
                pageSize: this.paginations.page_size,
                q: this.searchField
            }
            if (where) {
                data = Object.assign(data, where || {});
            }
            axios({
                type: 'get',
                path: '/api/studenttemp/getstudentlist',
                data: data,
                fn: result => {
                    this.paginations.total = result.totalCount;
                    result.studentList.forEach((item) => {
                        item.studentStatusDesc = dicHelper.getLabelByValue(this.$store.getters['student_temp_status'], item.studentTempStatus);
                        item.studentBirthday = item.studentBirthday && item.studentBirthday.split('T')[0];
                    })
                    this.studentsList = result.studentList;
                    this.loading = false;
                    fun && fun();
                }
            })
        },
        filterStudentStatus(value, row, column) {
            return row['studentTempStatus'] === value;
        },
        handleResultTag(result) {
            return tagTypeHelper.studentTrialResultTag(result);
        },

        handlePageSizeChange(pageSize) {
            this.getList({
                pageSize,
                fun: () => {
                    this.setPath('page_size', pageSize);
                }
            });
        },
        handlePageCurrentChange(page) {
            this.getList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },
        showAddStudent() {
            this.studentDialog.baseInfo = {
                studentName: "",
                studentSex: "",
                studentBirthday: "",
                studentIdentityCardNum: "",
                studentPhone: "",
                studentAddress: "",
                studentTempStatus: "00"
            };
            this.studentDialog.isShow = true;
            this.studentDialog.isUpdate = false;
            this.studentDialog.title = '添加试听学生';
        },

        submitAddStudent(studentForm) {
            var _this = this;
            this.$refs[studentForm].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'post',
                        path: '/api/studenttemp/addstudent',
                        data: _this.studentDialog.baseInfo,
                        fn: function (result) {
                            if (result == 1200) {
                                _this.getList();
                                _this.$message({
                                    message: '添加试听学生成功',
                                    type: 'success'
                                });
                                _this.studentDialog.isShow = false;
                            }
                        }
                    });
                }
            });
        },

        showEditPanel(row) {
            this.studentDialog.curId = row.id;
            this.studentDialog.isUpdate = true;
            this.studentDialog.studentTempStatus = row.studentTempStatus;
            this.studentDialog.result = row.result;
            this.studentDialog.baseInfo = {
                studentName: row.studentName,
                studentSex: row.studentSex,
                studentBirthday: row.studentBirthday,
                studentPhone: row.studentPhone,
                studentAddress: row.studentAddress
            };
            this.studentDialog.isShow = true;
            this.studentDialog.title = '编辑试听学生';
        },

        submitUpdateStudent(studentForm) {
            var _this = this;
            this.$refs[studentForm].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'put',
                        path: '/api/studenttemp/updatestudent/' + _this.studentDialog.curId,
                        data: _this.studentDialog.baseInfo,
                        fn: function (result) {
                            if (result == 1200) {
                                _this.getList();
                                _this.$message({
                                    message: '修改成功',
                                    type: 'success'
                                });
                                _this.studentDialog.isShow = false;
                            }
                        }
                    });
                }
            });
        },

        removeTempStudent(id) {
            axios({
                type: 'delete',
                path: '/api/studenttemp/removestudent/' + id,
                fn: (result) => {
                    if (result == 1200) {
                        this.getList();
                        this.$message({
                            message: '删除成功',
                            type: 'success'
                        });
                    }
                }
            })
        },

        submitTrialSuccess(id) {
            axios({
                type: 'put',
                path: '/api/studenttemp/trialsuccess/' + id,
                fn: (result) =>  {
                    if (result == 1200) {
                        this.studentDialog.isShow = false;
                        this.getList();
                        this.$message({
                            message: '已经变更为正式学员！',
                            type: 'success'
                        });
                    }
                }
            })
        },

        submitTrialFail(id) {
            axios({
                type: 'put',
                path: '/api/studenttemp/trialfail/' + id,
                fn: (result) => {
                    if (result == 1200) {
                        this.studentDialog.isShow = false;
                        this.getList();
                        this.$message({
                            message: '已经标记为试听失败学生',
                            type: 'danger'
                        });
                    }
                }
            })
        },

        showStudentDetail(studentCode, studentName) {
            this.$router.push({
                path: '/studentDetail',
                query: {
                    studentcode: studentCode,
                    studentname: studentName
                }
            })
        },

        searchStudent() {
            var page = 1;
            this.paginations.current_page_index = 1;
            this.getList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },

        resetStudentList(){
            this.searchField = {
                studentName: '',
                studentTempStatus: [],
                result: ''
            };
            this.getList();
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
