
const tagTypeHelper = {
    studentStatusTag(statusCode) {
        let type = '';
        switch (statusCode) {
            case '00': // 试听
                type = ''
                break;
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
}

export { tagTypeHelper }

