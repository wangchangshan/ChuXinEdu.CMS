<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode">
        <el-tab-pane :name="'dictionary'">
            <span slot="label"><i class="fa fa-book"></i> 字典配置</span> 
            <dictionary-setting v-if="activeLabelCode === 'dictionary'"></dictionary-setting>
        </el-tab-pane>

        <el-tab-pane :name="'role'">
            <span slot="label"><i class="fa fa-tags"></i> 角色权限配置</span>
            <role-setting v-if="activeLabelCode === 'role'"></role-setting>
        </el-tab-pane>

        <el-tab-pane :name="'arrange-template'">
            <span slot="label"><i class="fa fa-table"></i> 排课模板配置</span> 
            <arrange-template-setting v-if="activeLabelCode === 'arrange-template'"></arrange-template-setting>
        </el-tab-pane>

        <el-tab-pane :name="'dirty'">
            <span slot="label"><i class="fa fa-database"></i> 数据修复</span> 
            <dirty-data v-if="activeLabelCode === 'dirty'"></dirty-data>
        </el-tab-pane>

    </el-tabs>
</div>
</template>

<script>
import roleSetting from './role'
import arrangeTemplateSetting from './arrangeTemplate'
import dictionarySetting from './dictionary'
import dirtyData from './dirtyData'

export default {
    data() {
        return { 
            activeLabelCode: 'dictionary',
            teacherCode: ''
        }
    },
    created() {    
    },
    components: {
        "role-setting" : roleSetting,
        "arrange-template-setting" : arrangeTemplateSetting,
        "dictionary-setting": dictionarySetting,
        "dirty-data": dirtyData
    }
}
</script>

<style lang="less" scoped>

</style>
