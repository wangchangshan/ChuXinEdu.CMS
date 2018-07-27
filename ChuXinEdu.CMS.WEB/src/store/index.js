import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

import user from './user/index'
import menu  from './menu/index'
import page from './page/index'

export default new Vuex.Store({
    modules: {
        user: user,
        menu: menu,
        page: page
    }
});