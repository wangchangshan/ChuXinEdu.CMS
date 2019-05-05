const dateHelper = {
    getWeekNameByCode(code) {
        let week = '';
        switch (code) {
            case 'day1':
                week = '星期一';
                break;
            case 'day2':
                week = '星期二';
                break;
            case 'day3':
                week = '星期三';
                break;
            case 'day4':
                week = '星期四';
                break;
            case 'day5':
                week = '星期五';
                break;
            case 'day6':
                week = '星期六';
                break;
            case 'day7':
                week = '星期日';
                break;
            default:
                break;
        }
        return week;
    },
    getWeekNameByDate(theDay) {
        let week = '';
        let code = new Date(theDay).getDay();
        switch (code) {
            case 0:
                week = '星期日';
                break;
            case 1:
                week = '星期一';
                break;
            case 2:
                week = '星期二';
                break;
            case 3:
                week = '星期三';
                break;
            case 4:
                week = '星期四';
                break;
            case 5:
                week = '星期五';
                break;
            case 6:
                week = '星期六';
                break;
            case 7:
                week = '星期日';
                break;
            default:
                break;
        }
        return week;
    },
    getWeekCodeByDate(theDay) {
        let week = '';
        let code = new Date(theDay).getDay();
        switch (code) {
            case 0:
                week = 'day7';
                break;
            case 1:
                week = 'day1';
                break;
            case 2:
                week = 'day2';
                break;
            case 3:
                week = 'day3';
                break;
            case 4:
                week = 'day4';
                break;
            case 5:
                week = 'day5';
                break;
            case 6:
                week = 'day6';
                break;
            case 7:
                week = 'day7';
                break;
            default:
                break;
        }
        return week;
    },

    getDate(theDay) {
        var date;
        if (!theDay) {
            date = new Date();
        }
        else {
            date = new Date(theDay);
        }
        var seperator1 = "-";
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = year + seperator1 + month + seperator1 + strDate;
        return currentdate;
    },

    calcuteNewDate(_dateObject, x) {
        if (_dateObject == null || undefined == _dateObject || _dateObject == '') {
            _dateObject = new Date();
        }

        _dateObject.setMonth(_dateObject.getMonth() + x);

        //返回日期的毫秒表示
        var nd = _dateObject.valueOf();
        nd = new Date(nd);

        var y = nd.getFullYear();
        var m = nd.getMonth() + 1;
        var d = nd.getDate();

        if (m <= 9) m = "0" + m;
        if (d <= 9) d = "0" + d;
        var cdate = y + "-" + m;

        return cdate;
    },

    getDefaultMonthRange() {
        let endDate = new Date()
        let endMonth = this.calcuteNewDate(endDate, 0);
        let beginMonth = this.calcuteNewDate(endDate, -6);
        return beginMonth + ',' + endMonth;
    }
}

export { dateHelper }