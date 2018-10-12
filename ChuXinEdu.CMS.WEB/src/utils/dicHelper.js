
//import store from './../store/index'

const dicHelper = {
    getLabelByValue(lst, value) {
        let label = '';
        for (let dic of lst) {
            if (dic['value'] == value) {
                label = dic['label'];
                break;
            }
        }
        return label;
    },
}

export { dicHelper }

