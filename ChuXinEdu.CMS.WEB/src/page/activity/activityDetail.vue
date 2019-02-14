<template>
<div class="createPost-container">
    <el-form class="form-container" size="small" :model="postForm" :rules="rules" ref="postForm">

        <sticky :className="'sub-navbar '+ postForm.status">
            <el-button v-noRepeatClick size="small" type="danger" @click="draftForm">删除</el-button>
            <el-button v-noRepeatClick size="small" type="success" @click="submitForm">保存
            </el-button>
            <el-button v-noRepeatClick size="small" type="success" @click="submitForm">保存并返回列表
            </el-button>
            <el-button v-noRepeatClick size="small" type="warning" @click="draftForm">取消</el-button>
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
                                    <el-date-picker value-format="yyyy-MM-dd" size="small" class="date-rang-small" :editable="false" v-model="postForm.activityDate" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期">
                                    </el-date-picker>
                                </el-form-item>
                            </el-col>

                            <el-col :span="10">
                                <el-form-item label-width="80px" label="销课课时:" class="postInfo-container-item">
                                    <el-input-number v-model="postForm.courseCount" :min="0" size="mini"></el-input-number> 节
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </div>
                </el-col>
            </el-row>

            <el-form-item label-width="80px" prop="address" label="活动地点:">
                <el-input class="article-textarea" size="small" v-model="postForm.address">
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
                <Tinymce :height=400 ref="editor" v-model="postForm.content" />
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
    activityId: undefined,
    activitySubject: '',
    content: '',
    address: '',
    activityDate: '', 
    courseCount: 0,
    status: 'draft'
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
            }
            else {
                callback();
            }
        }
        return {
            selStudent:{
                rawList: [],
                options: [],
                selected: []
            },
            studentActivityList: [],
            postForm: Object.assign({}, defaultForm),
            rules: {
                activitySubject: [{
                    validator: validateRequire, trigger: 'blur'                    
                }],
                activityDate: [{
                    validator: validateRequire, trigger: 'change'
                }],
                address: [{
                    validator: validateRequire, trigger: 'blur'
                }]
            }
        }
    },
    computed: {
        addressLength() {
            return this.postForm.address.length
        }
    },
    created() {
        this.getActivieStudents();
        if (this.isEdit) {
          const activityId = this.$route.params && this.$route.params.activityId
          this.fetchActivityData(activityId)
        } else {
          this.postForm = Object.assign({}, defaultForm)
        }
    },
    methods: {
        getActivieStudents () {
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
                path: '/api/config/getactivestudent',
                fn: result => {
                    this.selStudent.rawList = result
                }
            });
        },
        submitForm() {
            this.$refs['postForm'].validate(valid => {
                if (valid) {
                    console.log(this.postForm)
                    this.$notify({
                        title: '成功',
                        message: '保存成功',
                        type: 'success',
                        duration: 2000
                    })
                    this.postForm.status = 'published'
                } else {
                    return false
                }
            })
        },
        draftForm() {
            if (this.postForm.activitySubject.length === 0) {
                this.$message({
                    message: '请填写必要的标题和内容',
                    type: 'warning'
                })
                return
            }
            this.$message({
                message: '保存成功',
                type: 'success',
                showClose: true,
                duration: 1000
            })
            this.postForm.status = 'draft'
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
                for (let s of this.selStudent.rawList){
                    if(s.value == item) {
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
            this.studentActivityList = this.studentActivityList.filter( item => {
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
