/**
 * 封装localStorage的通用存储方法
 * @param   {string}    prefix      存储内容前缀
 * @param   {string}    timeSign    时间
 */

var LocalDB = function (prefix, timeSign) {
    if(this instanceof LocalDB) {
        this.prefix = prefix;
        this.timeSign = timeSign || '|-|';
    } else {
        return new LocalDB(prefix, timeSign);
    }
}


LocalDB.prototype = {
    status: {
        SUCCESS: 0,
        FAILURE: 1,
        OVERFLOW: 2,
        TIMEOUT: 3
    },

    storage: localStorage || window.localStorage,

    getKey: function (key) {
        return this.prefix + key;
    },

    //add or modify
    set: function (key, value, callback, time) {
        var status = this.status.SUCCESS,
            key = this.getKey(key);

        try {
            time = new Date(time).getTime() || time.getTime();
        } catch (e) {
            time = new Date().getTime() + 1000 * 60 * 60 * 24 * 31;
        }

        try {
            if (Object.prototype.toString.call(value) === '[object Object]' || Object.prototype.toString.call(value) === '[object Array]') {
                value = JSON.stringify(value);
            }
            this.storage.setItem(key, time + this.timeSign + value);
        } catch (e) {
            status = this.status.OVERFLOW;
        }

        callback && callback.call(this, status, key, value);
    },

    get: function (key, callback) {
        var status = this.status.SUCCESS,
            key = this.getKey(key),
            value = null,
            timeSignlen = this.timeSign.length,
            that = this,
            index,
            time,
            result;

        try {
            value = that.storage.getItem(key);
        } catch (e) {
            result = {
                status: that.status.FAILURE,
                value: null
            };
            callback && callback.call(this, result.status, result.value);
            return result;
        }

        if (value) {
            index = value.indexOf(that.timeSign);
            time = +value.slice(0, index);
            if (new Date(time).getTime() > new Date().getTime() || time == 0) {
                value = value.slice(index + timeSignlen);
            }
            else {
                value = null;
                status = that.status.TIMEOUT;
                that.remove(key);
            }
        }
        else {
            status = that.status.FAILURE;
        }

        result = {
            status: status,
            value: value
        };
        callback && callback.call(this, result.status, result.value);

        return result;
    },

    remove: function (key, callback) {
        var status = this.status.FAILURE,
            key = this.getKey(key),
            value = null;

        try {
            value = this.storage.getItem(key);
        } catch (e) { }

        if (value) {
            try {
                this.storage.removeItem(key);
                status = this.status.SUCCESS;
            } catch (e) { }
        }
        callback && callback.call(this, status, status > 0 ? null : value.slice(value.indexOf(this.timeSign) + this.timeSign.length));
    }
}

export {LocalDB};

/**
 *  //Demo
    var localDB = new BaseLocalDB('MDT_UI_');
    localDB.set('k1', 'k1value', function () { console.log(arguments); });//[0,'k1','k1value']

    var localDB = new BaseLocalDB('MDT_CONFIG_');
    localDB.set('LoginType', 'MDT');
    var loginType = localDB.get('LoginType').value;

    var localDB = new BaseLocalDB('MDT_LOOKUP_');
    localDB.set('BoloPriority', [{Code:'P1', Description:'P1'},{Code:'P2', Description:'P2'}]);
    var BoloPriorityDB = localDB.get('BoloPriority');
    var val = JSON.parse(BoloPriority.BoloPriorityDB);
 * 
 */