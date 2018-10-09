const menuType = {
    ADD_MENU            :   'ADD_MENU',         // 添加菜单
    LOAD_ROUTERS        :   'LOAD_ROUTERS',    // 加载路由
    INIT_LEFT_MENU      :   'INIT_LEFT_MENU',   // 初始化左侧菜单
    SET_LEFT_COLLAPSE   :   'SET_LEFT_COLLAPSE', // 改变左边菜单的收缩宽度
    DISPLAY_MENU_NAME   :   'DISPLAY_MENU_NAME',   // 收缩时，用于隐藏菜单名称
}

const state = {
    menuItems: [],
    isRouterLoaded: false,
    sidebar: {
        opened: true,
        width: 160
    },
    isCollapse: false,
    isHidMenuName: false
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
            width: '160px'
        }
    },
    [menuType.SET_LEFT_COLLAPSE] (state) {
        state.isCollapse = !state.isCollapse;
        if(state.isCollapse) {
            state.sidebar.width = 40
        }
        else{
            state.sidebar.width = 160
        }
    },
    [menuType.DISPLAY_MENU_NAME] (state) {
        state.isHidMenuName = !state.isHidMenuName;
    }
}

const actions = {
    addMenu:({ commit }, menuItems) => {
        if(menuItems.length > 0) {
            commit(menuType.ADD_MENU, menuItems);
        }
    },
    loadRouters:({ commit }) => {
        commit(menuType.LOAD_ROUTERS);
    },
    initLeftMenu:({ commit }) => {
        commit(menuType.INIT_LEFT_MENU);
    },
    setMenuCollapse:({ commit }) => {  
       commit(menuType.SET_LEFT_COLLAPSE)  
    },
    setMenuNameDispaly:({ commit }) => {  
       commit(menuType.DISPLAY_MENU_NAME)  
    },
}

export default {
    state,
    getters,
    mutations,
    actions
}