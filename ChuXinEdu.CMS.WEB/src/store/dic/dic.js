const state = {
    all_dic_code: [
        'student_temp_status',
        'student_status',
    ].join(','),

    allDics: null,
    // student_temp_status: [], // 临时学生状态
    // student_status:[],
}

const getters = {
    all_dic_code: state => {
        return state.all_dic_code;
    },
    student_temp_status: state => {
        return state.allDics && state.allDics.student_temp_status || [];
    },
    student_status: state => {
        return state.allDics && state.allDics.student_status || [];
    }
}

const mutations = {
    set_all_dic(state, obj){
        state.allDics = obj;
    },
    // set_student_temp_status(state, list) {
    //     state.student_temp_status = list;
    // },
    // set_student_status(state, list) {
    //     state.student_status = list;
    // },
}

const actions = {
}

export default {
    state,
    getters,
    mutations,
    actions
}
