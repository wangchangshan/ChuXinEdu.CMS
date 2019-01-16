<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" class="demo-form-inline search-form">
            <el-form-item label="姓名：">
                <el-input type="text" size="small" v-model="searchField.teacherName" placeholder="请输入姓名" class="search_field"></el-input>
            </el-form-item>
            <el-form-item label="状态：">
                <el-select size="small" v-model="searchField.teacherStatus" placeholder="请选择教师状态" class="search_field" :clearable="true">
                    <el-option v-for="item in $store.getters['teacher_status']" :key="item.value" :label="item.label" :value="item.value">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchTeacher()'>查 询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetTeacherList()'>重 置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" icon="el-icon-plus" @click='showAddTeacherPanel()'>添加</el-button>
                <el-button v-noRepeatClick type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="teacherList" v-loading="loading" style="width: 100%" align="left" border stripe size="mini" :height="tableHeight">
            <el-table-column type="index" align='center' width="40" fixed></el-table-column>
            <el-table-column prop="teacherName" label="教师姓名" align='center' min-width="110" fixed sortable>
            </el-table-column>
            <el-table-column prop="teacherSex" label="性别" align='center' width="70">
            </el-table-column>
            <el-table-column prop="teacherPhone" label="联系电话" align='center' min-width="110">
            </el-table-column>
            <el-table-column prop="teacherAddress" label="家庭地址" align='left' min-width="150">
            </el-table-column>
            <el-table-column prop="teacherStatus" label="状态"  align='center' width="100">
                <template slot-scope="scope">
                    <el-tag :type="teacherStatusTag(scope.row.teacherStatus)" :disable-transitions="false">
                        {{scope.row.teacherStatusDesc}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="teacherRegisterDate" label="入职日期" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="100">
                <template slot-scope='scope'>
                    <el-button type="success" size="mini" @click='showTeacherDetail(scope.row.teacherCode,scope.row.teacherName)'>详 细</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <el-dialog :title="teacherDialog.title" :width="teacherDialog.width" :visible.sync="teacherDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="baseInfo" :model="teacherDialog.baseInfo" :rules="teacherDialog.baseInfoRules" :label-width="teacherDialog.formLabelWidth" :label-position='teacherDialog.labelPosition' size="mini">
                <el-form-item prop="teacherName" label="姓名">
                    <el-input v-model="teacherDialog.baseInfo.teacherName"></el-input>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="teacherDialog.baseInfo.teacherSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="teacherDialog.baseInfo.teacherBirthday" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item prop="teacherIdentityCardNum" label="身份证号码">
                    <el-input v-model="teacherDialog.baseInfo.teacherIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item prop="teacherPhone" label="联系电话">
                    <el-input v-model="teacherDialog.baseInfo.teacherPhone"></el-input>
                </el-form-item>
                <el-form-item prop="teacherAddress" label="家庭地址">
                    <el-input v-model="teacherDialog.baseInfo.teacherAddress"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="teacherDialog.baseInfo.teacherRemark"></el-input>
                </el-form-item>
                <el-form-item label="入职日期">
                    <el-date-picker v-model="teacherDialog.baseInfo.teacherRegisterDate" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="teacherDialog.isShow = false">取 消</el-button>
                    <el-button v-noRepeatClick size="small" type="primary" @click="submitNewTeacher('baseInfo')">提 交</el-button>
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
            loading: true,
            downloadLoading: false,
            teacherList: [],
            tableHeight: this.$store.state.page.win_content.height - 60,
            searchField: {
                teacherName: '',
                teacherStatus: ''
            },
            teacherDialog: {
                width: '600px',
                isShow: false,
                title: '',
                labelPosition: 'right',
                formLabelWidth: '100px',
                baseInfo: {
                    teacherName: "",
                    teacherSex: "",
                    teacherBirthday: "",
                    teacherIdentityCardNum: "",
                    teacherPhone: "",
                    teacherAddress: "",
                    teacherRemark: "",
                    teacherRegisterDate: "",
                    teacherStatus: "01"
                },
                baseInfoRules: {
                    teacherPhone: [{
                        required: true,
                        message: '请输入联系电话',
                        trigger: 'blur'
                    }],
                    teacherAddress: [{
                        required: true,
                        message: '请输入家庭地址',
                        trigger: 'blur'
                    }],
                    teacherName: [{
                        required: true,
                        message: '请输入教师姓名',
                        trigger: 'blur'
                    }],
                    teacherIdentityCardNum: [{
                        required: true,
                        message: '请输入身份证号码',
                        trigger: 'blur'
                    }]
                }
            }
        }
    },
    created() {
        this.getTeacherList();
    },
    methods: {
        searchTeacher() {
            this.getTeacherList();
        },
        resetTeacherList() {
            this.searchField = {
                teacherName: '',
                teacherStatus: ''
            };
            this.getTeacherList();
        },
        getTeacherList() {
            this.loading = true;
            var data = {
                q: this.searchField
            }
            axios({
                type: 'get',
                path: '/api/teacher/getteacherlist',
                data: data,
                fn: result => {
                    result.forEach(item => {
                        item.teacherStatusDesc = dicHelper.getLabelByValue(this.$store.getters['teacher_status'], item.teacherStatus);
                        item.teacherRegisterDate = item.teacherRegisterDate.split('T')[0];
                    });
                    this.teacherList = result;
                    this.loading = false;
                }
            })
        },

        showAddTeacherPanel() {
            this.teacherDialog.title = '添加教师基本信息';
            this.teacherDialog.baseInfo = {
                teacherName: "",
                teacherSex: "",
                teacherBirthday: "",
                teacherIdentityCardNum: "",
                teacherPhone: "",
                teacherAddress: "",
                teacherRemark: "",
                teacherRegisterDate: "",
                teacherStatus: "01"
            }
            this.teacherDialog.isUpdate = false;
            this.teacherDialog.isShow = true;
        },

        submitNewTeacher(teacherForm) {
            var _this = this;
            this.$refs[teacherForm].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'post',
                        path: '/api/teacher/postnewteacher',
                        data: _this.teacherDialog.baseInfo,
                        fn: function (result) {
                            if (result == 1200) {
                                _this.getTeacherList();
                                _this.teacherDialog.isShow = false;
                            }
                        }
                    })

                }
            });
            return;
        },

        showTeacherDetail(teacherCode, teacherName) {
            this.$router.push({
                path: '/teacherDetail',
                query: {
                    teacherCode: teacherCode,
                    teacherName: teacherName
                }
            })
        },

        teacherStatusTag(statusCode) {
            return tagTypeHelper.teacherStatusTag(statusCode);
        },

        export2Excle() {
            if (this.teacherList.length == 0) {
                this.$message({
                    message: '没有数据需要导出！',
                    type: 'success'
                });
                return;
            }
            this.downloadLoading = true
            import('@/vendor/Export2Excel').then(excel => {
                const tHeader = ['教师编码', '姓 名', '性 别', '出生日期', '身份证号', '联系电话', '家庭住址', '入职日期'];
                const filterVal = ['teacherCode', 'teacherName', 'teacherSex', 'teacherBirthday', 'teacherIdentityCardNum', 'teacherPhone', 'teacherAddress', 'teacherRegisterDate']
                const data = this.formatJson(filterVal, this.teacherList)
                excel.export_json_to_excel({
                    header: tHeader,
                    data,
                    filename: '教师列表',
                    autoWidth: true,
                    bookType: 'xlsx'
                })
                this.downloadLoading = false;
            })
        },
        formatJson(filterVal, jsonData) {
            return jsonData.map(v => filterVal.map(j => {
                if (j === 'teacherRegisterDate' || j === 'teacherBirthday') {
                    return v[j] && v[j].split('T')[0] || '';
                } else {
                    return v[j]
                }
            }))
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

.search_field {
    width: 160px;
}
</style>
