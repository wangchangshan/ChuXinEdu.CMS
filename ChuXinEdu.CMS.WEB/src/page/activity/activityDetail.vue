<template>
<div class="createPost-container">
    <el-form class="form-container" size="small" :model="postForm" :rules="rules" ref="postForm">

        <sticky :className="'sub-navbar '+ status">
            <el-button v-noRepeatClick v-if="activityId != 0" size="small" type="danger" @click="deleteForm">删除</el-button>
            <el-button v-noRepeatClick size="small" type="success" @click="submitForm(false)">保存
            </el-button>
            <el-button v-noRepeatClick size="small" type="success" @click="submitForm(true)">保存并返回列表
            </el-button>
            <router-link :to="'/activity'">
                <el-button v-noRepeatClick size="small" type="warning" >取消</el-button>
            </router-link>
        </sticky>

        <div class="createPost-main-container">
            <el-row>
                <el-col :span="24">
                    <el-form-item style="margin-left: 10px" prop="activitySubject">
                        <MDinput name="name" v-model="postForm.activitySubject" required :maxlength="200">
                            活动主题
                        </MDinput>
                    </el-form-item>
                    <div class="postInfo-container">
                        <el-row>
                            <el-col :span="14">
                                <el-form-item label-width="80px" prop="activityDate" label="活动时间:" class="postInfo-container-item">
                                    <el-date-picker v-model="postForm.activityDate" type="daterange" value-format="yyyy-MM-dd" size="small" class="date-rang-small" :editable="false" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期">
                                    </el-date-picker>
                                </el-form-item>
                            </el-col>

                            <el-col :span="10">
                                <el-form-item label-width="80px" label="销课课时:" class="postInfo-container-item">
                                    <el-input-number v-model="postForm.activityCourseCount" :min="0" size="mini"></el-input-number> 节
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </div>
                </el-col>
            </el-row>

            <el-form-item label-width="80px" prop="activityAddress" label="活动地点:">
                <el-input class="article-textarea" size="small" v-model="postForm.activityAddress">
                </el-input>
                <span class="word-counter" v-show="addressLength">{{addressLength}}字</span>
            </el-form-item>

            <div class="postInfo-container">
                <el-row>
                    <el-col :span="7">
                        <el-form-item label-width="80px" label="参加学生:" class="postInfo-container-item">
                            <el-select v-model="selStudent.selected" multiple collapse-tags @change="selectStudentChanged" filterable="" remote="" reserve-keyword placeholder="请输入学生姓名" :remote-method="getRemoteStudent">
                                <el-option v-for="item in selStudent.options" :key="item.value" :label="item.label" :value="item.value">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :span="17">
                        <el-tag v-for="student in studentActivityList" :key="student.studentCode" closable style="margin-left:3px" size="small" @close="removeStudentTag(student.studentCode)"> {{student.studentName}}</el-tag>
                    </el-col>
                </el-row>
            </div>

            <div class="editor-container">
                <Tinymce :height=400 ref="editor" v-model="postForm.activityContent" />
            </div>
        </div>
    </el-form>

</div>
</template>

<script>
import {
    axios
} from '@/utils/index'
import Tinymce from '@/components/Tinymce'
import MDinput from '@/components/MDinput'
import Sticky from '@/components/Sticky' // 粘性header组件
import {
    validateURL
} from '@/utils/validate'

const defaultForm = {
    activitySubject: '',
    activityAddress: '',
    activityDate: '',
    activityFromDate: '',
    activityToDate: '',
    activityCourseCount: 0,
    activityContent: ''
}

export default {
    name: 'activityDetail',
    components: {
        Tinymce,
        MDinput,
        Sticky
    },
    props: {
        isEdit: {
            type: Boolean,
            default: false
        }
    },
    data() {
        const validateRequire = (rule, value, callback) => {
            if (value === '' || value == undefined) {
                callback('必填项');
            } else {
                callback();
            }
        }
        return {
            status: 'edit',
            selStudent: {
                rawList: [],
                options: [],
                selected: []
            },
            studentActivityList: [],
            activityId: 0,
            postForm: Object.assign({}, defaultForm),
            rules: {
                activitySubject: [{
                    validator: validateRequire,
                    trigger: 'blur'
                }],
                activityDate: [{
                    validator: validateRequire,
                    trigger: ['change']
                }],
                activityAddress: [{
                    validator: validateRequire,
                    trigger: 'blur'
                }]
            }
        }
    },
    computed: {
        addressLength() {
            return this.postForm.activityAddress.length
        }
    },
    created() {
        this.getActivieStudents();
        if (this.isEdit) {
            const activityId = this.$route.params && this.$route.params.activityId
            this.activityId = activityId;
            this.fetchActivityData(activityId)
        } else {
            this.postForm = Object.assign({}, defaultForm)
        }
    },
    methods: {
        getActivieStudents() {
            axios({
                type: 'get',
                path: '/api/config/getactivestudent',
                fn: result => {
                    this.selStudent.rawList = result
                }
            });
        },
        fetchActivityData(activityId) {
            axios({
                type: 'get',
                path: '/api/activity/' + activityId,
                fn: result => {
                    this.postForm.activitySubject = result.activitySubject
                    this.postForm.activityAddress = result.activityAddress                    
                    this.postForm.activityDate = [result.activityFromDate.split('T')[0],result.activityToDate.split('T')[0]]
                    this.postForm.activityCourseCount = result.activityCourseCount
                    this.postForm.activityContent = result.activityContent
                }
            });
        },
        submitForm(jump2list) {
            this.$refs['postForm'].validate(valid => {
                if (valid) {
                    this.postForm.activityFromDate = this.postForm.activityDate[0];
                    this.postForm.activityToDate = this.postForm.activityDate[1];
                    axios({
                        type: 'post',
                        path: '/api/activity/' + this.activityId,
                        data: this.postForm,
                        fn: result => {
                            if (result.code == 1200) {
                                this.activityId = result.id;
                                this.$route.params.activityId = result.id;
                                this.$message({
                                    message: '保存成功',
                                    type: 'success'
                                });
                                if(jump2list){
                                    this.$router.push('/activity')
                                }
                            } else {
                                this.$message({
                                    message: '保存失败',
                                    type: 'error'
                                });
                            }
                        }
                    });
                    this.status = 'published'
                } else {
                    return false
                }
            })
        },
        deleteForm() {
            this.$confirm('确定删除当前活动吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/activity/' + this.activityId,
                    fn: result => {
                        if (result == 1200) {
                            this.$message({
                                message: '删除活动成功！',
                                type: 'success'
                            });
                            this.$router.push("/activity")
                        }else {
                            this.$message({
                                message: '删除活动失败！',
                                type: 'error'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        getRemoteStudent(query) {
            if (query !== '') {
                setTimeout(() => {
                    this.selStudent.options = this.selStudent.rawList.filter(item => {
                        return item.label.toLowerCase()
                            .indexOf(query.toLowerCase()) > -1;
                    });
                }, 200);
            } else {
                this.selStudent.options = [];
            }
        },

        selectStudentChanged() {
            this.studentActivityList = this.selStudent.selected.map(item => {
                let studentName = 'NONE'
                for (let s of this.selStudent.rawList) {
                    if (s.value == item) {
                        studentName = s.label;
                        break;
                    }
                }
                return {
                    activityId: 0,
                    studentCode: item,
                    studentName: studentName
                }
            });
        },

        removeStudentTag(val) {
            this.studentActivityList = this.studentActivityList.filter(item => {
                return item.studentCode != val;
            })
            this.selStudent.selected.splice(this.selStudent.selected.indexOf(val), 1);
        }
    }
}
</script>

<style lang="scss" scoped>
@import "src/style/mixin.scss";

.createPost-container {
    position: relative;

    .createPost-main-container {
        padding: 0px 25px 0 20px;

        .postInfo-container {
            position: relative;
            @include clearfix;

            .postInfo-container-item {
                float: left;
            }
        }

        .editor-container {
            min-height: 500px;
            margin: 0 0 0 5px;

            .editor-upload-btn-container {
                text-align: right;
                margin-right: 10px;

                .editor-upload-btn {
                    display: inline-block;
                }
            }
        }
    }

    .word-counter {
        width: 40px;
        position: absolute;
        right: -10px;
        top: 0px;
    }
}
</style>
