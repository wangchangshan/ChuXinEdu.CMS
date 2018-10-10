const types = {
    SET_BIRTHDAY_LIST   :   'SET_BIRTHDAY_LIST', 
    SET_TO_FINISH_LIST  :   'SET_TO_FINISH_LIST', 
    SET_TO_RECORD       :   'SET_TO_RECORD', 
    REDUCE_TO_RECORD    :   'REDUCE_TO_RECORD'
}

const state = {
    birthdayList: [], // 未来一周过生日的学生列表
    birthdayCount: 0, // 未来一周过生日的学生数目
    toRecordCount: 0, // 待销课数目
    toFinishList: [], //还有不多于5节课时的学生列表
    toFinishCount: 0, //还有不多于5节课时的学生数目
}

const getters = {
}

const mutations = {
    [types.SET_BIRTHDAY_LIST] (state, list) {
        list.forEach(element => {
            element.student_birthday = element.student_birthday.split('T')[0]
        });
        state.birthdayList = list;
        state.birthdayCount = list.length;
    },

    [types.SET_TO_FINISH_LIST] (state, list) {
        state.toFinishList = list;
        state.toFinishCount = list.length;
    },

    [types.SET_TO_RECORD] (state, count) {
        state.toRecordCount = count;
    },

    [types.REDUCE_TO_RECORD] (state, reduceCount) {
        state.toRecordCount -= reduceCount;
    },
}

const actions = {

}

export default{
    state,
    getters,
    mutations,
    actions
}
