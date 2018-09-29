<template>
<div class="info_container">
    <el-row class="info_row row" :gutter="5">
        <el-col :span="20">
            <div class="area">
                <p class="title"><i class="fa fa-edit"></i>基础信息</p>
                <el-form ref="studentBaseInfo" :model="studentBaseInfo" :rules="studentBaseInfoRules" :label-width="formUtil.formLabelWidth" :label-position='formUtil.labelPosition' style="margin:10px;width:auto;" size="mini">
                    <el-form-item prop="student_name" label="姓名">
                        <el-input v-model="studentBaseInfo.student_name" clearable></el-input>
                    </el-form-item>
                    <el-form-item prop="student_sex" label="性别">
                        <el-radio-group v-model="studentBaseInfo.student_sex">
                            <el-radio label="男"></el-radio>
                            <el-radio label="女"></el-radio>
                        </el-radio-group>
                    </el-form-item>
                    <el-form-item label="出生日期">
                        <el-date-picker v-model="studentBaseInfo.student_birthday" type="date" placeholder="选择日期"> </el-date-picker>
                    </el-form-item>
                    <el-form-item label="身份证号码">
                        <el-input v-model="studentBaseInfo.student_identity_card_num"></el-input>
                    </el-form-item>
                    <el-form-item prop="student_phone" label="联系电话">
                        <el-input v-model="studentBaseInfo.student_phone"></el-input>
                    </el-form-item>
                    <el-form-item prop="student_address" label="家庭地址">
                        <el-input v-model="studentBaseInfo.student_address"></el-input>
                    </el-form-item>
                    <el-form-item prop="student_introduce" label="介绍人">
                        <el-input v-model="studentBaseInfo.student_phone"></el-input>
                    </el-form-item>
                    <el-form-item label="备注">
                        <el-input type="textarea" v-model="studentBaseInfo.student_remark"></el-input>
                    </el-form-item>
                    <el-form-item prop="student_register_date" label="报名时间">
                        <el-date-picker v-model="studentBaseInfo.student_register_date" type="date" placeholder="选择日期"> </el-date-picker>
                    </el-form-item>
                </el-form>
            </div>
        </el-col>
    </el-row>

    <el-row class="row">
        <el-col :span="20">
            <div class="head-buttons-group">
                <el-button size="small">取 消</el-button>
                <el-button size="small" type="primary" @click="submitStudent('studentBaseInfo','courseInfo')">提 交</el-button>
            </div>
        </el-col>
    </el-row>
</div>
</template>

<script>
export default {
    data() {
        return {
            formUtil: {
                labelPosition: "right",
                formLabelWidth: "120px"
            },
            studentBaseInfo: {
                student_name: "",
                student_sex: "",
                student_birthday: "",
                student_identity_card_num: "",
                student_phone: "",
                student_address: "",
                student_introduce: "",
                student_remark: "",
                student_register_date: ""
            },
            studentBaseInfoRules: {
                student_name: [{
                        required: true,
                        message: '请输入学生姓名',
                        trigger: 'blur'
                    }
                ],
                student_sex: [{
                        required: true,
                        message: '请选择性别',
                        trigger: 'blur'
                    }
                ],
                student_phone: [{
                        required: true,
                        message: '请输入联系电话',
                        trigger: 'blur'
                    }
                ],
                student_address: [{
                        required: true,
                        message: '请输入家庭地址',
                        trigger: 'blur'
                    }
                ],
                student_register_date: [{
                        required: true,
                        message: '请选择报名时间',
                        trigger: 'blur'
                    }
                ]
            },
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
            courseInfoRules: {
                selected_package: [{
                        required: true,
                        message: '请选择课程类型',
                        trigger: 'blur'
                    }
                ],
                selected_course: [{
                        required: true,
                        message: '请选择课程内容',
                        trigger: 'blur'
                    }
                ],
                is_payed: [{
                        required: true,
                        message: '请选择是否缴费',
                        trigger: 'blur'
                    }
                ],
            }
        };
    },
    methods: {
        handleCoursePackageChange() {

        },
        handleCourseTypeDisplay(item) {
            //meishu_guohua   meishu_xihua
            //console.log(this.courseInfo.selected_course); // 存储的是label属性
            if (this.courseInfo.selected_package.length == 0) {
                return true;
            } else if (item.value.indexOf(this.courseInfo.selected_package[0]) > -1) {
                return false;
            } else {
                let index = this.courseInfo.selected_course.indexOf(item.value);
                if (index > -1) {
                    this.courseInfo.selected_course.splice(index, 1);
                }
                return true;
            }
        },
        handleIsDiscount() {
            if (this.courseInfo.is_discount == 'N') {
                this.courseInfo.discount_amount = 0;
            }
        },
        submitStudent(baseInfoForm){
            var isValidate = true;
            this.$refs[baseInfoForm].validate((valid) => {
                if (valid) {
                   isValidate = false;
                }
            });
            if(isValidate){
                //submit the student info
            }
        },
    }
};
</script>

<style lang="less" scoped>
[v-cloak] {
        display: none !important;
}

.info_container {
    padding: 0 10px;
    margin: 0 10px;
    overflow: auto;
}

.el-cascader {
    width: 100%;
}

.title {
    text-align: center;
    width: 100%;
    height: 40px;
    line-height: 40px;
    cursor: pointer;
    background-color: #3bc5ff;
    border: 1px solid #3bc5ff;
    color: #fff;
    display: block;
    .fa {
        margin-right: 5px;
    }
}

.info_row {
    .area {
        border: 1px solid #dfdfdf;
        height: 100%;
        font-size: 14px;
        padding: 10px;
        .form {
            width: 90%;
            margin-top: 20px;
        }
    }
}

.head-buttons-group {
    height: 40px;
    line-height: 40px;
    text-align: right;
}
</style>
