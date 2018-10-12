const types = {
    SET_S_T_STATUS: 'SET_S_T_STATUS',
}

const state = {
    student_temp_status: [], // 临时学生状态
}

const getters = {
    student_temp_status: state => {
        return state.student_temp_status;
    }
}

const mutations = {
    [types.SET_S_T_STATUS](state, list) {
        state.student_temp_status = list;
    },
}

const actions = {
}

export default {
    state,
    getters,
    mutations,
    actions
}
