<template>
<el-menu mode="vertical" :collapse="$store.state.menu.isCollapse" background-color="#324057" text-color="#fff" 
    active-text-color="#42b983" 
    :default-active="$store.state.menu.activeMenu" 
    :style="{height: $store.state.page.win_content.height + 63 + 'px'}">
    <template v-for="(item) in $store.state.menu.menuItems" v-if="item.hidden !== true">
        <!-- 拥有二级菜单 -->
        <el-submenu v-if="item.children && !item.noDropdpwn && item.children.length > 0" :index="item.path" :key="item.key" class="dropItem">
            <template slot="title">
                <i :class="'fa fa-margin ' + item.icon"></i>
                <span slot="title">{{item.name}}</span>
            </template>
            <router-link v-for="(citem) in item.children" :to="item.path + '/' + citem.path" :key="citem.path">
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
</template>

<script>
export default {
    name: "left-menu",
    data() {
        return {
        }
    },
    methods: {
        showDropdown() {

        }
    }
}
</script>

<style lang="less" scoped>
.fa-margin {
    margin-left: -10px;
    margin-right: 10px;
    font-size: 16px;
}

.el-menu--collapse{
    width: 40px;
}

.hiddenDropdown,
.hiddenDropname {
    display: none;
}
</style>
