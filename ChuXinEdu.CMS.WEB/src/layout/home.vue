<template>
<el-container>
    <el-aside :style="{width: $store.state.menu.sidebar.width + 'px','overflow-x':'hidden'}">
        <left-menu></left-menu>
    </el-aside>
    <el-container>
        <el-header style="padding:0">
            <head-nav></head-nav>
        </el-header>
        <el-main style="padding:10px 0 0 0;overflow-x:hidden" :style="{height: $store.state.page.win_content.height -63 + 'px'}">
            <transition name="fade-transform" mode="out-in" >
                <router-view ></router-view>
            </transition>
        </el-main>
    </el-container>
</el-container>
</template>

<script>
    import HeadNav from './headNav.vue'
    import LeftMenu from './leftMenu.vue'

    export default {
        name: "home",
        data(){
            return {
                win_size: {
                    height: '',
                    width: ''
                }
            }
        },
        components: {
            HeadNav,
            LeftMenu
        },
        methods: {
            //用于自适配窗口页面大小
            setSize() {
                //lib_$-->$,window的宽,高的获取是没有问题的。
                this.win_size = {
                    height:document.body.clientHeight - 63,
                    width:document.body.clientWidth - 183
                }
                //console.log('重新计算页面宽高');
                //将content部分的宽高，存入store中，
                this.$store.dispatch('set_win_content',this.win_size); //触发动作，content部分的宽高也随即改变。
            },
        },
        created(){
            this.setSize();
        },
        mounted() {
            window.onresize = () => {
                this.setSize();
            }
        },

    }
</script>


<style scoped lang='less'>
    .content_page{
        position: absolute;
        top:70px;
        background: #FFF;
        overflow:auto;
    }
     .content{
       width:100%;
       height:100%;
    }
</style>