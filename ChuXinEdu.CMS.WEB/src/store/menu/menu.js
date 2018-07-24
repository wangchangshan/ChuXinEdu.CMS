const menuType = {
    ADD_MENU        :   'ADD_MENU',         // 添加菜单
    LOAD_ROUTERS    :   'LOAD_ROUTERS',    // 加载路由
    INIT_LEFT_MENU  :   'INIT_LEFT_MENU',   // 初始化左侧菜单
}

const state = {
    menuItems: [],
    isRouterLoaded: false,
    sidebar: {
        opened: true,
        width: '180px'
    }
}

const getters = {
    getMenuItems: state => {
        return state.menuItems;
    },
    getRouterLoadedStatus: state => {
        return state.isRouterLoaded;
    }
}

const mutations = {
    [menuType.ADD_MENU] (state, menuItems) {
        if (menuItems.length === 0){
            state.menuItems = [];
        } else {
            state.menuItems = menuItems;
        }
    },
    [menuType.LOAD_ROUTERS] (state) {
        state.isRouterLoaded = !state.isRouterLoaded;
    },
    [menuType.INIT_LEFT_MENU] (state) {
        state.sidebar = {
            opened: true,
            width: '180px'
        }
    }
}

const actions = {
    addMenu:({ commit }, menuItems) => {
        if(menuItems.length > 0) {
            commit(menuType.ADD_MENU, menuitems);
        }
    },
    loadRouters:({ commit }) => {
        commit(menuType.LOAD_ROUTERS);
    },
    initLeftMenu:({ commit }) => {
        commit(menuType.INIT_LEFT_MENU);
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}