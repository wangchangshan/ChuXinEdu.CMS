<template>
<div class="list_container">
    <div class="table_container">
        <el-table :data="roleList" v-loading="loading" size="mini" align="left" border stripe :max-height="tableHeight">
            <el-table-column type="index" align='center' width="40"></el-table-column>
            <el-table-column prop="itemName" label="角色名称" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="itemCode" label="角色代码" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="itemDesc" label="描述" align='center' min-width="150">
            </el-table-column>
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="125">
                <template slot-scope='scope'>
                    <el-button type="primary" icon='edit' size="small" @click='roleManage(scope.row)'>管 理</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>

    <el-dialog :title="roleDialog.title" :visible.sync="roleDialog.isShow" :width="roleDialog.width" :close-on-press-escape='false' :modal-append-to-body="false">
        <div style="text-align: center">
            <el-transfer style="text-align: left; display: inline-block" v-model="roleDialog.teacherInRoleList" filterable :titles="roleDialog.transferTitles" :button-texts="['移除', '添加']" :format="{
        noChecked: '${total}',
        hasChecked: '${checked}/${total}'
      }" @change="handleTransferChange" :data="roleDialog.teacherList">
            </el-transfer>
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
            roleList: [],
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 150,

            curRoleCode: '',
            roleDialog: {
                width: '700px',
                isShow: false,
                title: '',
                transferTitles: [],
                teacherList:[],
                teacherInRoleList:[],
            }
        }
    },
    created() {
        this.getRoleList();
    },
    methods: {
        getRoleList() {
            axios({
                type: 'get',
                path: '/api/config/getroles',
                fn: result => {
                    this.roleList = result;
                }
            });
        },

        roleManage(row) {
            this.roleDialog.teacherList = [];
            this.roleDialog.teacherInRoleList = [];
            this.roleDialog.title = "角色【" + row.itemName + "】配置";
            this.roleDialog.transferTitles = ["可选教师列表", row.itemName];
            this.roleDialog.isShow = true;
            this.curRoleCode = row.itemCode;
            // 获取教师列表
            axios({
                type: 'get',
                path: '/api/teacher/getteacherwithrole/' + row.itemCode,
                fn: result => {
                    result.forEach(item => {
                        this.roleDialog.teacherList.push({
                            key: item.teacher_code,
                            label: item.teacher_name,
                            disabled: false
                        });
                        if(row.itemCode == item.role_code) {
                            this.roleDialog.teacherInRoleList.push(item.teacher_code);
                        }
                    })
                }
            });
        },

        handleTransferChange(value, direction, movedKeys) {
            // console.log(value, direction, movedKeys);
            if(direction == "right") {
                // 添加角色
                this.AddTeacherRole(movedKeys);
            }
            else {
                // 从角色中移除
                this.RemoveTeacherRole(movedKeys);
            }
        },

        AddTeacherRole(movedTeachers) {
            axios({
                type: 'post',
                path: '/api/config/addteacherrole/' + this.curRoleCode,
                data: movedTeachers,
                fn: result => {}
            });
        },

        RemoveTeacherRole(movedTeachers) {
            axios({
                type: 'post',
                path: '/api/config/removeteacherrole/' + this.curRoleCode,
                data: movedTeachers,
                fn: result => {}
            });
        }
    }
}
</script>

<style lang="less" scoped>
.list_container {
    overflow-y: hidden;
}
</style>
