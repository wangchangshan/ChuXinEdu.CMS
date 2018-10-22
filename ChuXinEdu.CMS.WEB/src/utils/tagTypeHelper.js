
const tagTypeHelper = {
    studentStatusTag(statusCode) {
        let type = '';
        switch (statusCode) {
            case '01': // 正常在学
                type = 'success'
                break;
            case '02': // 中途退费
                type = 'danger'
                break;
            case '03': // 结束未续费
                type = 'info'
                break;
        }
        return type;
    },
    courseCategoryTag(categoryCode) {
        let type = '';
        switch (categoryCode) {
            case 'meishu':
                type = 'success'
                break;
            case 'shufa':
                type = ''
                break;
        }
        return type;
    },
    courseFolderTag(folderCode) {
        let type = '';
        switch (folderCode) {
            case 'meishu_00':
                type = 'success';
                break;
            case 'meishu_01':
                type = '';
                break;
            case 'shufa_00':
                type = ''
                break;
        }
        return type;
    },
    studentTrialResultTag(result) {
        let type = '';
        switch (result) {
            case '成功':
                type = 'success'
                break;
            case '失败':
                type = 'info'
                break;
            case '待定':
                type = 'warning'
                break;
        }
        return type;
    }
}

export { tagTypeHelper }

