<template>
<div class="list_container">
    <div class="table_container">
        <el-table :data="templateList" v-loading="loading" size="mini" align="left" border stripe :height="tableHeight">
            <el-table-column type="index" align='center' width="40"></el-table-column>
            <el-table-column prop="arrangeTemplateName" label="排课模板名称" align='center' min-width="220">
            </el-table-column>
            <el-table-column prop="arrangeTemplateCode" label="模板代码" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="templateEnabled" label="是否启用" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="primary" icon='edit' size="small" @click='templateManage(scope.row)'>管 理</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <div class="footer_container">
        <el-button type="success" icon="el-icon-plus" size="small" @click="showAddArrangeTemplate()">添加排课模板</el-button>
    </div>

    <el-dialog :title="templateDialog.title" :visible.sync="templateDialog.isShow" :width="templateDialog.width" :close-on-press-escape='false' :modal-append-to-body="false">
        <el-form :inline="true" ref="templateBase" :model="templateDialog" :rules="templateDialog.templateDialogRules" class="demo-form-inline" size="mini" label-suffix='：' style="margin-right:20px">
            <el-form-item prop="templateName" label="模板名称">
                <el-input v-model="templateDialog.templateName"></el-input>
            </el-form-item>
            <el-form-item label="是否启用">
                <el-switch
                    v-model="templateDialog.templateEnabled"
                    active-color="#13ce66"
                    active-value="Y"
                    inactive-value="N">
                </el-switch>

            </el-form-item>
        </el-form>
        <el-table :data="templateDialog.periodList" size="mini" align="left" border stripe :max-height="templateDialog.height">
            <el-table-column property="dayName" label="星期" align='center' width="120">
            </el-table-column>
            <el-table-column property="coursePeriod" label="课程时间段" align='left'>
                <template slot-scope='scope'>
                    <div v-for="period in scope.row.coursePeriod" v-bind:key="period.id">
                        <el-time-select class="time-mini" placeholder="上课时间" v-model="period.startTime" :picker-options="{
                                    start: '09:00',
                                    step: '00:30',
                                    end: '20:00'
                            }" size="mini">
                        </el-time-select>
                        <el-time-select class="time-mini" placeholder="下课时间" v-model="period.endTime" :picker-options="{
                                    start: '09:00',
                                    step: '00:30',
                                    end: '20:00',
                                    minTime: period.startTime
                            }" size="mini">
                        </el-time-select>
                        <el-button type="success" icon="el-icon-plus" size="mini" circle @click='createNewPeriod(scope.row.dayCode)'></el-button>
                        <el-button type="danger" icon="el-icon-minus" size="mini" circle @click='removeLine(scope.row.dayCode, period.id)'></el-button>
                    </div>
                </template>
            </el-table-column>
            <el-table-column label="休息？" align='center' width="90">
                <template slot-scope='scope'>
                    <el-switch @change="setDayFlag(scope.row.dayCode)" v-model="scope.row.isActive" active-color="#13ce66" active-value="N" inactive-value="Y">
                    </el-switch>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer_container" style="text-align:center;margin-top:10px">
            <el-button size="small" @click="resetForm('templateBase')">取 消</el-button>
            <el-button v-if="templateDialog.type == 'edit'" v-noRepeatClick size="small" type="danger" @click="btnDelEvent()">删 除</el-button>
            <el-button v-noRepeatClick size="small" type="primary" @click="btnSubmitEvent('templateBase')">确 定</el-button>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    axios
} from '@/utils/index'

export default {
    data() {
        return {
            templateList: [],
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 100,

            curRoleCode: '',
            templateDialog: {
                width: '600px',
                height: 400,
                isShow: false,
                title: '模板信息',
                templateName: '',
                templateEnabled: 'Y',
                type: 'edit',
                templateCode: '',
                templateDialogRules: {
                    templateName: [{
                        required: true,
                        message: '请填写排课模板名称',
                        trigger: 'blur'
                    }],
                },
                periodList: [{
                    dayCode: 'day1',
                    dayName: '星期一',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day2',
                    dayName: '星期二',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day3',
                    dayName: '星期三',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day4',
                    dayName: '星期四',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day5',
                    dayName: '星期五',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day6',
                    dayName: '星期六',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, {
                    dayCode: 'day7',
                    dayName: '星期日',
                    isActive: 'Y',
                    coursePeriod: [{
                        id: Math.random().toString(),
                        startTime: '',
                        endTime: ''
                    }]
                }, ]
            }
        }
    },
    created() {
        this.getTemplateList();
    },
    methods: {
        getTemplateList() {
            this.loading = true;
            axios({
                type: 'get',
                path: '/api/arrangetemplate/getarrangetemplates',
                fn: result => {
                    this.templateList = result;
                    this.loading = false;
                }
            });
        },

        resetForm(form){
            this.$refs[form].resetFields();
            this.templateDialog.templateName = '';
            this.templateDialog.isShow = false;
        },

        templateManage(row) {
            this.templateDialog.type = 'edit';
            this.templateDialog.templateCode = row.arrangeTemplateCode;
            this.templateDialog.templateName = row.arrangeTemplateName;
            this.templateDialog.templateEnabled = row.templateEnabled;
            this.templateDialog.isShow = true;
            this.getTemplateDetail(row.arrangeTemplateCode);
        },

        getTemplateDetail(templateCode) {
            axios({
                type: 'get',
                path: '/api/arrangetemplate/getarrangetemplatedetail/' + templateCode,
                fn: result => {
                    for (let i = 0; i < 7; i++) {
                        this.templateDialog.periodList[i].coursePeriod = [];
                    }
                    result.forEach(item => {
                        let index = parseInt(item.courseWeekDay.charAt(3)) - 1;
                        this.templateDialog.periodList[index].coursePeriod.push({
                            id: Math.random().toString(),
                            startTime: item.coursePeriod.split('-')[0],
                            endTime: item.coursePeriod.split('-')[1]
                        });
                    });
                }
            });
        },

        createNewPeriod(dayCode) {
            let index = parseInt(dayCode.charAt(3)) - 1;
            this.templateDialog.periodList[index].coursePeriod.push({
                id: Math.random().toString(),
                startTime: '',
                endTime: ''
            })
        },

        removeLine(dayCode, periodId) {
            let index = parseInt(dayCode.charAt(3)) - 1;
            let arr = this.templateDialog.periodList[index].coursePeriod;
            if (arr.length == 1 && this.templateDialog.periodList[index].isActive == 'Y') {
                this.$message({
                    type: 'warning',
                    message: '请保留至少一个时间段！'
                });
            } else {
                this.templateDialog.periodList[index].coursePeriod = arr.filter(item => item.id != periodId);
            }
        },

        showAddArrangeTemplate() {
            for (let i = 0; i < 7; i++) {
                this.templateDialog.periodList[i].coursePeriod = [{
                    id: Math.random().toString(),
                    startTime: '',
                    endTime: ''
                }];
            }
            this.templateDialog.templateCode = '';
            this.templateDialog.templateName = '';
            this.templateDialog.templateEnabled = 'Y';
            this.templateDialog.type = 'add';
            this.templateDialog.isShow = true;
        },

        btnSubmitEvent(form) {
            this.$refs[form].validate((valid) => {
                if (valid) {
                    this.templateSubmit();
                }
            });
        },

        btnDelEvent() {
            this.$confirm('确定删除这个排课模板吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/arrangetemplate/deltemplate/' + this.templateDialog.templateCode,
                    fn: result => {
                        if (result === 1200) {
                            this.getTemplateList();
                            this.$message({
                                message: '删除成功！',
                                type: 'success'
                            });
                            this.templateDialog.isShow = false;
                        }
                        else if(result === 1600 ){
                            this.$message({
                                message: '当前模板正在使用，不能直接删除！',
                                type: 'warning'
                            });
                        }
                        else
                        {
                            this.$message({
                                message: '删除排课模板失败！',
                                type: 'error'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        templateSubmit(){
            let listTemplateDetails = [];
            let templateCode = this.templateDialog.templateCode;
            for (let i = 0; i < 7; i++) {
                let dayCode = this.templateDialog.periodList[i].dayCode;
                this.templateDialog.periodList[i].coursePeriod.forEach(item => {
                    if(item.startTime && item.endTime){
                        listTemplateDetails.push({
                            arrangeTemplateCode: templateCode,
                            coursePeriod: item.startTime + '-' + item.endTime,
                            courseWeekDay: dayCode
                        });
                    }
                })
            }

            let template = {
                templateCode: templateCode,
                templateName: this.templateDialog.templateName,
                templateEnabled: this.templateDialog.templateEnabled,
                details: listTemplateDetails
            }
            if (this.templateDialog.type == 'add') {
                axios({
                    type: 'post',
                    path: '/api/arrangetemplate/addnewtemplate',
                    data: template,
                    fn: result => {
                        if (result === 1200) {
                            this.getTemplateList();
                            this.$message({
                                message: '添加排课模板成功',
                                type: 'success'
                            });
                            this.templateDialog.isShow = false;
                        }
                        else{
                            this.$message({
                                message: '添加排课模板失败',
                                type: 'error'
                            });
                        }
                    }
                });
            } else if (this.templateDialog.type == 'edit') {
                axios({
                    type: 'post',
                    path: '/api/arrangetemplate/updatetemplate',
                    data: template,
                    fn: result => {
                        if (result === 1200) {
                            this.getTemplateList();
                            this.$message({
                                message: '更新排课模板成功',
                                type: 'success'
                            });
                            this.templateDialog.isShow = false;
                        }
                    }
                });
            } else {
                this.$message({
                    message: '无法添加或更新排课模板，请联系系统管理人员',
                    type: 'warning'
                });
            }
        },

        setDayFlag(dayCode) {
            let index = parseInt(dayCode.charAt(3)) - 1;
            let isActive = this.templateDialog.periodList[index].isActive;
            if (isActive == 'N') {
                this.templateDialog.periodList[index].coursePeriod = [];
            } else {
                this.templateDialog.periodList[index].coursePeriod = [{
                    id: Math.random().toString(),
                    startTime: '',
                    endTime: ''
                }];
            }
        }

    }
}
</script>

<style lang="less" scoped>
.list_container {
    overflow-y: hidden;
}

.footer_container {
    height: 36px;
    line-height: 36px;
    text-align: left;
}
</style>
