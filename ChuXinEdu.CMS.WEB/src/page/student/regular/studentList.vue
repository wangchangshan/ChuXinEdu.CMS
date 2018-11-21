<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" class="demo-form-inline search-form">
            <!-- <el-form-item label="学号">not show for ipad UI
                <el-input type="text" size="small" v-model="searchField.studentCode" placeholder="请输入学号" class="search_field"></el-input>
            </el-form-item> -->
            <el-form-item label="姓名">
                <el-input type="text" size="small" v-model="searchField.studentName" placeholder="请输入学生姓名" class="search_field"></el-input>
            </el-form-item>
            <el-form-item label="状态">
                <el-select size="small" v-model="searchField.studentStatus" placeholder="请选择学生状态" class="search_field" :clearable="true">
                    <el-option v-for="item in $store.getters['student_status']" :key="item.value" :label="item.label" :value="item.value">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchStudent()'>查询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetStudentList()'>重置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" @click='showAddStudent()'><i class="fa fa-user-plus" aria-hidden="true"></i> 添加</el-button>
                <el-button type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="studentsList" v-loading="loading" style="width: 100%" border stripe align="center" size="mini" :max-height="tableHeight">
            <el-table-column prop="studentCode" label="学号" align='center' min-width="105" sortable fixed>
            </el-table-column>
            <el-table-column prop="studentName" label="姓名" align='center' min-width="75" fixed>
                <template slot-scope="scope">
                    <el-popover trigger="hover" placement="right-end" width="150">
                        <img :src="scope.row.studentAvatarPath" class='avatar-min'>
                        <div slot="reference" class="name-wrapper" style="display:inline">
                            {{ scope.row.studentName }}
                        </div>
                    </el-popover>
                </template>
            </el-table-column>
            <el-table-column prop="studentSex" label="性别" align='center' width="50">
            </el-table-column>
            <el-table-column prop="studentBirthday" label="生日" align='center' min-width="90">
            </el-table-column>
            <el-table-column prop="studentPhone" label="联系电话" align='center' width="110">
            </el-table-column>
            <el-table-column prop="studentAddress" label="家庭地址" align='left' min-width="145">
            </el-table-column>
            <el-table-column prop="studentCourseCategory" label="学习课程" align='left' min-width="129">
                <template slot-scope="scope">
                    <el-tag :type="courseCategoryTag(item.code)" v-for="item in scope.row.studentCourseCategory" :key="item.id" :disable-transitions="false">
                        {{item.name}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="studentStatus" label="学生状态" align='center' min-width="70">
                <template slot-scope="scope">
                    <el-tag :type="studentStatusTag(scope.row.studentStatus)" :disable-transitions="false">
                        {{scope.row.studentStatusDesc}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="90">
                <template slot-scope='scope'>
                    <el-button type="success" size="mini" @click='showStudentDetail(scope.row.studentCode,scope.row.studentName)'>详 细</el-button>
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
                <el-form-item prop="studentRegisterDate" label="报名时间">
                    <el-date-picker v-model="studentDialog.baseInfo.studentRegisterDate" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="身份证号码">
                    <el-input v-model="studentDialog.baseInfo.studentIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="studentDialog.baseInfo.studentRemark"></el-input>
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
    axios,
    dicHelper,
    tagTypeHelper
} from '@/utils/index'

export default {
    data() {
        return {
            loading: false,
            downloadLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 106,
            lookup: null,
            studentsList: [],
            searchField: {
                studentCode: '',
                studentName: '',
                studentStatus: '',
            },
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
                    }],
                    studentRegisterDate: [{
                        required: true,
                        message: '请输入报名日期',
                        trigger: 'blur'
                    }],
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
                pageSize: this.paginations.page_size,
                q: this.searchField
            }
            if (where) {
                data = Object.assign(data, where || {});
            }
            axios({
                type: 'get',
                path: '/api/student/getstudentlist',
                data: data,
                fn: function (result) {
                    _this.paginations.total = result.totalCount;
                    result.data.forEach((item) => {
                        item.studentStatusDesc = dicHelper.getLabelByValue(_this.$store.getters['student_status'], item.studentStatus);
                    });
                    _this.studentsList = result.data;
                    fun && fun();
                }
            })
        },
        courseCategoryTag(categoryCode) {
            return tagTypeHelper.courseCategoryTag(categoryCode);
        },
        studentStatusTag(statusCode) {
            return tagTypeHelper.studentStatusTag(statusCode);
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
                                    path: '/studentDetail',
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
        resetStudentList() {
            this.searchField = {
                studentCode: '',
                studentName: '',
                studentStatus: '',
            };
            this.getList();
        },

        export2Excle() {
            let data = {
                q: this.searchField
            }
            this.downloadLoading = true
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getstudentlist2export',
                data: data,
                fn: function (result) {
                    import('@/vendor/Export2Excel').then(excel => {
                        const tHeader = ['学 号', '姓 名', '性 别', '电 话', '生 日', '地 址','报名时间','备 注'];
                        const filterVal = ['studentCode', 'studentName', 'studentSex', 'studentPhone', 'studentBirthday', 'studentAddress','studentRegisterDate', 'studentRemark']
                        const data = _this.formatJson(filterVal, result)
                        excel.export_json_to_excel({
                            header: tHeader,
                            data,
                            filename: "学生列表",
                            autoWidth: true,
                            bookType: 'xlsx'
                        })
                        _this.downloadLoading = false;
                    })
                }
            })
        },
        formatJson(filterVal, jsonData) {
            return jsonData.map(v => filterVal.map(j => {
                if (j === 'studentBirthday' || j === 'studentRegisterDate') {
                    return v[j] && v[j].split('T')[0] || '';//parseTime(v[j])
                } else {
                    return v[j]
                }
            }))
        }
    }
}
</script>

<style lang="less" scoped>
img.avatar-min {
    width: 145px;
    height: 145px;
}

.btnRight {
    float: right;
    margin-right: 10px !important;
}

.search_field {
    width: 140px;
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
