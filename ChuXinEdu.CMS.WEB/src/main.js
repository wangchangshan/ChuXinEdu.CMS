// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store/index'
import ElementUI from 'element-ui'
import {LocalDB, menuHelper} from './utils/index'
import Home from '@/layout/home'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import 'element-ui/lib/theme-chalk/index.css'
import 'font-awesome/css/font-awesome.css'
import '@/style/index.scss' // global css

Vue.use(ElementUI)

NProgress.configure({ showSpinner: false })// NProgress Configuration

/**
 * 如果用户刷新页面,导致存入vuex中的菜单数据清空,需要从缓存获取;
 */
var strMenuData = LocalDB.instance('MENU_').getValue('LEFTMENU').value;
var menuData = '';
if(strMenuData) {
    menuData = JSON.parse(strMenuData)
} 
if(menuData) { //menuData  存在router会重复的问题 或者退出登录的时候需要清空路由
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

var strAllDics = LocalDB.instance('DIC_').getValue('ALL').value;
var dics = '';
if(strAllDics) {
    dics = JSON.parse(strAllDics)
} 
if(dics){
    store.commit('set_all_dic', dics); 
}

router.beforeEach((to, from, next) => {
    NProgress.start() 
    let hasPermission = false;
    // 定位到登录页面时， 清空缓存数据
    if(to.path === '/') {
        LocalDB.instance('USER_').remove('BASEINFO');
        LocalDB.instance('MENU_').remove('LEFTMENU');
        store.commit('ADD_MENU', []);
        hasPermission = true;
    }

    // 判断是否有用户登录的记录
    let userInfo = JSON.parse(LocalDB.instance('USER_').getValue('BASEINFO').value);
    // 没有用户信息，route.path不是定位到登录页面的,直接跳登录页面。
    if(!userInfo && to.path !== '/') {
        next({ path: '/' });
        NProgress.done()
    } else {
        // 有用户信息和路由名称的，直接跳要路由的页面。
        if(!hasPermission && to.meta.roles){
            let pageRoles = to.meta.roles;
            let arrUserRole = userInfo.roles && userInfo.roles.split(',') || [];
            
            for(let r of arrUserRole){
                if(pageRoles.indexOf(',' + r + ',') > -1){
                    hasPermission = true;
                    break;
                }
            }
        }

        if(to.name) {
            if(hasPermission) {
                store.commit('SET_ACTIVE_MENU', to.path);
                next();
            }else{
                next({ path: '/403' })
            }
        } else {
            next({ path: '/404' })
        }
    }
});

router.afterEach(() => {
    NProgress.done() // finish progress bar
  })

Vue.config.productionTip = false

Vue.directive('noRepeatClick', {
    inserted(el, binding) {
        el.addEventListener('click', e => {
            el.classList.add('is-disabled');
            el.disabled = true;
            setTimeout(() => {
                el.disabled = false;
                el.classList.remove('is-disabled');
            }, 1000)
        })
    }
})


/* eslint-disable no-new */
new Vue({
    el: '#app',
    router,
    store,
    components: { App },
    template: '<App/>'
})
