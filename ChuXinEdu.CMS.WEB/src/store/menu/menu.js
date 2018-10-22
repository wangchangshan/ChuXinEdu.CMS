const menuType = {
    ADD_MENU            :   'ADD_MENU',         // 添加菜单
    LOAD_ROUTERS        :   'LOAD_ROUTERS',    // 加载路由
    INIT_LEFT_MENU      :   'INIT_LEFT_MENU',   // 初始化左侧菜单
    SET_LEFT_COLLAPSE   :   'SET_LEFT_COLLAPSE', // 改变左边菜单的收缩宽度
    SET_ACTIVE_MENU     :   'SET_ACTIVE_MENU',  //设置活动菜单
}

const state = {
    menuItems: [],
    activeMenu: '/dashboard',
    isRouterLoaded: false,
    sidebar: {
        opened: true,
        width: 150
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
            width: 150
        }
    },
    [menuType.SET_LEFT_COLLAPSE] (state) {
        state.isCollapse = !state.isCollapse;
        if(state.isCollapse) {
            state.sidebar.width = 40
        }
        else{
            state.sidebar.width = 150
        }
    },
    [menuType.SET_ACTIVE_MENU] (state, toMenu) {
        if(toMenu.indexOf('studentDetailMain') > -1){
            state.activeMenu = '/studentList';
        }
        else {
            state.activeMenu = toMenu;
        }
    },
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
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}