<template>
<div class="list_container">
    <div class="table_container">
        <el-table :data="templateList" v-loading="loading" size="mini" align="left" border stripe :height="tableHeight">
            <el-table-column type="index" align='center' width="40"></el-table-column>
            <el-table-column prop="arrangeTemplateName" label="排课模板名称" align='center' min-width="220">
            </el-table-column>
            <el-table-column prop="arrangeTemplateCode" label="模板代码" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="primary" icon='edit' size="small" @click='templateManage(scope.row)'>管 理</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <el-dialog :title="templateDialog.title" :visible.sync="templateDialog.isShow" :width="templateDialog.width" :close-on-press-escape='false' :modal-append-to-body="false">
        
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
            templateList: [],
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 150,

            curRoleCode: '',
            templateDialog: {
                width: '700px',
                isShow: false,
                title: '',
            }
        }
    },
    created() {
        this.getTemplateList();
    },
    methods: {
        getTemplateList() {
            this.loading = true;
            axios({
                type: 'get',
                path: '/api/config/getarrangetemplates',
                fn: result => {
                    this.templateList = result;
                    this.loading = false;
                }
            });
        },

        templateManage(row) {
            alert("待开发，敬请期待")
        }
    }
}
</script>

<style lang="less" scoped>
.list_container {
    overflow-y: hidden;
}
</style>
