<template>
<div class="fillcontain">
    <div class="search_container">
        <el-form :inline="true" class="demo-form-inline search-form">
            <el-form-item label="名称：">
                <el-input type="text" size="small" v-model="searchField.activitySubject" placeholder="请输入活动主题" class="search_field"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchActivity()'>查 询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetActivityList()'>重 置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <router-link :to="'/newActivity'">
                    <el-button type="primary" size="small" icon="el-icon-plus">创建新活动</el-button>
                </router-link>
            </el-form-item>
        </el-form>
    </div>
    <div class="table_container">
        <el-table :data="activityList" v-loading="loading" style="width: 100%" align="left" border stripe size="mini" :height="tableHeight">
            <el-table-column type="expand">
                <template slot-scope="scope">
                    <div v-html="scope.row.activityContent"></div>
                </template>
            </el-table-column>
            <el-table-column prop="activitySubject" label="活动主题" align='left' min-width="230">
            </el-table-column>
            <el-table-column prop="activityDate" label="活动时间" align='center' width="175">
            </el-table-column>
            <el-table-column prop="activityCourseCount" label="课时数" align='center' width="70">
            </el-table-column>
            <el-table-column prop="activityAddress" label="地址" align='left' min-width="120">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="100">
                <template slot-scope="scope">
                    <router-link :to="{ name: '编辑活动', params: { activityId: scope.row.activityId }}">
                        <el-button type="warning" size="small" icon="el-icon-edit">编辑</el-button>
                    </router-link>
                </template>
            </el-table-column>
        </el-table>
        <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="searchField.pageSize" :layout="paginations.layout" :total="paginations.total" :current-page="searchField.pageIndex" @current-change='handlePageCurrentChange' @size-change='handlePageSizeChange'>

                    </el-pagination>
                </div>
            </el-col>
        </el-row>
    </div>
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
            activityList: [],
            tableHeight: this.$store.state.page.win_content.height - 106,
            searchField: {
                pageIndex: 1,
                pageSize: 15,
                activitySubject: ''
            },
            paginations: {
                total: 0,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
        }
    },
    created() {
        this.getActivityList();
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

        searchActivity() {
            var page = 1;
            this.searchField.pageIndex = 1;
            this.getActivityList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },

        resetActivityList() {
            this.searchField.activitySubject = '';
            this.searchField.pageIndex = 1;
            this.getActivityList();
        },

        getActivityList({
            page,
            pageSize,
            where,
            fun
        } = {}) {
            this.loading = true;
            var query = this.$route.query;
            this.searchField.pageIndex = page || parseInt(query.page) || 1;
            this.searchField.pageSize = pageSize || parseInt(query.page_size) || this.searchField.pageSize;
            var data = {
                q: this.searchField
            }
            if (where) {
                data = Object.assign(data, where || {});
            }

            axios({
                type: 'get',
                path: '/api/activity/getlist',
                data: data,
                fn: result => {
                    this.paginations.total = result.totalCount;
                    result.data.forEach(item => {
                        if (item.activityFromDate == item.activityToDate) {
                            item.activityDate = item.activityFromDate
                        } else {
                            item.activityDate = item.activityFromDate + '至' + item.activityToDate
                        }
                    });
                    this.activityList = result.data;
                    this.loading = false;
                }
            })
        },

        handlePageSizeChange(pageSize) {
            this.getActivityList({
                pageSize,
                fun: () => {
                    this.setPath('page_size', pageSize);
                }
            });
        },
        handlePageCurrentChange(page) {
            this.getActivityList({
                page,
                fun: () => {
                    this.setPath('page', page);
                }
            });
        },
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
