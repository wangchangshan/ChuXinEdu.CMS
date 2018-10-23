<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" class="demo-form-inline search-form">
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" icon="el-icon-plus" @click='showAddPackagePanel()'>添加</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="packageList" v-loading="loading" style="width: 100%" align="left" border stripe size="mini" :max-height="tableHeight">
            <el-table-column type="index" width="50" fixed></el-table-column>
            <el-table-column prop="packageCode" label="套餐编码" align='left' width="120" sortable fixed>
            </el-table-column>
            <el-table-column prop="packageName" label="课程套餐名称" align='left' min-width="200" fixed>
            </el-table-column>
            <el-table-column prop="packageCourseCategoryName" label="课程类别" sortable align='left' min-width="100">
            </el-table-column>
            <el-table-column prop="packageCourseCount" label="课时数" align='center' width="80">
            </el-table-column>
            <el-table-column prop="packagePrice" label="课程价格" align='left' min-width="100">
                <template slot-scope="scope">
                    <span style="color:#f56767">￥{{ scope.row.packagePrice }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="packageEnabled" label="是否启用" :filters="[{text: '是', value: '是'},{text: '否', value: '否'}]" :filter-method="filterPackageEnable" align='left' width="100">
                <template slot-scope="scope">
                    <el-tag :type="courseTag(scope.row.packageEnabled)" :disable-transitions="false">
                        {{scope.row.packageEnabled}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="packageCreateTime" label="创建日期" align='left' width="100">
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
    <el-dialog :title="dialog.title" :visible.sync="dialog.isShow" :width="dialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="packageDetail" :model="dialog.packageDetail" :rules="dialog.packageDetailRules" size="mini" :label-width="dialog.formLabelWidth" :label-position='dialog.labelPosition' label-suffix='：' style="margin:10px;width:auto;">
                <el-form-item prop="packageName" label="课程套餐名称">
                    <el-input :disabled="dialog.isPackageUsed" v-model="dialog.packageDetail.packageName"></el-input>
                </el-form-item>
                <el-form-item prop="packageCourseCategoryCode" label="课程类别">
                    <template>
                        <el-select :disabled="dialog.isPackageUsed" v-model="dialog.packageDetail.packageCourseCategoryCode" placeholder="请选择">
                            <el-option v-for="item in lookup.courseCategory" :key="item.value" :label="item.label" :value="item.value">
                            </el-option>
                        </el-select>
                    </template>
                </el-form-item>
                <el-form-item prop="packageCourseCount" label="课时数">
                    <el-input-number :disabled="dialog.isPackageUsed" v-model="dialog.packageDetail.packageCourseCount" :min="0"></el-input-number>
                </el-form-item>
                <el-form-item prop="packagePrice" label="总价格">
                    <el-input-number :disabled="dialog.isPackageUsed" v-model="dialog.packageDetail.packagePrice" :min="0"></el-input-number>
                </el-form-item>
                <el-form-item prop="packageEnabled" label="状态">
                    <el-switch v-model="dialog.packageDetail.packageEnabled" active-color="#13ce66" inactive-color="#dcdfe6" active-text="启用" active-value="是" inactive-text="不启用" inactive-value="否"> </el-switch>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="dialog.isShow = false">取 消</el-button>
                    <el-button v-show="!dialog.isUpdate" type="primary" @click="submitNewPackage('packageDetail')">提 交</el-button>
                    <el-button v-show="dialog.isUpdate && !dialog.isPackageUsed" type="danger" @click='submitRemovePackage()'>删 除</el-button>
                    <el-button v-show="dialog.isUpdate" type="primary" @click="submitEditPackage('packageDetail')">保 存</el-button>
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
            packageList: [],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 73,
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
            dialog: {
                width: '600px',
                isShow: false,
                title: '添加套餐',
                labelPosition: 'right',
                formLabelWidth: '130px',
                isUpdate: false,
                isPackageUsed: false,
                currentId: '',
                packageDetail: {
                    packageName: '',
                    packageCourseCategoryCode: '',
                    packageCourseCategoryName: '',
                    packageCourseCount: '',
                    packagePrice: '',
                    packageEnabled: '是',
                },
                packageDetailRules: {
                    packageName: [{
                        required: true,
                        message: '请输入套餐名称',
                        trigger: 'blur'
                    }],
                    packageCourseCategoryCode: [{
                        required: true,
                        message: '请选择套餐类别',
                        trigger: 'change'
                    }],
                    packageCourseCount: [{
                        required: true,
                        type: 'number',
                        min: 1,
                        message: '套餐课时数至少为1',
                        trigger: 'change'
                    }],
                }
            },
        }
    },
    created() {
        this.getPackageList();

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
        getPackageList() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/coursepackage',
                fn: function (result) {
                    result.forEach(item => {
                        item.packageCreateTime = item.packageCreateTime.split('T')[0];
                    })
                    _this.packageList = result;
                }
            })
        },

        showAddPackagePanel() {
            this.dialog.title = '新增课程套餐';
            this.dialog.packageDetail = {
                packageName: '',
                packageCourseCategoryCode: '',
                packageCourseCategoryName: '',
                packageCourseCount: '',
                packagePrice: '',
                packageEnabled: '是',
            }
            this.dialog.isUpdate = false;
            this.dialog.isPackageUsed = false;
            this.dialog.isShow = true;
        },

        submitNewPackage(packageForm) {
            var _this = this;
            this.$refs[packageForm].validate((valid) => {
                if (valid) {
                    var code = _this.dialog.packageDetail.packageCourseCategoryCode,
                        categoryName = _this.GetLabelByValue(_this.lookup.courseCategory, code);
                    _this.dialog.packageDetail.packageCourseCategoryName = categoryName;

                    axios({
                        type: 'post',
                        path: '/api/coursepackage',
                        data: _this.dialog.packageDetail,
                        fn: function (result) {
                            if (result == 200) {
                                _this.getPackageList();
                                _this.dialog.isShow = false;
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

        submitEditPackage(packageForm) {
            var _this = this;
            this.$refs[packageForm].validate((valid) => {
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
