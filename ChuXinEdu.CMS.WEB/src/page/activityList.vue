<template>
<div class="fillcontain">
    <div class="table_container">
        <el-table :data="activityList" v-loading="loading" style="width: 100%" align="center" size="mini" :max-height="tableHeight">
            <el-table-column type="expand">
                <template slot-scope="props">
                    <el-form label-position="left" inline class="demo-table-expand">
                        <el-form-item label="参加学生：">
                            <span>{{ props.row.activity_attend_students }}</span>
                        </el-form-item>
                    </el-form>
                </template>
            </el-table-column>
            <el-table-column prop="activity_title" label="活动主题" align='left' min-width="200">
            </el-table-column>
            <el-table-column prop="activity_from_date" label="活动时间" align='center' width="200">
            </el-table-column>
            <el-table-column prop="activity_address" label="活动地点" align='center' min-width="200">
            </el-table-column>
            
            <el-table-column prop="operation" align='center' label="操作" fixed="right" width="180">
                <template slot-scope='scope'>
                    <el-button type="success" icon='edit' size="small" @click='showStudentDetail()'>查看详细</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination v-if="paginations.total > 0" :page-sizes="paginations.page_sizes" :page-size="paginations.page_size" :layout="paginations.layout" :total="paginations.total" :current-page="paginations.current_page_index" @current-change='handleCurrentChange'
                        @size-change='handleSizeChange'>

                    </el-pagination>
                </div>
            </el-col>
        </el-row>
    </div>
    <el-dialog :title="dialog.title" :visible.sync="dialog.show" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="studentInfo" :model="studentInfo" :rules="studentInfoRules" :label-width="dialog.formLabelWidth" :label-position='dialog.labelPosition' style="margin:10px;width:auto;">
                <el-form-item label="姓名">
                    <el-input v-model="studentInfo.student_name"></el-input>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="studentInfo.student_sex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="出生日期">
                    <el-date-picker v-model="studentInfo.student_birthday" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item label="身份证号码">
                    <el-input v-model="studentInfo.student_identity_card_num"></el-input>
                </el-form-item>
                <el-form-item label="联系电话">
                    <el-input v-model="studentInfo.student_phone"></el-input>
                </el-form-item>
                <el-form-item label="家庭地址">
                    <el-input v-model="studentInfo.student_address"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input type="textarea" v-model="studentInfo.student_remark"></el-input>
                </el-form-item>
                <el-form-item label="报名时间">
                    <el-date-picker v-model="studentInfo.student_register_date" type="date" placeholder="选择日期"> </el-date-picker>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button @click="dialog.show = false">取 消</el-button>
                    <el-button type="primary">提 交</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
export default {
    data() {
        return {
            activityList: [{
                    activity_id: '201807001',
                    activity_title: '奥林匹克森林公园写生',
                    activity_from_date: '2008-06-19',
                    activity_to_date: '2008-06-20',
                    activity_address: '北京市昌平区天巢园小区',
                    activity_attend_students: '胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，'
                },
                {
                    activity_id: '201807001',
                    activity_title: '夜宿海洋馆活动',
                    activity_from_date: '2008-06-19',
                    activity_to_date: '2008-06-20',
                    activity_address: '北京市昌平区天巢园小区',
                    activity_attend_students: '胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，'
                },
                {
                    activity_id: '201807001',
                    activity_title: '中国国家美术馆展览',
                    activity_from_date: '2008-06-19',
                    activity_to_date: '2008-06-20',
                    activity_address: '北京市昌平区天巢园小区',
                    activity_attend_students: '胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，胡小平，'
                },
            ],
            searchField: {
                student_name: ''
            },
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 63,
            search_form_rules: {
                student_name: [{
                    required: false,
                    message: '学生姓名不能为空',
                    trigger: 'blur'
                }]
            },
            paginations: {
                current_page_index: 1,
                total: 3,
                page_size: 15,
                page_sizes: [10, 15, 20, 30],
                layout: "total, sizes, prev, pager, next, jumper" // 翻页属性
            },
            dialog: {
                width: '500px',
                show: false,
                title: '添加学生',
                labelPosition: 'right',
                formLabelWidth: '120px'
            },
            studentInfo: {

            },
            studentInfoRules: {

            }
        }
    },
    methods: {
        courseTag(course) {
            let basic = '';
            switch (course) {
                case '国画':
                    basic = 'success'
                    break;
                case '西画':
                    basic = ''
                    break;
                case '书法':
                    basic = 'info'
                    break;
                default:
                    basic = 'danger'
            }
            return basic;
        },
        handleSizeChange(val) {
            console.log(`每页 ${val} 条`);
        },
        handleCurrentChange(val) {
            console.log(`当前页: ${val}`);
        },
        addStudent() {
            this.dialog.title = '新增学生';
            this.dialog.show = true;
        },
        showStudentDetail() {
            alert('待开发')
        },
        searchStudent() {
            alert('待开发')
        }
    }
}
</script>

<style lang="less" scoped>
.btnRight {
    float: right;
    margin-right: 10px !important;
}

.search_container {
    height: 36px;
    line-height: 36px;
}

.search-form {
    width: 100%;
    min-width: 750px;
}

.pagination {
    text-align: left;
    margin-top: 10px
}
</style>
