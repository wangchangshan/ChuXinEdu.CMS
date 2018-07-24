<template>
    <div class="home">
        <head-nav></head-nav>
        <div class="left-fixed-right-auto">
            <left-menu></left-menu>
            <div class="content_page">
                <div class="content">
                    <bread></bread>
                    <!--页面渲染入口-->
                    <router-view></router-view>
                </div>
            </div>
        </div>
    </div>
</template>
