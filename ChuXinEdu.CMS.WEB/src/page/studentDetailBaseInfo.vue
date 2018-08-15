<template>
<div class="info_container">
    <el-row class="info_row row" :gutter="10">
        <el-col :span="5">
            <div class="area">
                <div class="imgarea">
                    <img src="../../static/image/test2.jpg">
                    <el-button type="success" size="small">上传<i class="el-icon-upload el-icon--right"></i></el-button>
                </div>
            </div>
        </el-col>

        <el-col :span="10">
            <div class="area">
                <div class="namearea">
                    <p>姓名：王小川</p>
                    <p>性别：男</p>
                    <p>出生日期：2006-07-19</p>
                    <p>入学日期：2017-09-19</p>
                    <p>联系电话：189xxxx9028</p>
                    <p>家庭住址：北京市海淀区龙兴园小区</p>
                    <p class="awards"><i class="el-icon-edit el-icon--left"></i>编辑</p>
                </div>
            </div>
        </el-col>
        <el-col :span="9">
            <div class="area">
                <div class="dataarea">
                    <p class="gtitle"><i class="el-icon-date el-icon--left"></i>课程数据</p>
                    <div class="gdataarea clear">
                        <div class="gdata left">
                            <p class="num">40</p>
                            <p class="title">总课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">18</p>
                            <p class="title">完成课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">￥3000</p>
                            <p class="title">缴费金额</p>
                        </div>
                    </div>
                </div>
            </div>
        </el-col>
    </el-row>
    <el-row class="info_row row" :gutter="10">
        <el-col :span="24">
            <template>
                <el-table :data="arrangementList" stripe border style="width: 100%">
                    <el-table-column prop="package_name" align='center' label="课程套餐" min-width="200" fixed>
                    </el-table-column>
                    <el-table-column prop="package_type" align='center' label="课程类别" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_date" align='center' label="缴费日期" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_mount" align='center' label="缴费金额（元）" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_type" align='center' label="缴费方式" min-width="180">
                    </el-table-column>
                    <el-table-column prop="teacher_name" align='center' label="收款人" min-width="180">
                    </el-table-column>
                    <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                        <template slot-scope='scope'>
                            <el-button type="success" icon='edit' size="small" @click='updateStudentPackage()'>更新</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <div style="margin-top:10px">
                    <el-button type="success" icon="el-icon-plus" size="small" @click="packageDialog.show = true">添加课程套餐</el-button>
                </div>
            </template>
        </el-col>
    </el-row>

    <el-dialog :title="packageDialog.title" :visible.sync="packageDialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="courseInfo" :model="packageDialog.courseInfo" :rules="packageDialog.courseInfoRules" :label-width="packageDialog.formLabelWidth" :label-position='packageDialog.labelPosition' style="margin:10px;width:auto;">
                <el-form-item prop="selected_package" label="课程类型">
                    <el-cascader :options="packageDialog.courseInfo.course_package" v-model="packageDialog.courseInfo.selected_package" @change="handleCoursePackageChange()"></el-cascader>
                </el-form-item>
                <el-form-item prop="selected_course" label="课程内容">
                    <el-checkbox-group v-model="packageDialog.courseInfo.selected_course">
                        <el-checkbox v-for="item in packageDialog.courseInfo.course_type" :key="item.value" :label="item.value" :disabled="handleCourseTypeDisplay(item)">{{item.label}}</el-checkbox>
                    </el-checkbox-group>
                </el-form-item>
                <el-form-item prop="is_payed" label="是否缴费">
                    <el-radio-group v-model="packageDialog.courseInfo.is_payed">
                        <el-radio label="Y">是</el-radio>
                        <el-radio label="N">否</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="是否优惠" v-show="packageDialog.courseInfo.is_payed == 'Y'">
                    <el-radio-group v-model="packageDialog.courseInfo.is_discount" @change="handleIsDiscount()">
                        <el-radio label="Y">是</el-radio>
                        <el-radio label="N">否</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="优惠价格" v-show="packageDialog.courseInfo.is_payed == 'Y'">
                    <el-input v-model="packageDialog.courseInfo.discount_amount" :disabled="packageDialog.courseInfo.is_discount == 'N'"></el-input>
                </el-form-item>

                <el-form-item label="缴费类型" v-show="packageDialog.courseInfo.is_payed == 'Y'">
                    <el-select v-model="packageDialog.courseInfo.selected_payment_type" placeholder="请选择">
                        <el-option v-for="item in packageDialog.courseInfo.payment_type" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="缴费日期" v-show="packageDialog.courseInfo.is_payed == 'Y'">
                    <el-date-picker v-model="packageDialog.courseInfo.payment_date" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="收款人" v-show="packageDialog.courseInfo.is_payed == 'Y'">
                    <el-select v-model="packageDialog.courseInfo.money_receiver" placeholder="请选择">
                        <el-option v-for="item in packageDialog.courseInfo.receivers" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item class="text_center">
                        <el-button @click="packageDialog.show = false">取 消</el-button>
                        <el-button type="primary">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
export default {
    name: 'student-base-info',
    data() {
        return {
            arrangementList: [{
                package_name: '40节美术套餐',
                package_type: '美术',
                free_date: '2018-06-07',
                free_mount: '5000',
                free_type: '微信',
                teacher_name: '唐得红'
            }, {
                package_name: '40节美术套餐',
                package_type: '美术',
                free_date: '2018-06-07',
                free_mount: '5000',
                free_type: '微信',
                teacher_name: '唐得红'
            }, {
                package_name: '40节美术套餐',
                package_type: '美术',
                free_date: '2018-06-07',
                free_mount: '5000',
                free_type: '微信',
                teacher_name: '唐得红'
            }],
            packageDialog: {
                width: '500px',
                show: false,
                title: '上传作品',
                labelPosition: 'right',
                formLabelWidth: '120px',
                courseInfo: {
                    course_package: [{
                        value: 'meishu',
                        label: '美术',
                        children: [{
                            value: 'meishu_taocan1',
                            label: '20节课2800元'
                        }, {
                            value: 'g_taocan2',
                            label: '40节课5300元'
                        }, {
                            value: 'g_taocan3',
                            label: '80节课8300元'
                        }, {
                            value: 'g_taocan4',
                            label: '暑期活动20节课2100元'
                        }, {
                            value: 'x_taocan1',
                            label: '20节课2800元'
                        }, {
                            value: 'x_taocan2',
                            label: '40节课5300元'
                        }, {
                            value: 'x_taocan3',
                            label: '80节课8300元'
                        }]
                    }, {
                        value: 'shufa',
                        label: '书法',
                        children: [{
                            value: 's_taocan1',
                            label: '书法20节课2600元'
                        }, {
                            value: 's_taocan2',
                            label: '书法40节课5200元'
                        }, {
                            value: 's_taocan3',
                            label: '书法80节课8200元'
                        }]
                    }],
                    selected_package: [],
                    course_type: [{
                            value: "meishu_guohua",
                            label: "国画"
                        },
                        {
                            value: "meishu_xihua",
                            label: "西画"
                        }, {
                            value: "shufa_shufa",
                            label: "书法"
                        }
                    ],
                    selected_course: [],
                    is_discount: 'N',
                    discount_amount: 0,
                    is_payed: '',
                    selected_payment_type: '',
                    payment_type: [{
                        value: 'zhifubao',
                        label: '支付宝'
                    }, {
                        value: 'weixin',
                        label: '微信'
                    }, {
                        value: 'yinhangka',
                        label: '银行卡'
                    }, {
                        value: 'pose',
                        label: 'Pose机'
                    }, {
                        value: 'cash',
                        label: '现金'
                    }],
                    money_receiver: '',
                    receivers: [{
                        value: 'tangdehong',
                        label: '唐得红'
                    }, {
                        value: 'mazhao',
                        label: '马朝'
                    }],
                    payment_date: ''
                },
                courseInfoRules: []
            }
        }
    },
    methods: {
         handleCourseTypeDisplay(item) {
            //meishu_guohua   meishu_xihua
            //console.log(this.courseInfo.selected_course); // 存储的是label属性
            if (this.packageDialog.courseInfo.selected_package.length == 0) {
                return true;
            } else if (item.value.indexOf(this.packageDialog.courseInfo.selected_package[0]) > -1) {
                return false;
            } else {
                let index = this.courseInfo.selected_course.indexOf(item.value);
                if (index > -1) {
                    this.courseInfo.selected_course.splice(index, 1);
                }
                return true;
            }
        },
        updateStudentPackage(){
            alert('修改是否收费信息')
        }
    }
}
</script>

<style lang="less" scoped>
.info_container {
    padding: 0;
    margin: 0;
    overflow-x: hidden;
}

.row {
    margin: 20px;
}

.info_row {
    .area {
        border: 1px solid #dfdfdf;
        height: 200px;
        overflow: hidden;
        .imgarea {
            text-align: center;
            padding: 8px;
            img {
                width: 150px;
                height: 150px;
                border-radius: 50%;
            }
        }
        .namearea {
            padding: 10px;
            font-size: 14px;
            p {
                line-height: 24px;
            }
            .awards {
                text-align: center;
                width: 100%;
                height: 30px;
                line-height: 30px;
                cursor: pointer;
                background-color: #3bc5ff;
                border: 1px solid #3bc5ff;
                color: white;
                display: block;
            }
            .awards:hover {
                background-color: #f9c855;
                border: 1px solid #f9c855;
            }
        }
        .dataarea {
            padding: 10px;
            text-align: center;
            font-size: 14px;
            .gtitle {
                width: 100%;
                height: 30px;
                line-height: 30px;
                cursor: pointer;
                background-color: #3bc5ff;
                color: white;
                display: block;
            }
            .gdataarea {
                padding-left: 25px;
                p {
                    line-height: 38px;
                }
                .num {
                    font-weight: bolder;
                    color: #67c23a;
                }
                .title {
                    color: #3bc5ff;
                }
                .gdata {
                    margin: 10px;
                    float: left;
                }
            }
            .morearea {
                a {
                    color: #3bc5ff;
                }
            }
        }
    }
}
</style>
