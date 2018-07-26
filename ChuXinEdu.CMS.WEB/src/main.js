// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
//import vueRouter from 'vue-router'
import store from './store/index'
import ElementUI from 'element-ui'
import {LocalDB, menuHelper} from './utils/index'
import 'element-ui/lib/theme-chalk/index.css'
import 'font-awesome/css/font-awesome.css'

Vue.use(ElementUI)
//debugger
var strUserInfo = LocalDB.instance('USER_').getValue('menuData').value;
var menuData = '';
if(strUserInfo) {
    menuData = JSON.parse(strUserInfo)
} 
if(menuData) {
    store.commit('ADD_MENU', menuData);  // ?? commit or dispatch ,将缓存数据注入到store中
    const routes = menuHelper.generateRoutesFromMenu(menuData)  //根据菜单生成的路由信息

    const asyncRouterMap = [
        {
            path: '/index',
            name:'',
            hidden   : true,
            component: require('@/layout/home.vue'),
            redirect: '/index',
            children:routes
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
    let userinfo = JSON.parse(LocalDB.instance('USER_').getValue('userinfo').value);
    // 没有用户信息，route.path不是定位到登录页面的,直接跳登录页面。
    if(!userinfo && to.path !== '/') {
        next({ path: '/' });
    } else {
        // 有用户信息和路由名称的，直接跳要路由的页面。
        if(to.name) {
            next();
        } else {
            next({ path: '/404' })
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
