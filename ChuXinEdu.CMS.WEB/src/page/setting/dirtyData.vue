<template>
<div class="list_container">
    <div class="table_container">
        <el-table :data="dirtyList" v-loading="loading" size="mini" align="left" border stripe :height="tableHeight">
            <el-table-column prop="tablename" label="数据表" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="id" label="ID" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="packagecode" label="套餐编号" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="studentcode" label="学号" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="studentname" label="姓名" align='center' min-width="120">
            </el-table-column>
            <el-table-column prop="restcoursecount" label="剩余可上课时" align='center' min-width="100">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="success" icon='edit' size="small" @click='clearDirty(scope.row)'>修 正</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    data() {
        return {
            dirtyList: [],
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 150,
        }
    },
    created() {
        this.loading = true;
        this.getDirtyList();
    },
    methods: {
        getDirtyList() {
            axios({
                type: 'get',
                path: '/api/config/getdirty',
                fn: result => {
                    this.dirtyList = result;
                    this.loading = false;
                }
            });
        },

        clearDirty(row) {
            this.loading = true;
            var dirtyData = { 
                'tablename': row.tablename,
                'id': row.id 
            }
            axios({
                type: 'post',
                path: '/api/config/cleardirty',
                data: dirtyData,
                fn: result => {
                    if(result == 1200){
                        this.$message({
                            type: "success",
                            message: "修复成功！"
                        });
                        this.getDirtyList();
                    }
                    else{
                        this.$message({
                            type: "error",
                            message: "修复程序出错了！"
                        });
                        this.loading = false;
                    }
                }
            });
        }
    }
}
</script>

<style lang="less" scoped>
.list_container {
    overflow-y: hidden;
}
</style>
