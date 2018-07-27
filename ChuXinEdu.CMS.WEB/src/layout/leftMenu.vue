<template>
    <el-row class="menu_page">
        <el-col>
            <el-menu mode="vertical" class="el-menu-vertical-demo" 
                background-color="#324057" text-color="#fff" active-text-color="#42b983">
                <template v-for="(item) in $store.state.menu.menuItems" v-if="item.hidden !== true">
                        <!-- 拥有二级菜单 -->
                        <el-submenu v-if="item.children && !item.noDropdpwn && item.children.length > 0" :index="item.path" :key="item.path" class="dropItem"> 
                                <template slot="title">
                                    <i :class="'fa fa-margin ' + item.icon"></i>
                                    <span slot="title">{{item.name}}</span>
                                </template>
                            <router-link v-for="(citem) in item.children" :to="citem.path" :key="citem.path">
                                <el-menu-item :index="citem.path">
                                    <span slot="title"> {{citem.name}} </span>
                                </el-menu-item>
                            </router-link>
                        </el-submenu>

                    <!-- 没有二级菜单 -->
                    <router-link v-bind:to="item.path" :key="item.path">
                        <el-menu-item v-if="item.noDropdown" :index="item.path">
                                <template solt="title">
                                    <i :class="'fa fa-margin ' + item.icon" @mouseover="showDropdown"></i>
                                    <span :class="{'hiddenDropname':$store.state.menu.menuItems.isHidMenuName}" slot="title">{{item.name}}</span>
                                </template>

                            <!-- <router-link :to="item.path">
                                <el-menu-item :index="item.children[0].path" :class="{'hiddenDropdown':!$store.state.menu.isDropname}">
                                     <span slot="title">{{item.children[0].name}}</span>
                                </el-menu-item>
                            </router-link> -->
                        </el-menu-item>
                    </router-link>
                </template>
            </el-menu>
        </el-col>
    </el-row>
</template>

<script>
    export default {
        name: "left-menu",
        data(){
            return {
                isDropdown: false
            }
        },
        methods: {
            showDropdown(){

            }
        }
    }
</script>

<style lang="less" scoped>
	.menu_page{
		position: fixed;
		top:71px;
        left:0;
        min-height: 100%; 
        background-color: #324057;
        z-index: 99;
	}
    .fa-margin{
        margin-right:5px;
    }
    .el-menu-vertical-demo:not(.el-menu--collapse) {
        width: 180px;
        min-height: 400px;
    }
    .el-menu-vertical-demo{
        width:35px;
    }
    .el-submenu .el-menu-item{
        min-width:180px;
    }
    .el-menu{
        .el-menu-item{
            padding-left:40px !important;
        }
     }
 
    .hiddenDropdown,
    .hiddenDropname{
        display:none;
    }
</style>
