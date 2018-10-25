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
                    <el-col :span="12" style="min-width:280px">
                        <el-card shadow="never" class="card-teacher-base">
                            <el-form :label-width="packageDialog.formLabelWidth" :label-position='packageDialog.labelPosition' size="mini" label-suffix="：">
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
                                    <el-button type="primary" @click="showUpdateStudent()"><i class="el-icon-edit el-icon--left"></i>编 辑</el-button>
                                </el-form-item>
                            </el-form>
                        </el-card>
                    </el-col>
                    <el-col :span="12" style="min-width:340px">
                        <el-card shadow="never" class="card-teacher-course">
                            <div class="dataarea">
                                <p class="gtitle"><i class="el-icon-date el-icon--left"></i>课程数据</p>
                                <el-table :data="pageData.courseOverview" border size="mini" :summary-method="getSummaries" show-summary style="width: 100%; margin-top: 20px">
                                    <el-table-column prop="courseCategoryName" label="课程名称" align="center" width="70">
                                    </el-table-column>
                                    <el-table-column prop="totalCourseCount" label="总课时数" align="center" min-width="70">
                                    </el-table-column>
                                    <el-table-column prop="totalRestCourseCount" label="剩余课时数" align="center" min-width="90">
                                    </el-table-column>
                                    <el-table-column prop="totalTuition" label="缴费金额" align="center" min-width="70">
                                        <!-- <template slot-scope='scope'>
                                                 <span style="color:#f56767;font-weight:600">{{ scope.row.totalTuition }}</span> 
                                        </template> -->
                                    </el-table-column>
                                </el-table>
                            </div>
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
                    <el-select v-model="teacherDialog.baseInfo.teacherStatus" placeholder="请选择学生状态">
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
                <el-form-item label="身份证号码">
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
                <el-form-item label="报名时间">
                    <el-date-picker v-model="teacherDialog.baseInfo.teacherRegisterDate" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="teacherDialog.isShow = false">取 消</el-button>
                    <el-button size="small" type="primary" @click="submitUpdateStudent('baseInfo')">提 交</el-button>
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
                    token: '123456798',
                    teacherCode: '',
                    teacherName: ''
                },
                uploadUrl: '/api/teacher/uploadavatar',
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
                    teacherStatus: "",
                    teacherStatusDesc: ""
                },
                coursePackageList: [],
                courseOverview: []
            },
            packageDialog: {
                width: '520px',
                isShow: false,
                title: '添加课程套餐',
                labelPosition: 'right',
                formLabelWidth: '100px',
                uploadPanel: 'N',
                courseInfo: {
                    selectedPackage: [],
                    selectedFolder: '',
                    actualCourseCount: 0,
                    isDiscount: 'N',
                    actualPrice: 0,
                    isPayed: 'N',
                    selectedPaymentType: '',
                    payeeCode: '',
                    payDate: ''
                },
                payeeList: [],
                coursePackage: [],
                payPattern: [],
                courseFolder: [],
            },
            teacherDialog: {
                width: '600px',
                isShow: false,
                title: '更新学生基本信息',
                labelPosition: 'right',
                formLabelWidth: '90px',
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
        _this.GetStudentBaseData();
    },
    methods: {
        setAvatarShow() {
            this.avatarPanel.isShow = !this.avatarPanel.isShow;
            this.avatarPanel.params.teacherCode = this.pageData.teacherInfo.teacherCode;
            this.avatarPanel.params.teacherName = this.pageData.teacherInfo.teacherName;
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
        /**
         * upload fail
         *
         * [param] status    server api return error status, like 500
         * [param] field
         */
        cropUploadFail(status, field) {
            console.log('-------- upload fail --------');
            console.log(status);
            console.log('field: ' + field);
        },

        GetStudentBaseData() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/teacher/getbaseinfo',
                data: {
                    teacherCode: _this.teacherCode
                },
                fn: function (result) {
                    result.teacherInfo.teacherBirthday = result.teacherInfo.teacherBirthday.split('T')[0];
                    result.teacherInfo.teacherRegisterDate = result.teacherInfo.teacherRegisterDate.split('T')[0];
                    result.teacherInfo.teacherStatusDesc = dicHelper.getLabelByValue(_this.$store.getters['teacher_status'], result.teacherInfo.teacherStatus);
                    _this.avatarPanel.imgDataUrl = result.teacherInfo.teacherAvatarPath;
                    _this.pageData = result;

                }
            });
        },

        DelPackage() {
            var _this = this;
            this.$confirm('是否确定删除该套餐? 如果已经排课，排课信息将一并删除！', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/teacher/removecoursepackage',
                    data: {
                        id: _this.packageDialog.courseInfo.id
                    },
                    fn: function (result) {
                        if (result === 200) {
                            _this.GetStudentBaseData();
                            _this.$message({
                                message: '删除成功',
                                type: 'success'
                            });
                            _this.packageDialog.isShow = false;
                        } else if (result === 201) {
                            _this.$message({
                                message: '学生已经开始上课，不能删除此套餐！',
                                type: 'warning'
                            });

                            let child = document.getElementsByClassName("v-modal")[0];
                            child.parentNode.removeChild(child);
                        }
                    }
                });
            }).catch(() => {
                let child = document.getElementsByClassName("v-modal")[0];
                child.parentNode.removeChild(child);
            });
        },

        showAddCoursePackage() {
            this.packageDialog.uploadPanel = 'N';
            this.packageDialog.courseInfo = {
                selectedPackage: [],
                selectedFolder: '',
                isDiscount: 'N',
                actualCourseCount: 0,
                actualPrice: 0,
                isPayed: 'N',
                selectedPaymentType: '',
                payeeCode: '',
                payDate: ''
            }
            this.packageDialog.isShow = true;
        },
        submitAddPackage() {
            var _this = this;
            var cascader = _this.packageDialog.courseInfo.selectedPackage;
            if (cascader.length == 0) {
                _this.$message({
                    message: '请选择套餐类型',
                    type: 'warning'
                });
                return;
            }

            if (_this.packageDialog.courseInfo.actualCourseCount == 0) {
                _this.$message({
                    message: '请填写实际课程数目',
                    type: 'warning'
                });
                return;
            }

            var folderCode = _this.packageDialog.courseInfo.selectedFolder,
                folderName = _this.GetLabelByValue(_this.packageDialog.courseFolder, folderCode);

            if (!folderCode) {
                _this.$message({
                    message: '请选择课程内容',
                    type: 'warning'
                });
                return;
            }

            var teacherCode = _this.packageDialog.courseInfo.payeeCode,
                teacherName = _this.GetLabelByValue(_this.packageDialog.payeeList, teacherCode);

            var patternCode = _this.packageDialog.courseInfo.selectedPaymentType,
                patternName = _this.GetLabelByValue(_this.packageDialog.payPattern, patternCode);

            // 添加新的套餐
            var newPackage = {
                StudentCode: _this.teacherCode,
                StudentName: _this.pageData.teacherInfo.teacherName,
                PackageCode: cascader[1],
                CourseFolderCode: folderCode,
                CourseFolderName: folderName,
                IsDiscount: _this.packageDialog.courseInfo.isDiscount,
                IsPayed: _this.packageDialog.courseInfo.isPayed,
                ActualCourseCount: _this.packageDialog.courseInfo.actualCourseCount,
                ActualPrice: _this.packageDialog.courseInfo.actualPrice,
                PayeeCode: teacherCode,
                PayeeName: teacherName,
                PayPatternCode: patternCode,
                PayPatternName: patternName,
                PayDate: _this.packageDialog.courseInfo.payDate || '1900-01-01'
            }

            axios({
                type: 'post',
                path: '/api/teacher/postnewpackage',
                data: newPackage,
                fn: function (result) {
                    if (result === 200) {
                        _this.GetStudentBaseData();
                        _this.$message({
                            message: '添加课程套餐成功',
                            type: 'success'
                        });
                        _this.packageDialog.isShow = false;
                    }
                }
            });
        },


        showUpdateCoursePackage(row) {
            this.packageDialog.uploadPanel = 'Y';
            this.packageDialog.courseInfo = {
                id: row.id,
                selectedPackage: [row.courseCategoryCode, row.packageCode],
                selectedFolder: row.courseFolderCode,
                isDiscount: row.isDiscount,
                actualCourseCount: row.actualCourseCount,
                actualPrice: row.actualPrice,
                isPayed: row.isPayed,
                selectedPaymentType: row.payPatternCode,
                payeeCode: row.payeeCode,
                payDate: row.payDate.split('T')[0]
            }

            this.packageDialog.isShow = true;
        },
        UpdatePackage() {
            var _this = this;

            var folderCode = _this.packageDialog.courseInfo.selectedFolder,
                folderName = _this.GetLabelByValue(_this.packageDialog.courseFolder, folderCode);

            var teacherCode = _this.packageDialog.courseInfo.payeeCode,
                teacherName = _this.GetLabelByValue(_this.packageDialog.payeeList, teacherCode);

            var patternCode = _this.packageDialog.courseInfo.selectedPaymentType,
                patternName = _this.GetLabelByValue(_this.packageDialog.payPattern, patternCode);

            var updatePackage = {
                IsDiscount: _this.packageDialog.courseInfo.isDiscount,
                IsPayed: _this.packageDialog.courseInfo.isPayed,
                ActualPrice: _this.packageDialog.courseInfo.actualPrice,
                PayeeCode: teacherCode,
                PayeeName: teacherName,
                PayPatternCode: patternCode,
                PayPatternName: patternName,
                PayDate: _this.packageDialog.courseInfo.payDate || '1900-01-01'
            }

            axios({
                type: 'put',
                path: '/api/teacher/updateteacherpackage/' + _this.packageDialog.courseInfo.id,
                data: updatePackage,
                fn: function (result) {
                    if (result === 200) {
                        _this.GetStudentBaseData();
                        _this.$message({
                            message: '修改课程套餐成功',
                            type: 'success'
                        });
                        _this.packageDialog.isShow = false;
                    }
                }
            });
        },

        showUpdateStudent() {
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

        submitUpdateStudent(teacherForm) {
            var _this = this;
            this.$refs[teacherForm].validate((valid) => {
                if (valid) {
                    var teacher = Object.assign({}, _this.teacherDialog.baseInfo);
                    axios({
                        type: 'put',
                        path: '/api/teacher/updateteacher/' + _this.teacherCode,
                        data: teacher,
                        fn: function (result) {
                            if (result === 200) {
                                _this.GetStudentBaseData();
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

        handleCourseFolderDisplay(item) {
            // 控制课程小类的是否可选
            if (this.packageDialog.courseInfo.selectedPackage.length == 0) {
                return true;
            } else if (item.value.indexOf(this.packageDialog.courseInfo.selectedPackage[0]) > -1) {
                return false;
            } else {
                if (this.packageDialog.courseInfo.selectedFolder == item.value) {
                    this.packageDialog.courseInfo.selectedFolder = '';
                }
                return true;
            }
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
        },
        getSummaries(param) {
            const {
                columns,
                data
            } = param;
            const sums = [];
            columns.forEach((column, index) => {
                if (index === 0) {
                    sums[index] = '合计';
                    return;
                }
                const values = data.map(item => Number(item[column.property]));
                if (!values.every(value => isNaN(value))) {
                    sums[index] = values.reduce((prev, curr) => {
                        const value = Number(curr);
                        if (!isNaN(value)) {
                            return prev + curr;
                        } else {
                            return prev;
                        }
                    }, 0);
                    sums[index] += '';
                } else {
                    sums[index] = 'N/A';
                }
            });

            return sums;
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
