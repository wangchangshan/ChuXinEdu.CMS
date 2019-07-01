const types = {
    SET_WIN_CONTENT : 'SET_WIN_CONTENT',  // 设置content部分的宽高
    SET_TEMPLATE_CODE: 'SET_TEMPLATE_CODE', //设置全局模板编码
}

const state = {
    win_content: {
        width: '',
        height: ''
    },
    arrangeTemplateCode: ''
}

const getters = {
    width: state => state.win_content.width,
    height: state => state.win_content.height,
    templateCode: state => state.arrangeTemplateCode
}

const mutations = {
    [types.SET_WIN_CONTENT] (state, contentobjs) {
        state.win_content.width = contentobjs.width;
        state.win_content.height = contentobjs.height;
    },

    [types.SET_TEMPLATE_CODE] (state, code) {
        state.arrangeTemplateCode = code;
    }
}

const actions = {
    set_win_content:({commit}, contentobjs) => {
        commit(types.SET_WIN_CONTENT, contentobjs)
    },

    set_template_code:({commit}, code) => {
        commit(types.SET_TEMPLATE_CODE, code)
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}