import Vue from 'vue'
import axios from 'axios'
import vueAxios from 'vue-axios'
import {
    LocalDB
} from '@/utils/index'

Vue.use(vueAxios, axios)

/**
 * 封装axios的通用请求
 * @param   {string}    type    get或post
 * @param   {string}    url     请求的接口URL
 * @param   {object}    data    传入的参数，没有则为空对象
 * @param   {Function}  fn      回调函数
 * @param   {boolean}   tokenFlag 是否需要携带token参数。true：需要；false：不需要。一般除了登陆，都需要。
 */
export default function ({
        type,
        path,
        data,
        fn,
        errFn,
        tokenFlag,
        headers,
        opts
    } = {}) {

    var options = {
        method  : type,
        url     : path,
        // headers : headers && typeof headers === 'object' ? headers : {'Access-Control-Allow-Origin': '*','Access-Control-Allow-Methods':'GET,POST,PUT,DELETE'}
        headers : headers && typeof headers === 'object' ? headers : {}
    };

    // 检测接口权限
    var api_flag = true;

    if(api_flag === true) {
        options[type === 'get' ? 'params' : 'data'] = data;
        options[type === 'delete' ? 'params' : 'data'] = data;

        // tokenFlag
        if(true) {
            let strUserInfo = LocalDB.instance('USER_').getValue('BASEINFO').value;
            if(strUserInfo) {
                //let user = JSON.parse(strUserInfo);
                //options.headers.token = user.token || '';
            }
            else{
                options.headers.token = '';
            }

            // 如果后台不会接受headers里面的参数，则使用普通参数方式传递
            //data.token = '';
        }

        // axios内置属性均可写在这里
        if (opts && typeof opts === 'object') {
            for (var f in opts) {
                options[f] = opts[f];
            }
        }

        // 发送请求。 一般请求还是表格类型的请求，因为其返回的数据结构是根据api中设定的，这里只需返回就行。
        Vue.axios(options).then((res) => {
            fn(res.data);
        }).catch((err) => {
            if(typeof errFn === 'function'){
                errFn();
            }
            // this.$message({
            //     showClose: true,
            //     message:    '请求错误：Internal Server Error',
            //     type:       'error'
            // });
        })
    }
    else{
        this.$alert('您没有权限请求该接口！', '请求错误', {
            confirmButtonText: '确定',
            type             : 'warning'
        });
    }
};
