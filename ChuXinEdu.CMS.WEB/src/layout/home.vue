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
            <transition name="fade-transform" mode="out-in">
                <router-view></router-view>
            </transition>
        </el-main>
    </el-container>
</el-container>
</template>

<script>
import HeadNav from './headNav.vue'
import LeftMenu from './leftMenu.vue'
import {
    axios
} from '@/utils/index'

export default {
    name: "home",
    data() {
        return {
            winSize: {
                height: '',
                width: ''
            }
        }
    },
    components: {
        HeadNav,
        LeftMenu
    },
    created() {
        this.setSize();
        this.getBirthdayNotify();
        this.getToRecordNotify();
        this.getToFinishNotify();
        this.getTempStudentStatus();
    },
    mounted() {
        window.onresize = () => {
            this.setSize();
        }
    },
    methods: {
        setSize() {
            this.winSize = {
                height: document.body.clientHeight - 63,
                width: document.body.clientWidth - 183
            }
            this.$store.dispatch('set_win_content', this.winSize); //触发动作，content部分的宽高也随即改变。
        },
        getBirthdayNotify() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getbirthdaynotify',
                fn: function (result) {
                    if(result){
                        _this.$store.commit('SET_BIRTHDAY_LIST', result);                    
                    }
                }
            })
        },
        getToRecordNotify() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/course/gettorecordcount',
                fn: function (result) {
                    _this.$store.commit('SET_TO_RECORD', result);  
                }
            })
        },
        getToFinishNotify() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/course/gettofinish',
                fn: function (result) {
                    if(result){
                        _this.$store.commit('SET_TO_FINISH_LIST', result); 
                    } 
                }
            })
        },
        getTempStudentStatus(){
            var _this = this;
            axios({
                type: 'get',
                path: '/api/config/getdicbycode',
                data: {typeCode : 'student_temp_status'},
                fn: function (result) {
                    if(result){
                        _this.$store.commit('SET_S_T_STATUS', result); 
                    } 
                }
            })
        },
    }

}
</script>

<style lang="less" scoped>
.content_page {
    position: absolute;
    top: 70px;
    background: #FFF;
    overflow: auto;
}

.content {
    width: 100%;
    height: 100%;
}
</style>
