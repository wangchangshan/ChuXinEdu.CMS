<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode">
        <el-tab-pane :name="'picture'">
            <span slot="label"><i class="fa fa-picture"></i>图片管理</span> 
            <pic-manager v-if="activeLabelCode === 'picture'"></pic-manager>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import picManager from './picManager'

export default {
    data() {
        return { 
            activeLabelCode: 'picture',
        }
    },
    created() {    
    },
    components: {
        "pic-manager" : picManager
    }
}
</script>

<style lang="less" scoped>

</style>
