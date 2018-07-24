<template>
    <div class="bread_container" id="bread_container">
        <span class="bars">
            <i class="fa fa-bars" :class="{isactive:changeBarDirection}"></i>
        </span>
        <el-breadcrumb class="breadcrumb" separator="/">
            <el-breadcrumb-item :to="{ path: '/index' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item></el-breadcrumb-item>
        </el-breadcrumb>
    </div>
</template>

<script>
export default {
    data(){
        return {
            changeBarDirection: false,
            levelList: null
        }
    },
    methods: {
        
    }
}
</script>


<style lang="less">
    .bread_container{
        background-color: #eff2f7;
        margin-bottom: 5px;
        width: 100%;
        .bars{
            float: left;
            margin: 4px 10px;
            cursor: pointer;
            .isactive{
                -webkit-transform: rotate(90deg);
                transform: rotate(90deg);
                transition: .38s;
                -webkit-transform-origin: 50% 50%;
                transform-origin: 50% 50%;
            }
        }
        .breadcrumb{
            height: 30px;
            line-height: 30px;
            .breadbutton{
                float: left;
                margin: 4px 5px 0 0;
            }
        }
    }
</style>