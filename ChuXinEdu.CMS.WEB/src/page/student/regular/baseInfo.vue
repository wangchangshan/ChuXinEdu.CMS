<template>
<div class="info_container">
    <el-row class="info_row row" :gutter="10" style="margin:10px">
        <el-container>
            <el-aside width="220px">
                <div class="avatar-panel">
                    <my-avatar field="img" @crop-success="cropSuccess" @crop-upload-success="cropUploadSuccess" @crop-upload-fail="cropUploadFail" v-model="avatarPanel.isShow" :width="200" :height="200" :url="avatarPanel.uploadUrl" :params="avatarPanel.params" img-format="png">
                    </my-avatar>
                    <img :src="avatarPanel.imgDataUrl"/>
                    <el-button type="success" size="mini" @click="setAvatarShow">设置头像<i class="el-icon-upload el-icon--right"></i></el-button>
                </div>
            </el-aside>
            <el-main style="padding:10px; padding-bottom:0">
                <el-row :gutter="10">
                    <el-col :xs="18" :sm="12" :lg="12" style="min-width:250px">
                        <el-card shadow="never" class="card-student-base">
                            <el-form :label-width="studentDialog.formLabelWidth" :label-position='studentDialog.labelPosition' size="mini" label-suffix="：">
                                <el-form-item label="姓名">
                                    {{ pageData.studentInfo.studentName }} &nbsp;&nbsp; <el-tag size="medium" :type="studentStatusTag(pageData.studentInfo.studentStatus)">{{ pageData.studentInfo.studentStatusDesc}}</el-tag>
                                </el-form-item>
                                <el-form-item label="性别">
                                    {{ pageData.studentInfo.studentSex }}
                                </el-form-item>
                                <el-form-item label="出生日期">
                                    {{ pageData.studentInfo.studentBirthday }}
                                </el-form-item>
                                <el-form-item label="报名日期">
                                    {{ pageData.studentInfo.studentRegisterDate }}
                                </el-form-item>
                                <el-form-item label="联系电话">
                                    {{ pageData.studentInfo.studentPhone }}
                                </el-form-item>
                                <el-form-item label="家庭住址">
                                    {{ pageData.studentInfo.studentAddress }}
                                </el-form-item>
                                <el-form-item label="备注">
                                    {{ pageData.studentInfo.studentRemark }}
                                </el-form-item>
                                <el-form-item>
                                    <el-button type="primary" @click="showUpdateStudent()"><i class="el-icon-edit el-icon--left"></i>编 辑</el-button>
                                </el-form-item>
                            </el-form>
                        </el-card>
                    </el-col>
                    <el-col :xs="6" :sm="12" :lg="12" >
                        <el-card shadow="never" class="card-student-course">
                            <div class="dataarea">
                                <p class="gtitle"><i class="el-icon-date el-icon--left"></i>课程数据</p>
                                <el-table :data="pageData.courseOverview" border size="mini" :summary-method="getSummaries" show-summary style="width: 100%; margin-top: 20px">
                                    <el-table-column prop="courseCategoryName" label="类别" align="center" width="60">
                                    </el-table-column>
                                    <el-table-column prop="totalCourseCount" label="总课时" align="center" min-width="60">
                                    </el-table-column>
                                    <el-table-column prop="totalRestCourseCount" label="剩余课时" align="center" min-width="70">
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
    <el-row class="info_row row" :gutter="10">
        <el-col :span="24">
            <template>
                <el-table :data="pageData.coursePackageList" stripe border style="width: 100%" size="mini">
                    <el-table-column prop="" align='left' label="课程套餐" min-width="210">
                        <template slot-scope='scope'>
                            <i class="el-icon-success" :style="{color: scope.row.restCourseCount==0 ?'#67C23A' :''}"></i>
                            {{ scope.row.packageName }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="" align='center' label="课程类别" min-width="90">
                        <template slot-scope='scope'>
                            {{ scope.row.courseCategoryName  + "/" + scope.row.courseFolderName }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="actualCourseCount" align='center' label="总课时" min-width="70">
                    </el-table-column>
                    <el-table-column prop="restCourseCount" align='center' label="剩余课时" min-width="70">
                    </el-table-column>
                    <el-table-column prop="payDate" align='center' label="缴费日期" min-width="100">
                        <template slot-scope='scope'>
                            {{ scope.row.payDate && scope.row.payDate.split('T')[0] }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="actualPrice" align='center' label="缴费金额" min-width="70">
                        <template slot-scope='scope'>
                            <span style="color:#f56767;">{{ scope.row.actualPrice }}</span> 
                        </template>
                    </el-table-column>
                    <el-table-column prop="payeeName" align='center' label="收款人" min-width="70">
                    </el-table-column>
                    <el-table-column prop="operation" align='center' label="操作" fixed="right" width="110">
                        <template slot-scope='scope'>
                            <el-button type="warning" icon="el-icon-edit" size="mini" @click='showUpdateCoursePackage(scope.row)'>更新</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <div style="margin-top:10px">
                    <el-button type="success" v-if="pageData.studentInfo.studentStatus == '01'" icon="el-icon-plus" size="small" @click="showAddCoursePackage()">添加课程套餐</el-button>
                </div>
            </template>
        </el-col>
    </el-row>

    <el-dialog :title="packageDialog.title" :width="packageDialog.width" :visible.sync="packageDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="courseInfo" :model="packageDialog.courseInfo" :label-width="packageDialog.formLabelWidth" :label-position='packageDialog.labelPosition' style="margin:10px;width:auto;" size="mini">
                <el-form-item label="课程类型">
                    <el-cascader :disabled="packageDialog.uploadPanel == 'Y'" @change="packageChanged" :options="packageDialog.coursePackage" v-model="packageDialog.courseInfo.selectedPackage" size="mini" style="width:350px"></el-cascader>
                </el-form-item>
                <el-form-item label="课程内容">
                    <el-radio-group :disabled="packageDialog.uploadPanel == 'Y'" v-model="packageDialog.courseInfo.selectedFolder">
                        <el-radio v-for="item in packageDialog.folders" :key="item.value" :label="item.value">{{item.label}}</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="实际课程数目">
                    <el-input-number v-model="packageDialog.courseInfo.actualCourseCount" :disabled="packageDialog.uploadPanel == 'Y'" :min="0"></el-input-number>
                </el-form-item>
                <el-form-item label="是否缴费">
                    <el-radio-group v-model="packageDialog.courseInfo.isPayed">
                        <el-radio label="Y">是</el-radio>
                        <el-radio label="N">否</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="是否优惠" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-radio-group v-model="packageDialog.courseInfo.isDiscount">
                        <el-radio label="Y">是</el-radio>
                        <el-radio label="N">否</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="实际价格" v-show="packageDialog.courseInfo.isPayed == 'Y' && packageDialog.courseInfo.isDiscount == 'Y'">
                    <el-input-number v-model="packageDialog.courseInfo.actualPrice" :min="0"></el-input-number>
                </el-form-item>

                <el-form-item label="缴费类型" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-select v-model="packageDialog.courseInfo.selectedPaymentType" placeholder="请选择">
                        <el-option v-for="item in $store.getters['pay_pattern']" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="缴费日期" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-date-picker v-model="packageDialog.courseInfo.payDate" :editable="false" type="date" value-format="yyyy-MM-dd" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="收款人" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-select v-model="packageDialog.courseInfo.payeeCode" placeholder="请选择">
                        <el-option v-for="item in packageDialog.payeeList" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item style="text-align:right; margin-right:20px">
                    <el-button @click="packageDialog.isShow = false" size="small">取 消</el-button>
                    <el-button v-noRepeatClick v-show="packageDialog.uploadPanel == 'N'" type="primary" size="small" @click="submitAddPackage()">提 交</el-button>
                    <el-button v-noRepeatClick v-show="packageDialog.uploadPanel == 'Y'" type="danger" size="small" @click="DelPackage()">删 除</el-button>
                    <el-button v-noRepeatClick v-show="packageDialog.uploadPanel == 'Y'" type="primary" size="small" @click="UpdatePackage()">保 存</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>

    <el-dialog :title="studentDialog.title" :width="studentDialog.width" :visible.sync="studentDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="baseInfo" :model="studentDialog.baseInfo" :rules="studentDialog.baseInfoRules" :label-width="studentDialog.formLabelWidth" :label-position='studentDialog.labelPosition' size="mini">
                <el-form-item label="姓名">
                    <el-input v-model="studentDialog.baseInfo.studentName"></el-input>
                    <el-select v-model="studentDialog.baseInfo.studentStatus" placeholder="请选择学生状态">
                        <el-option v-for="item in $store.getters['student_status']" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="studentDialog.baseInfo.studentSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="studentDialog.baseInfo.studentBirthday" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="身份证号码">
                    <el-input v-model="studentDialog.baseInfo.studentIdentityCardNum"></el-input>
                </el-form-item>
                <el-form-item prop="studentPhone" label="联系电话">
                    <el-input v-model="studentDialog.baseInfo.studentPhone"></el-input>
                </el-form-item>
                <el-form-item prop="studentAddress" label="家庭地址">
                    <el-input v-model="studentDialog.baseInfo.studentAddress"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="studentDialog.baseInfo.studentRemark"></el-input>
                </el-form-item>
                <el-form-item label="报名时间">
                    <el-date-picker v-model="studentDialog.baseInfo.studentRegisterDate" :editable="false" value-format="yyyy-MM-dd" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item style="text-align:right">
                    <el-button size="small" @click="studentDialog.isShow = false">取 消</el-button>
                    <el-button v-noRepeatClick size="small" type="primary" @click="submitUpdateStudent('baseInfo')">提 交</el-button>
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
    name: 'student-base-info',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            avatarPanel: {
                isShow: false,
                params: {
                    type: 'student',
                    token: '123456798',
                    code: '',
                    name: ''
                },
                uploadUrl: '/api/upload/uploadavatar',
                imgDataUrl: ''
            },
            pageData: {
                studentInfo: {
                    studentCode: "",
                    studentName: "",
                    studentSex: "",
                    studentBirthday: "",
                    studentIdentityCardNum: "",
                    studentPhone: "",
                    studentPropagateType: "",
                    studentPropagateTxt: "",
                    studentRegisterDate: "",
                    studentAddress: "",
                    studentAvatarPath: "",
                    studentStatus: "",
                    studentStatusDesc: ""
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
                folders:[],
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
            },
            studentDialog: {
                width: '600px',
                isShow: false,
                title: '更新学生基本信息',
                labelPosition: 'right',
                formLabelWidth: '90px',
                baseInfo: {
                    studentName: "",
                    studentSex: "",
                    studentBirthday: "",
                    studentIdentityCardNum: "",
                    studentPhone: "",
                    studentAddress: "",
                    student_introduce: "",
                    studentRemark: "",
                    studentRegisterDate: "",
                    studentStatus: "",
                },
                baseInfoRules: {
                    studentPhone: [{
                        required: true,
                        message: '请输入联系电话',
                        trigger: 'blur'
                    }],
                    studentAddress: [{
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
        this.GetStudentBaseData();

        // 获取所有课程套餐
        axios({
            type: 'get',
            path: '/api/config/getcoursepackage',
            fn: result => {
                this.packageDialog.coursePackage = result;
            }
        });

        // 获取收费教师
        axios({
            type: 'get',
            path: '/api/config/getfinancer',
            fn: result => {
                this.packageDialog.payeeList = result;
            }
        });
    },
    methods: {
        setAvatarShow() {
            this.avatarPanel.isShow = !this.avatarPanel.isShow;
            this.avatarPanel.params.code = this.pageData.studentInfo.studentCode;
            this.avatarPanel.params.name = this.pageData.studentInfo.studentName;
        },

        cropSuccess(imgDataUrl, field) {
            //console.log('-------- crop success --------');
            this.avatarPanel.imgDataUrl = imgDataUrl;
        },
        cropUploadSuccess(jsonData, field) {
            // console.log('-------- upload success --------');
            // console.log(jsonData);
            // console.log('field: ' + field);
        },
        /**
         * upload fail
         *
         * [param] status    server api return error status, like 500
         * [param] field
         */
        cropUploadFail(status, field) {
        },

        GetStudentBaseData() {
            axios({
                type: 'get',
                path: '/api/student/getbaseinfo',
                data: {
                    studentCode: this.studentCode
                },
                fn: result => {
                    result.studentInfo.studentBirthday = result.studentInfo.studentBirthday && result.studentInfo.studentBirthday.split('T')[0] || '';
                    result.studentInfo.studentRegisterDate = result.studentInfo.studentRegisterDate.split('T')[0];
                    result.studentInfo.studentStatusDesc = dicHelper.getLabelByValue(this.$store.getters['student_status'], result.studentInfo.studentStatus);
                    this.avatarPanel.imgDataUrl = result.studentInfo.studentAvatarPath;
                    this.pageData = result;

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
                    path: '/api/student/removecoursepackage',
                    data: {
                        id: _this.packageDialog.courseInfo.id
                    },
                    fn: function (result) {
                        if (result === 1200) {
                            _this.GetStudentBaseData();
                            _this.$message({
                                message: '删除成功',
                                type: 'success'
                            });
                            _this.packageDialog.isShow = false;
                        } else if (result === 1600) {
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
                folderName = dicHelper.getLabelByValue(_this.$store.getters['course_folder'], folderCode);

            if (!folderCode) {
                _this.$message({
                    message: '请选择课程内容',
                    type: 'warning'
                });
                return;
            }

            var teacherCode = _this.packageDialog.courseInfo.payeeCode,
                teacherName = dicHelper.getLabelByValue(_this.packageDialog.payeeList, teacherCode);

            var patternCode = _this.packageDialog.courseInfo.selectedPaymentType,
                patternName = dicHelper.getLabelByValue(_this.$store.getters['pay_pattern'], patternCode);

            // 添加新的套餐
            var newPackage = {
                StudentCode: _this.studentCode,
                StudentName: _this.pageData.studentInfo.studentName,
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
                path: '/api/student/postnewpackage',
                data: newPackage,
                fn: function (result) {
                    if (result === 1200) {
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
                folderName = dicHelper.getLabelByValue(_this.$store.getters['course_folder'], folderCode);

            var teacherCode = _this.packageDialog.courseInfo.payeeCode,
                teacherName = dicHelper.getLabelByValue(_this.packageDialog.payeeList, teacherCode);

            var patternCode = _this.packageDialog.courseInfo.selectedPaymentType,
                patternName = dicHelper.getLabelByValue(_this.$store.getters['pay_pattern'], patternCode);
                
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
                path: '/api/student/updatestudentpackage/' + _this.packageDialog.courseInfo.id,
                data: updatePackage,
                fn: function (result) {
                    if (result === 1200) {
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
            this.studentDialog.isShow = true;
            this.studentDialog.baseInfo = {
                studentName: this.pageData.studentInfo.studentName,
                studentSex: this.pageData.studentInfo.studentSex,
                studentBirthday: this.pageData.studentInfo.studentBirthday,
                studentIdentityCardNum: this.pageData.studentInfo.studentIdentityCardNum,
                studentPhone: this.pageData.studentInfo.studentPhone,
                studentAddress: this.pageData.studentInfo.studentAddress,
                studentRemark: this.pageData.studentInfo.studentRemark,
                studentRegisterDate: this.pageData.studentInfo.studentRegisterDate,
                studentStatus: this.pageData.studentInfo.studentStatus
            };
        },

        submitUpdateStudent(studentForm) {
            var _this = this;
            this.$refs[studentForm].validate((valid) => {
                if (valid) {
                    var student = Object.assign({}, _this.studentDialog.baseInfo);
                    axios({
                        type: 'put',
                        path: '/api/student/updatestudent/' + _this.studentCode,
                        data: student,
                        fn: function (result) {
                            if (result === 1200) {
                                _this.GetStudentBaseData();
                                _this.$message({
                                    message: '更新基础信息成功',
                                    type: 'success'
                                });
                                _this.studentDialog.isShow = false;
                            }
                        }
                    });
                }
            });

        },

        packageChanged() {
           if (this.packageDialog.courseInfo.selectedPackage.length == 0) {
                this.packageDialog.folders = [];
            } else{
                let pCode = this.packageDialog.courseInfo.selectedPackage[0];
                this.packageDialog.folders = this.$store.getters['course_folder'].filter(item => {return item.value.indexOf(pCode + '_') > -1})
            }
        },

        studentStatusTag(studentStatusCode) {
            return tagTypeHelper.studentStatusTag(studentStatusCode);
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
        margin-top: 10px;

        img {
            width: 200px;
            height: 200px;
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
