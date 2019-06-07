<template>
<div class="fillcontain">
    <el-tabs type="card" v-model="activeLabelCode">
        <el-tab-pane :name="'env'">
            <span slot="label"><i class="fa fa-picture"></i>机构环境</span> 
            <pic-manager v-if="activeLabelCode === 'env'" v-bind:wx_pic_code="'01'"></pic-manager>
        </el-tab-pane>

        <el-tab-pane :name="'study'">
            <span slot="label"><i class="fa fa-picture"></i>学习日常</span> 
            <pic-manager v-if="activeLabelCode === 'study'" v-bind:wx_pic_code="'00'"></pic-manager>
        </el-tab-pane>

        <el-tab-pane :name="'fine'">
            <span slot="label"><i class="fa fa-picture"></i>优秀作品</span> 
            <pic-manager v-if="activeLabelCode === 'fine'" v-bind:wx_pic_code="'02'"></pic-manager>
        </el-tab-pane>

        <el-tab-pane :name="'teacher'">
            <span slot="label"><i class="fa fa-picture"></i>教师图片</span> 
            <pic-manager v-if="activeLabelCode === 'teacher'" v-bind:wx_pic_code="'03'"></pic-manager>
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import picManager from './picManager'

export default {
    data() {
        return { 
            activeLabelCode: 'env',
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
