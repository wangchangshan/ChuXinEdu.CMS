const state = {
    all_dic_code: [
        'student_temp_status',
        'student_status',
        'teacher_status',
        'course_category',
        'course_folder',
        'pay_pattern'
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
    },
    teacher_status: state => {
        return state.allDics && state.allDics.teacher_status || [];
    },
    course_category: state => {
        return state.allDics && state.allDics.course_category || [];
    },
    course_folder: state => {
        return state.allDics && state.allDics.course_folder || [];
    },
    course_folder_meishu: state => {
        return state.allDics && state.allDics.course_folder && state.allDics.course_folder.filter(folder => folder.value.indexOf('meishu') > -1);
    },
    course_folder_shufa: state => {
        return state.allDics && state.allDics.course_folder && state.allDics.course_folder.filter(folder => folder.value.indexOf('shufa') > -1);
    },
    pay_pattern: state => {
        return state.allDics && state.allDics.pay_pattern || [];
    },
    
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
