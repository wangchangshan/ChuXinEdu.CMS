// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
//import vueRouter from 'vue-router'
import store from './store/index'
import ElementUI from 'element-ui'
import {LocalDB, menuHelper} from './utils/index'
import Home from '@/layout/home'
import 'element-ui/lib/theme-chalk/index.css'
import 'font-awesome/css/font-awesome.css'

Vue.use(ElementUI)

/**
 * 如果用户刷新页面,导致存入vuex中的菜单数据清空,需要从缓存获取;
 */
var strMenuData = LocalDB.instance('MENU_').getValue('leftMenu').value;
var menuData = '';
if(strMenuData) {
    menuData = JSON.parse(strMenuData)
} 
if(menuData) {
    store.commit('ADD_MENU', menuData);  // ?? commit or dispatch ,将缓存数据注入到store中
    const routes = menuHelper.generateRoutesFromMenu(menuData)  //根据菜单生成的路由信息
    const asyncRouterMap = [
        {
            path        :   '/index',
            name        :   '',
            hidden      :   true,
            component   :   Home,
            redirect    :   '/index',
            children    :   routes
        }
    ];
    router.addRoutes(asyncRouterMap);
}

router.beforeEach((to, from, next) => {
    // 定位到首页时， 清空缓存数据
    if(to.path === '/') {
        LocalDB.instance('USER_').remove('userinfo');
        LocalDB.instance('MENU_').remove('leftMenu');
        store.commit('ADD_MENU', []);
    }

    // 判断是否有用户登录的记录
    let userinfo = JSON.parse(LocalDB.instance('USER_').getValue('userInfo').value);
    // 没有用户信息，route.path不是定位到登录页面的,直接跳登录页面。
    if(!userinfo && to.path !== '/') {
        next({ path: '/' });
    } else {
        // 有用户信息和路由名称的，直接跳要路由的页面。
        if(to.name) {
            next();
        } else {
           // next({ path: '/404' })
        }
    }
});

Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
    el: '#app',
    router,
    store,
    components: { App },
    template: '<App/>'
})
