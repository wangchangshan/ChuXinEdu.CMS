<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" :rules="search_form_rules" ref="searchField" class="demo-form-inline search-form">
            <el-form-item prop='package_name' label="套餐名称">
                <el-input type="text" v-model="searchField.package_name" placeholder="请输入名称"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" @click='searchPackage("searchField")'>筛选</el-button>
            </el-form-item>

            <el-form-item class="btnRight">
                <el-button type="primary" icon="el-icon-plus" @click='addPackage()'>添加</el-button>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="coursePackage" v-loading="loading" style="width: 100%" align="center" :max-height="tableHeight">
            <el-table-column prop="package_id" label="序号" align='center' width="100" sortable fixed>
            </el-table-column>
            <el-table-column prop="package_name" label="课程套餐名称" align='center' min-width="220" fixed>
            </el-table-column>
            <el-table-column prop="package_course_type" label="课程类别" align='center' width="80">
            </el-table-column>
            <el-table-column prop="package_course_count" label="课时数" align='center' width="80">
            </el-table-column>
            <el-table-column prop="package_price" label="课程价格" align='center' width="100">
                <template slot-scope="scope">
                    <span style="color:#00d053">{{ scope.row.package_price }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="package_is_active" label="是否启用" align='left' sortable width="100">
                <template slot-scope="scope">
                    <el-tag :type="courseTag(scope.row.package_is_active)" :disable-transitions="false">
                        {{scope.row.package_is_active}}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="package_create_date" label="创建日期" align='center' width="100">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" min-width="100">
                <template slot-scope='scope'>
                    <el-button type="warning" icon='edit' size="small" @click='editPackage(scope.row)'>编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="paginations.page_size" :layout="paginations.layout" :total="paginations.total" :current-page="paginations.current_page_index" @current-change='handleCurrentChange'
                        @size-change='handleSizeChange'>
                    </el-pagination>
                </div>
            </el-col>
        </el-row>
    </div>
    <el-dialog :title="dialog.title" :visible.sync="dialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="packageDetail" :model="packageDetail" :rules="packageDetailRules" :label-width="dialog.formLabelWidth" :label-position='dialog.labelPosition' label-suffix='：' style="margin:10px;width:auto;">
                <el-form-item label="课程套餐名称">
                    <el-input v-model="packageDetail.package_name"></el-input>
                </el-form-item>
                <el-form-item label="课程类别">
                    <template>
                        <el-select v-model="packageDetail.package_course_type" placeholder="请选择">
                            <el-option v-for="item in lookup.course_type" :key="item.code" :label="item.name" :value="item.code">
                            </el-option>
                        </el-select>
                    </template>
                </el-form-item>
                <el-form-item label="课时数">
                    <el-input v-model="packageDetail.package_course_count"></el-input>
                </el-form-item>
                <el-form-item label="总价格">
                    <el-input v-model="packageDetail.package_price"></el-input>
                </el-form-item>
                <el-form-item label="状态">
                    <el-switch v-model="packageDetail.package_is_active" active-color="#13ce66" inactive-color="#dcdfe6" active-text="启用" inactive-text="不启用">
                    </el-switch>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="dialog.show = false">取 消</el-button>
                    <el-button type="primary">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
export default {
    data() {
        return {
            lookup:{
                course_type:[{
                    code: 'meishu',
                    name: '美术'
                },{
                    code: 'shufa',
                    name: '书法'
                }]
            },
            coursePackage: [{
                    package_id: '1',
                    package_name: '美术40节6000元',
                    package_course_type: '美术',
                    package_course_count: '40',
                    package_price: '6000',
                    package_is_active: '是',
                    package_create_date: '2018-08-08'
                },
                {
                    package_id: '2',
                    package_name: '美术80节8800元送2节',
                    package_course_type: '美术',
                    package_course_count: '42',
                    package_price: '8800',
                    package_is_active: '是',
                    package_create_date: '2018-08-08'
                },
                {
                    package_id: '3',
                    package_name: '书法60节6800元',
                    package_course_type: '书法',
                    package_course_count: '60',
                    package_price: '6800',
                    package_is_active: '否',
                    package_create_date: '2018-08-08'
                }, {
                    package_id: '4',
                    package_name: '书法80节8800元',
                    package_course_type: '书法',
                    package_course_count: '80',
                    package_price: '8800',
                    package_is_active: '是',
                    package_create_date: '2018-08-08'
                },
            ],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 63,
            search_form_rules: {
                package_name: [{
                    required: false,
                    message: '名称不能为空',
                    trigger: 'blur'
                }]
            },
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
                width: '500px',
                show: false,
                title: '添加套餐',
                labelPosition: 'right',
                formLabelWidth: '120px'
            },
            packageDetail: {
                package_name: '',
            },
            packageDetailRules: {

            }
        }
    },
    methods: {
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
        addPackage() {
            this.dialog.title = '新增课程套餐';
            this.dialog.show = true;
        },
        editPackage() {
            alert('待开发')
        },
        searchPackage() {
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
