<template>
<div class="info_container">
    <el-row class="info_row row" :gutter="10" style="margin:10px">
        <el-container>
            <el-aside width="260px">
                <div class="avatar-panel">
                    <my-avatar field="img" @crop-success="cropSuccess" @crop-upload-success="cropUploadSuccess" @crop-upload-fail="cropUploadFail" v-model="avatarPanel.isShow" :width="300" :height="300" :url="avatarPanel.uploadUrl" :params="avatarPanel.params" img-format="png">
                    </my-avatar>
                    <img :src="avatarPanel.imgDataUrl">
                    <el-button type="success" size="mini" @click="setAvatarShow">设置头像<i class="el-icon-upload el-icon--right"></i></el-button>
                </div>
            </el-aside>
            <el-main style="padding:10px; padding-bottom:0">
                <el-row :gutter="10">
                    <el-col :span="24" style="min-width:280px">
                        <el-card shadow="never" class="card-teacher-base">
                            <el-form :label-width="teacherDialog.formLabelWidth" :label-position='teacherDialog.labelPosition' size="mini" label-suffix="：">
                                <el-form-item label="姓名">
                                    {{ pageData.teacherInfo.teacherName }} &nbsp;&nbsp; <el-tag size="medium" :type="teacherStatusTag(pageData.teacherInfo.teacherStatus)">{{ pageData.teacherInfo.teacherStatusDesc}}</el-tag>
                                </el-form-item>
                                <el-form-item label="性别">
                                    {{ pageData.teacherInfo.teacherSex }}
                                </el-form-item>
                                <el-form-item label="出生日期">
                                    {{ pageData.teacherInfo.teacherBirthday }}
                                </el-form-item>
                                <el-form-item label="报名日期">
                                    {{ pageData.teacherInfo.teacherRegisterDate }}
                                </el-form-item>
                                <el-form-item label="联系电话">
                                    {{ pageData.teacherInfo.teacherPhone }}
                                </el-form-item>
                                <el-form-item label="家庭住址">
                                    {{ pageData.teacherInfo.teacherAddress }}
                                </el-form-item>
                                <el-form-item label="备注">
                                    {{ pageData.teacherInfo.teacherRemark }}
                                </el-form-item>
                                <el-form-item>
                                    <el-button type="primary" @click="showUpdateTeacher()"><i class="el-icon-edit el-icon--left"></i>编 辑</el-button>
                                </el-form-item>
                            </el-form>
                        </el-card>
                    </el-col>
                </el-row>
            </el-main>
        </el-container>
    </el-row>
    <el-dialog :title="teacherDialog.title" :width="teacherDialog.width" :visible.sync="teacherDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="baseInfo" :model="teacherDialog.baseInfo" :rules="teacherDialog.baseInfoRules" :label-width="teacherDialog.formLabelWidth" :label-position='teacherDialog.labelPosition' size="mini">
                <el-form-item label="姓名">
                    {{ teacherDialog.baseInfo.teacherName }}
                    <el-select v-model="teacherDialog.baseInfo.teacherStatus" placeholder="请选择教师状态">
                        <el-option v-for="item in $store.getters['teacher_status']" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="teacherDialog.baseInfo.teacherSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="teacherDialog.baseInfo.teacherBirthday" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item prop="teacherIdentityCardNum" label="身份证号码">
                    <el-input v-model="teacherDialog.baseInfo.teacherIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item prop="teacherPhone" label="联系电话">
                    <el-input v-model="teacherDialog.baseInfo.teacherPhone"></el-input>
                </el-form-item>
                <el-form-item prop="teacherAddress" label="家庭地址">
                    <el-input v-model="teacherDialog.baseInfo.teacherAddress"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="teacherDialog.baseInfo.teacherRemark"></el-input>
                </el-form-item>
                <el-form-item label="入职时间">
                    <el-date-picker v-model="teacherDialog.baseInfo.teacherRegisterDate" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="teacherDialog.isShow = false">取 消</el-button>
                    <el-button size="small" type="primary" @click="submitUpdateTeacher('baseInfo')">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios,
    dicHelper,
    tagTypeHelper
} from '@/utils/index'

import myAvatar from 'vue-image-crop-upload';

export default {
    name: 'teacher-base-info',
    props: {
        'teacherCode': String,
    },
    data() {
        return {
            avatarPanel: {
                isShow: false,
                params: {
                    type: 'teacher',
                    token: '123456798',
                    code: '',
                    name: ''
                },
                uploadUrl: '/api/upload/uploadavatar',
                imgDataUrl: ''
            },
            pageData: {
                teacherInfo: {
                    teacherCode: "",
                    teacherName: "",
                    teacherSex: "",
                    teacherBirthday: "",
                    teacherIdentityCardNum: "",
                    teacherPhone: "",
                    teacherPropagateType: "",
                    teacherPropagateTxt: "",
                    teacherRegisterDate: "",
                    teacherAddress: "",
                    teacherAvatarPath: "",
                    teacherStatus: "",
                    teacherStatusDesc: ""
                },
                courseOverview: []
            },
            teacherDialog: {
                width: '600px',
                isShow: false,
                title: '更新教师基本信息',
                labelPosition: 'right',
                formLabelWidth: '100px',
                curId: 0,
                baseInfo: {
                    teacherName: "",
                    teacherSex: "",
                    teacherBirthday: "",
                    teacherIdentityCardNum: "",
                    teacherPhone: "",
                    teacherAddress: "",
                    teacher_introduce: "",
                    teacherRemark: "",
                    teacherRegisterDate: "",
                    teacherStatus: "",
                },
                baseInfoRules: {
                    teacherPhone: [{
                        required: true,
                        message: '请输入联系电话',
                        trigger: 'blur'
                    }],
                    teacherAddress: [{
                        required: true,
                        message: '请输入家庭地址',
                        trigger: 'blur'
                    }],
                    teacherIdentityCardNum: [{
                        required: true,
                        message: '请输入身份证号码',
                        trigger: 'blur'
                    }]
                }
            }
        }
    },
    components: {
        'my-avatar': myAvatar
    },
    created() {
        var _this = this;
        _this.GetTeacherBaseData();
    },
    methods: {
        setAvatarShow() {
            this.avatarPanel.isShow = !this.avatarPanel.isShow;
            this.avatarPanel.params.code = this.pageData.teacherInfo.teacherCode;
            this.avatarPanel.params.name = this.pageData.teacherInfo.teacherName;
        },

        cropSuccess(imgDataUrl, field) {
            console.log('-------- crop success --------');
            this.avatarPanel.imgDataUrl = imgDataUrl;
        },
        cropUploadSuccess(jsonData, field) {
            console.log('-------- upload success --------');
            console.log(jsonData);
            console.log('field: ' + field);
        },

        cropUploadFail(status, field) {
            console.log('-------- upload fail --------');
            console.log(status);
            console.log('field: ' + field);
        },

        GetTeacherBaseData() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/teacher/getteacherbase/' + _this.teacherCode,
                fn: function (result) {
                    result.teacherBirthday = result.teacherBirthday.split('T')[0];
                    result.teacherRegisterDate = result.teacherRegisterDate.split('T')[0];
                    result.teacherStatusDesc = dicHelper.getLabelByValue(_this.$store.getters['teacher_status'], result.teacherStatus);
                    _this.avatarPanel.imgDataUrl = result.teacherAvatarPath;
                    _this.pageData.teacherInfo = result;
                    _this.teacherDialog.curId = result.id;
                }
            });
        },

        showUpdateTeacher() {
            this.teacherDialog.isShow = true;
            this.teacherDialog.baseInfo = {
                teacherName: this.pageData.teacherInfo.teacherName,
                teacherSex: this.pageData.teacherInfo.teacherSex,
                teacherBirthday: this.pageData.teacherInfo.teacherBirthday,
                teacherIdentityCardNum: this.pageData.teacherInfo.teacherIdentityCardNum,
                teacherPhone: this.pageData.teacherInfo.teacherPhone,
                teacherAddress: this.pageData.teacherInfo.teacherAddress,
                teacherRemark: this.pageData.teacherInfo.teacherRemark,
                teacherRegisterDate: this.pageData.teacherInfo.teacherRegisterDate,
                teacherStatus: this.pageData.teacherInfo.teacherStatus
            };
        },

        submitUpdateTeacher(teacherForm) {
            var _this = this;
            this.$refs[teacherForm].validate((valid) => {
                if (valid) {
                    var teacher = Object.assign({}, _this.teacherDialog.baseInfo);
                    axios({
                        type: 'put',
                        path: '/api/teacher/updateteacher/' + _this.teacherDialog.curId,
                        data: teacher,
                        fn: function (result) {
                            if (result === 200) {
                                _this.GetTeacherBaseData();
                                _this.$message({
                                    message: '更新基础信息成功',
                                    type: 'success'
                                });
                                _this.teacherDialog.isShow = false;
                            }
                        }
                    });
                }
            });

        },

        GetLabelByValue(lst, value) {
            let label = '';
            for (let obj of lst) {
                if (obj['value'] == value) {
                    label = obj['label'];
                    break;
                }
            }
            return label;
        },

        teacherStatusTag(teacherStatusCode) {
            return tagTypeHelper.teacherStatusTag(teacherStatusCode);
        }
    }
}
</script>

<style lang="less" scoped>
.el-card__body .el-form-item--mini {
    margin-bottom: 5px;
}

.info_container {
    padding: 0;
    margin-top: -20px;
    overflow-x: hidden;
}

.row {
    margin: 20px;
}

.info_row {
    .avatar-panel {
        overflow: hidden;
        text-align: center;
        margin-top: 20px;

        img {
            width: 210px;
            height: 210px;
            border-radius: 50%;
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

        .morearea {
            a {
                color: #3bc5ff;
            }
        }
    }
}
</style>
