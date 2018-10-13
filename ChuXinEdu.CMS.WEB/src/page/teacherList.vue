<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" class="demo-form-inline search-form">
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" icon="el-icon-plus" @click='showAddTeacherPanel()'>添加</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="teacherList" v-loading="loading" style="width: 100%" align="left" stripe size="mini" :max-height="tableHeight">
            <el-table-column type="index" width="50" fixed></el-table-column>
            <el-table-column prop="teacherCode" label="教师编号" align='left' width="120" sortable fixed>
            </el-table-column>
            <el-table-column prop="teacherName" label="教师姓名" align='left' width="100" fixed>
            </el-table-column>
            <el-table-column prop="teacherSex" label="性别" align='center' width="80">
            </el-table-column>
            <el-table-column prop="teacherPhone" label="联系电话" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="teacherAddress" label="家庭地址" align='left' min-width="180">
            </el-table-column>
            <el-table-column prop="teacherStatus" label="状态" :filters="[{text: '是', value: '是'},{text: '否', value: '否'}]" :filter-method="filterPackageEnable" align='left' width="100">
                <!-- <template slot-scope="scope">
                    <el-tag :type="courseTag(scope.row.packageEnabled)" :disable-transitions="false">
                        {{scope.row.packageEnabled}}
                    </el-tag>
                </template> -->
            </el-table-column>
            <el-table-column prop="teacherRegisterDate" label="入职日期" align='left' width="100">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" min-width="100">
                <template slot-scope='scope'>
                    <el-button type="warning" icon='el-icon-edit' size="mini" @click='showEditPackagePanel(scope.row)'>编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <!-- <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="paginations.page_size" :layout="paginations.layout" :total="paginations.total" :current-page="paginations.current_page_index" @current-change='handleCurrentChange' @size-change='handleSizeChange'>
                    </el-pagination>
                </div>
            </el-col>
        </el-row> -->
    </div>
    
    <el-dialog :title="teacherialog.title" :width="teacherialog.width" :visible.sync="teacherialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="baseInfo" :model="teacherialog.baseInfo" :rules="teacherialog.baseInfoRules" :label-width="teacherialog.formLabelWidth" :label-position='teacherialog.labelPosition' size="mini">
                <el-form-item prop="teacherName" label="姓名">
                    <el-input v-model="teacherialog.baseInfo.teacherName"></el-input>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="teacherialog.baseInfo.teacherSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="teacherialog.baseInfo.teacherBirthday" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item prop="teacherIdentityCardNum" label="身份证号码">
                    <el-input v-model="teacherialog.baseInfo.teacherIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item prop="teacherPhone" label="联系电话">
                    <el-input v-model="teacherialog.baseInfo.teacherPhone"></el-input>
                </el-form-item>
                <el-form-item prop="teacherAddress" label="家庭地址">
                    <el-input v-model="teacherialog.baseInfo.teacherAddress"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="teacherialog.baseInfo.teacherRemark"></el-input>
                </el-form-item>
                <el-form-item label="入职日期">
                    <el-date-picker v-model="teacherialog.baseInfo.teacherRegisterDate" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="teacherialog.isShow = false">取 消</el-button>
                    <el-button size="small" type="primary" @click="submitNewTeacher('baseInfo')">提 交</el-button>
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
            lookup: {
                courseCategory: []
            },
            teacherList: [],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 63,
            paginations: {
                current_page_index: 1,
                total: 4,
                page_size: 15,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
            searchField: {
                package_name: ''
            },
            teacherialog: {
                width: '600px',
                isShow: false,
                title: '添加教师基本信息',
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
                        message: '请输入学生姓名',
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

        var _this = this;
        // 获取课程大类
        axios({
            type: 'get',
            path: '/api/config/getdicbycode',
            data: {
                typeCode: 'course_category'
            },
            fn: function (result) {
                _this.lookup.courseCategory = result;
            }
        });
    },
    methods: {
        getTeacherList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/teacher',
                fn: function (result) {
                    result.forEach(item => {
                        item.teacherRegisterDate = item.teacherRegisterDate.split('T')[0];
                    })
                    _this.teacherList = result;
                }
            })
        },

        showAddTeacherPanel() {
            this.teacherialog.title = '添加教师基本信息';
            this.teacherialog.packageDetail = {
                packageName: '',
                packageCourseCategoryCode: '',
                packageCourseCategoryName: '',
                packageCourseCount: '',
                packagePrice: '',
                packageEnabled: '是',
            }
            this.teacherialog.isUpdate = false;
            this.teacherialog.isShow = true;
        },

        submitNewTeacher(teacherForm) {
            var _this = this;
            this.$refs[teacherForm].validate((valid) => {
                if (valid) {
                    axios({
                        type: 'post',
                        path: '/api/teacher',
                        data: _this.teacherDialog.baseInfo,
                        fn: function (result) {
                            if (result == 200) {
                                _this.getTeacherList();
                                _this.teacherialog.isShow = false;
                            }
                        }
                    })

                }
            });
            return;
        },

        showEditPackagePanel(row) {
            var _this = this;
            // 判断是否可以编辑
            axios({
                type: 'get',
                path: '/api/coursepackage/' + row.id,
                fn: function (result) {
                    if (result) {
                        _this.dialog.isPackageUsed = true;
                    } 
                    else{
                        _this.dialog.isPackageUsed = false;
                    }
                }
            })

            this.dialog.title = '更新课程套餐';
            this.dialog.packageDetail = {
                packageName: row.packageName,
                packageCourseCategoryCode: row.packageCourseCategoryCode,
                packageCourseCategoryName: row.packageCourseCategoryName,
                packageCourseCount: row.packageCourseCount,
                packagePrice: row.packagePrice,
                packageEnabled: row.packageEnabled,
            }
            this.dialog.currentId = row.id;
            this.dialog.isUpdate = true;
            this.dialog.isShow = true;
        },

        submitEditPackage(teacherForm) {
            var _this = this;
            this.$refs[teacherForm].validate((valid) => {
                if (valid) {
                    var code = _this.dialog.packageDetail.packageCourseCategoryCode,
                        categoryName = _this.GetLabelByValue(_this.lookup.courseCategory, code);
                    _this.dialog.packageDetail.packageCourseCategoryName = categoryName;

                    axios({
                        type: 'put',
                        path: '/api/coursepackage/' + _this.dialog.currentId,
                        data: _this.dialog.packageDetail,
                        fn: function (result) {
                            if (result == 200) {
                                _this.$message({
                                    type: 'success',
                                    message: '修改成功！'
                                });
                                _this.getPackageList();
                                _this.dialog.isShow = false;
                            }
                        }
                    });
                }
            });
        },

        submitRemovePackage() {
            var _this = this;
            axios({
                type: 'delete',
                path: '/api/coursepackage/' + _this.dialog.currentId,
                fn: function (result) {
                    if (result == 200) {
                        _this.$message({
                            type: 'success',
                            message: '删除成功！'
                        });
                        _this.getPackageList();
                        _this.dialog.isShow = false;
                    }
                }
            })
        },

        filterPackageEnable(value, row, column){
            return row['packageEnabled'] === value;
        },

        courseTag(course) {
            let basic = '';
            switch (course) {
                case '是':
                    basic = 'success'
                    break;
                case '否':
                    basic = 'info'
                    break;
            }
            return basic;
        },
        handleSizeChange(val) {
            console.log(`每页 ${val} 条`);
        },
        handleCurrentChange(val) {
            console.log(`当前页: ${val}`);
        },
        GetLabelByValue(lst, value) {
            let label = '';
            for (let obj of lst) {
                if (obj['value'] == value) {
                    label = obj['label'];
                    break;
                }
            }
            return label;
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
