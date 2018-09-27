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
                    <p>姓名：{{ BaseInfo.studentInfo.studentName }}</p>
                    <p>性别：{{ BaseInfo.studentInfo.studentSex }}</p>
                    <p>出生日期：{{ BaseInfo.studentInfo.studentBirthday }}</p>
                    <p>入学日期：{{ BaseInfo.studentInfo.studentRegisterDate }}</p>
                    <p>联系电话：{{ BaseInfo.studentInfo.studentPhone }}</p>
                    <p>身份证号：{{ BaseInfo.studentInfo.studentIdentityCardNum }}</p>
                    <p>家庭住址：{{ BaseInfo.studentInfo.studentAddress }}</p>
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
                            <p class="num">{{ BaseInfo.totalCourseCount }}</p>
                            <p class="title">总课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">{{ BaseInfo.restCourseCount }}</p>
                            <p class="title">剩余课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">￥{{ BaseInfo.totalTuition }}</p>
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
                <el-table :data="BaseInfo.coursePackageList" stripe border style="width: 100%" size="mini">
                    <el-table-column prop="packageName" align='center' label="课程套餐" min-width="200">
                    </el-table-column>
                    <el-table-column prop="" align='center' label="课程类别" min-width="160">
                        <template slot-scope='scope'>
                            {{ scope.row.courseCategoryName  + "/" + scope.row.courseFolderName }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="payDate" align='center' label="缴费日期" min-width="110">
                        <template slot-scope='scope'>
                            {{ scope.row.payDate && scope.row.payDate.split('T')[0] }}
                        </template>
                    </el-table-column>
                    <el-table-column prop="actualPrice" align='center' label="缴费金额（元）" min-width="110">
                    </el-table-column>
                    <el-table-column prop="payPattern" align='center' label="缴费方式" min-width="110">
                    </el-table-column>
                    <el-table-column prop="payeeName" align='center' label="收款人" min-width="120">
                    </el-table-column>
                    <el-table-column prop="operation" align='center' label="操作" fixed="right" width="120">
                        <template slot-scope='scope'>
                            <el-button type="primary" icon="el-icon-edit" size="mini" @click='updateStudentPackage()'>更新</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <div style="margin-top:10px">
                    <el-button type="success" icon="el-icon-plus" size="small" @click="packageDialog.isShow = true">添加课程套餐</el-button>
                </div>
            </template>
        </el-col>
    </el-row>

    <el-dialog :title="packageDialog.title" :width="packageDialog.width" :visible.sync="packageDialog.isShow" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="courseInfo" :model="packageDialog.courseInfo" :label-width="packageDialog.formLabelWidth" :label-position='packageDialog.labelPosition' style="margin:10px;width:auto;" size="mini">
                <el-form-item label="课程类型">
                    <el-cascader :options="packageDialog.coursePackage" v-model="packageDialog.courseInfo.selectedPackage" size="mini" style="width:350px"></el-cascader>
                </el-form-item>
                <el-form-item label="课程内容">
                    <el-checkbox-group v-model="packageDialog.courseInfo.selectedFolder">
                        <el-checkbox v-for="item in packageDialog.courseFolder" :key="item.value" :label="item.value" :disabled="handleCourseFolderDisplay(item)">{{item.label}}</el-checkbox>
                    </el-checkbox-group>
                </el-form-item>
                <el-form-item prop="isPayed" label="是否缴费">
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
                        <el-option v-for="item in packageDialog.payPattern" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="缴费日期" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-date-picker v-model="packageDialog.courseInfo.payDate" type="date" value-format="yyyy-MM-dd" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="收款人" v-show="packageDialog.courseInfo.isPayed == 'Y'">
                    <el-select v-model="packageDialog.courseInfo.payeeCode" placeholder="请选择">
                        <el-option v-for="item in packageDialog.payeeList" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item class="text_center">
                    <el-button @click="packageDialog.isShow = false" size="small">取 消</el-button>
                    <el-button type="primary" size="small" @click="submitPackage()">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    name: 'student-base-info',
    props: {
        'studentCode': String,
    },
    data() {
        return {
            BaseInfo: {
                "totalCourseCount": 0,
                "restCourseCount": 0,
                "totalTuition": 0,
                "studentInfo": {
                    "studentCode": "",
                    "studentName": "",
                    "studentSex": "",
                    "studentBirthday": "",
                    "studentIdentityCardNum": "",
                    "studentPhone": "",
                    "studentPropagateType": "",
                    "studentPropagateTxt": "",
                    "studentRegisterDate": "",
                    "studentAddress": "",
                    "studentAvatarPath": "",
                },
                "coursePackageList": []
            },
            packageDialog: {
                width: '520px',
                isShow: false,
                title: '添加课程套餐',
                labelPosition: 'right',
                formLabelWidth: '90px',
                courseInfo: {
                    selectedPackage: [],
                    selectedFolder: [],
                    isDiscount: 'N',
                    actualPrice: 0,
                    isPayed: '',
                    selectedPaymentType: '',
                    payeeCode: '',
                    payDate: ''
                },
                payeeList: [],
                coursePackage: [],
                payPattern: [],
                courseFolder: [],
            }
        }
    },
    created() {
        var _this = this;
        // 获取学生所有基本信息
        axios({
            type: 'get',
            path: '/api/student/getbaseinfo',
            data: {
                studentCode: _this.studentCode
            },
            fn: function (result) {
                result.studentInfo.studentBirthday = result.studentInfo.studentBirthday.split('T')[0];
                result.studentInfo.studentRegisterDate = result.studentInfo.studentRegisterDate.split('T')[0];
                _this.BaseInfo = result;
            }
        });

        // 获取付款方式
        axios({
            type: 'get',
            path: '/api/config/getdicbycode',
            data: {
                typeCode: 'pay_pattern'
            },
            fn: function (result) {
                _this.packageDialog.payPattern = result;
            }
        });

        // 获取课程小类
        axios({
            type: 'get',
            path: '/api/config/getdicbycode',
            data: {
                typeCode: 'course_folder'
            },
            fn: function (result) {
                _this.packageDialog.courseFolder = result;
            }
        });

        // 获取所有课程套餐
        axios({
            type: 'get',
            path: '/api/config/getcoursepackage',
            fn: function (result) {
                _this.packageDialog.coursePackage = result;
            }
        });

        // 获取收费教师
        axios({
            type: 'get',
            path: '/api/teacher/getteachers',
            fn: function (result) {
                _this.packageDialog.payeeList = result;
            }
        });
    },
    methods: {
        handleCourseFolderDisplay(item) {
            // 控制课程小类的是否可选
            //console.log(this.courseInfo.selected_course); // 存储的是label属性
            if (this.packageDialog.courseInfo.selectedPackage.length == 0) {
                return true;
            } else if (item.value.indexOf(this.packageDialog.courseInfo.selectedPackage[0]) > -1) {
                return false;
            } else {
                let index = this.packageDialog.courseInfo.selectedFolder.indexOf(item.value);
                if (index > -1) {
                    this.packageDialog.courseInfo.selectedFolder.splice(index, 1);
                }
                return true;
            }
        },

        submitPackage() {
            var _this = this;
            var folderCode = _this.packageDialog.courseInfo.selectedFolder[0],
                folderName = _this.GetLabelByValue(_this.packageDialog.courseFolder, folderCode);

            var teacherCode = _this.packageDialog.courseInfo.payeeCode,
                teacherName = _this.GetLabelByValue(_this.packageDialog.payeeList, teacherCode);

            // 添加新的套餐
            var newPackage = {
                StudentCode: _this.studentCode,
                StudentName: _this.BaseInfo.studentInfo.studentName,
                PackageCode: _this.packageDialog.courseInfo.selectedPackage[1],
                CourseFolderCode: folderCode,
                CourseFolderName: folderName,
                IsDiscount: _this.packageDialog.courseInfo.isDiscount,
                IsPayed: _this.packageDialog.courseInfo.isPayed,
                ActualPrice: _this.packageDialog.courseInfo.actualPrice,
                PayeeCode: teacherCode,
                PayeeName: teacherName,
            }
            console.log(newPackage);
            axios({
                type: 'post',
                path: '/api/student/postnewpackage',
                data: newPackage,
                fn: function (result) {
                    //
                }
            });
        },

        updateStudentPackage() {
            alert('修改是否收费信息')
        },

        GetLabelByValue(lst, value){
            let label = '';
            for(let obj of lst) {
                if(obj['value'] == value) {
                    label = obj['label'];
                    break;
                }
            }
            return label;
        }
    }
}
</script>

<style lang="less" scoped>
.info_container {
    padding: 0;
    margin-top: -20px;
    overflow-x: hidden;
}

.row {
    margin: 20px;
}

.info_row {
    .area {
        border: 1px solid #dfdfdf;
        height: 210px;
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
