import Vue from 'vue'
import Router from 'vue-router'
import Login from '@/page/login'

Vue.use(Router)

export default new Router({
    routes: [
        {
            path: '/',
            name: '登录',
            component: Login
        }
    ]
})
