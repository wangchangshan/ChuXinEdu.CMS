import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

import dic from './dic/index'
import user from './user/index'
import menu  from './menu/index'
import header from './header/index'
import page from './page/index'

export default new Vuex.Store({
    modules: {
        dic: dic,
        user: user,
        menu: menu,
        header: header,
        page: page
    }
});