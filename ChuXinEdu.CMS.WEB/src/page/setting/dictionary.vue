<template>
<div class="list_container">
    <div class="table_container">
        <el-table :data="dicTypeList" v-loading="loading" size="mini" align="left" border stripe :height="tableHeight">
            <el-table-column type="index" align='center' width="40"></el-table-column>
            <el-table-column prop="typeName" label="字典类别名称" align='center' min-width="220">
            </el-table-column>
            <el-table-column prop="typeCode" label="字典类别名编码" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="primary" icon='edit' size="small" @click='showEditDicPanel(scope.row)'>管 理</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <div class="footer_container" style="text-align: center">
        <el-button type="success" icon="el-icon-plus" size="small" @click="showAddDicPanel()">添加字典</el-button>
    </div>

    <el-dialog :title="dicItemDialog.title" :visible.sync="dicItemDialog.isShow" :width="dicItemDialog.width" :close-on-press-escape='false' :modal-append-to-body="false">
        <el-form :inline="true" ref="dicBase" :model="dicItemDialog" :rules="dicItemDialog.dicRules" class="demo-form-inline" size="mini" label-suffix='：' style="margin-right:20px">
            <el-form-item prop="typeName" label="类别名称">
                <el-input v-model="dicItemDialog.typeName"></el-input>
            </el-form-item>
            <el-form-item prop="typeCode" label="类别编码">
                <el-input v-model="dicItemDialog.typeCode" :disabled="dicItemDialog.panelType == 'edit'"></el-input>
            </el-form-item>
        </el-form>

        <el-table :data="dicItemDialog.itemList" size="mini" align="left" border stripe :height="dicItemDialog.height">
            <el-table-column property="itemName" label="名称" align='left'>
                <template slot-scope='scope'>
                    <el-input size="mini" v-model="scope.row.itemName"></el-input>
                </template>
            </el-table-column>
            <el-table-column property="itemCode" label="编码" align='left'>
                <template slot-scope='scope'>
                    <el-input size="mini" v-model="scope.row.itemCode"></el-input>
                </template>
            </el-table-column>
            <el-table-column property="itemSortWeight" label="排序" align='left'>
                <template slot-scope='scope'>
                    <el-input size="mini" v-model="scope.row.itemSortWeight"></el-input>
                </template>
            </el-table-column>
            <el-table-column label="启用？" align='center' width="90">
                <template slot-scope='scope'>
                    <el-switch v-model="scope.row.itemEnabled" active-color="#13ce66" active-value="Y" inactive-value="N">
                    </el-switch>
                </template>
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="100">
                <template slot-scope='scope'>
                    <el-button type="success" icon="el-icon-plus" size="mini" circle @click='addItem()'></el-button>
                    <el-button type="danger" icon="el-icon-minus" size="mini" circle @click='removeItem(scope.row.id)'></el-button>
                </template>
            </el-table-column>
        </el-table>
        <div class="footer_container" style="text-align:center;margin-top:10px">
            <el-button size="small" @click="resetForm('dicBase')">取 消</el-button>
            <el-button v-if="dicItemDialog.panelType == 'edit'" v-noRepeatClick size="small" type="danger" @click="btnDelEvent()">删 除</el-button>
            <el-button v-noRepeatClick size="small" type="primary" @click="btnSubmitEvent('dicBase')">确 定</el-button>
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
            allDicList: [],
            dicTypeList: [],
            dicItemList: [],
            loading: true,
            tableHeight: this.$store.state.page.win_content.height - 100,

            dicItemDialog: {
                width: '600px',
                height: 400,
                isShow: false,
                title: '详细信息',
                panelType: 'edit',
                typeName: '',
                typeCode: '',
                itemList: [{
                    id: Math.random().toString(),
                    itemCode: '',
                    itemName: '',
                    itemSortWeight: '',
                    itemEnabled: 'Y'
                }],
                typeCode: '',
                dicRules: {
                    typeName: [{
                        required: true,
                        message: '请填写字典类别名称',
                        trigger: 'blur'
                    }],
                    typeCode: [{
                        required: true,
                        message: '请填写字典类别编码',
                        trigger: 'blur'
                    }],
                }
            }
        }
    },
    created() {
        this.getDicList();
    },
    methods: {
        getDicList() {
            this.loading = true;
            axios({
                type: 'get',
                path: '/api/dictionary/getdictionarys',
                fn: result => {
                    this.allDicList = result;
                    this.dicTypeList = [];
                    let typesFlag = [];
                    result.forEach(item => {
                        if (typesFlag.indexOf(item.typeCode) == -1) {
                            this.dicTypeList.push({
                                typeCode: item.typeCode,
                                typeName: item.typeName
                            });
                            typesFlag.push(item.typeCode)
                        }
                    })
                    this.loading = false;
                }
            });
        },

        resetForm(form) {
            this.$refs[form].resetFields();
            this.dicItemDialog.typeName = '';
            this.dicItemDialog.typeCode = '';
            this.dicItemDialog.isShow = false;
        },

        showEditDicPanel(row) {
            this.dicItemDialog.panelType = 'edit';
            this.dicItemDialog.typeCode = row.typeCode;
            this.dicItemDialog.typeName = row.typeName;
            this.dicItemDialog.isShow = true;
            
            this.dicItemDialog.itemList = this.allDicList.filter(item => item.typeCode == row.typeCode);
        },

        addItem() {
            this.dicItemDialog.itemList.push({
                id: Math.random().toString(),
                itemCode: '',
                itemName: '',
                itemSortWeight: '',
                itemEnabled: 'Y'
            })
        },

        removeItem(id) {
            if (this.dicItemDialog.itemList.length == 1) {
                this.$message({
                    type: 'warning',
                    message: '请保留至少一条记录！'
                });
            } else {
                this.dicItemDialog.itemList = this.dicItemDialog.itemList.filter(item => item.id != id);
            }
        },

        showAddDicPanel() {
            this.dicItemDialog.typeCode = '';
            this.dicItemDialog.typeName = '';
            this.dicItemDialog.panelType = 'add';
            this.dicItemDialog.isShow = true;
            this.dicItemDialog.itemList = [{
                    id: Math.random().toString(),
                    itemCode: '',
                    itemName: '',
                    itemSortWeight: '',
                    itemEnabled: 'Y'
                }]
        },

        btnSubmitEvent(form) {
            this.$refs[form].validate((valid) => {
                if (valid) {
                    this.templateSubmit();
                }
            });
        },

        templateSubmit() {
            let listDicItems = [];
            let listItemCodes = [];
            let typeCode = this.dicItemDialog.typeCode;
            let typeName = this.dicItemDialog.typeName;

            for(let item of this.dicItemDialog.itemList) {
                if(listItemCodes.indexOf(item.itemCode) > -1) {
                    this.$message({
                        message: '字典编码重复，请修改后重新提交',
                        type: 'error'
                    });
                    return;
                }
                listItemCodes.push(item.itemCode);
                let itemkey = 0;
                if((item.id + '').split('.').length == 1){
                    itemkey = item.id;
                }
                listDicItems.push({
                    id: itemkey,
                    typeCode: typeCode,
                    typeName: typeName,
                    itemCode: item.itemCode,
                    itemName: item.itemName,
                    isParent: 'N',
                    itemSortWeight: item.itemSortWeight,
                    itemEnabled: item.itemEnabled
                });
            }
            if (this.dicItemDialog.panelType == 'add') {
                axios({
                    type: 'post',
                    path: '/api/dictionary/addnewdic',
                    data: listDicItems,
                    fn: result => {
                        if (result === 1200) {
                            this.getDicList();
                            this.$message({
                                message: '添加字典成功',
                                type: 'success'
                            });
                            this.dicItemDialog.isShow = false;
                        }
                        else if(result === 1600) {
                            this.$message({
                                message: '字典类别编码重复，请修改后重新提交',
                                type: 'error'
                            });
                        }
                        else {
                            this.$message({
                                message: '添加字典失败',
                                type: 'error'
                            });
                        }
                    }
                });
            } else if (this.dicItemDialog.panelType == 'edit') {
                axios({
                    type: 'post',
                    path: '/api/dictionary/updatedic',
                    data: listDicItems,
                    fn: result => {
                        if (result === 1200) {
                            this.getDicList();
                            this.$message({
                                message: '更新字典成功',
                                type: 'success'
                            });
                            this.dicItemDialog.isShow = false;
                        }
                        else if (result === 1401) {
                            this.$message({
                                message: '您没有权限修改字典项，请联系系统管理员',
                                type: 'warning'
                            });
                        } 
                        else {
                            this.$message({
                                message: '更新排课模板失败',
                                type: 'error'
                            });
                        }
                    }
                });
            } else {
                this.$message({
                    message: '无法添加或更新字典，请联系系统管理人员',
                    type: 'warning'
                });
            }
        },

        btnDelEvent() {
            this.$confirm('确定删除当前字典吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/dictionary/deldic/' + this.dicItemDialog.typeCode,
                    fn: result => {
                        if (result === 1200) {
                            this.getDicList();
                            this.$message({
                                message: '删除成功！',
                                type: 'success'
                            });
                            this.dicItemDialog.isShow = false;
                        } else if (result === 1401) {
                            this.$message({
                                message: '您没有权限删除字典项，请联系系统管理员',
                                type: 'warning'
                            });
                        } else {
                            this.$message({
                                message: '删除字典失败！',
                                type: 'error'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },
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
