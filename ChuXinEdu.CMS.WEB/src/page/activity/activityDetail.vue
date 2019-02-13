<template>
<div class="createPost-container">
    <el-form class="form-container" size="small" :model="postForm" :rules="rules" ref="postForm">

        <sticky :className="'sub-navbar '+ postForm.status">
            <el-button size="small" v-loading="loading" type="success" @click="submitForm">发布
            </el-button>
            <el-button size="small" v-loading="loading" type="warning" @click="draftForm">草稿</el-button>
        </sticky>

        <div class="createPost-main-container">
            <el-row>
                <el-col :span="24">
                    <el-form-item style="margin-left: 10px" prop="title">
                        <MDinput name="name" v-model="postForm.title" required :maxlength="200">
                            活动主题
                        </MDinput>
                    </el-form-item>
                    <div class="postInfo-container">
                        <el-row>
                            <el-col :span="14">
                                <el-form-item label-width="80px" label="活动时间:" class="postInfo-container-item">
                                    <el-date-picker size="small" class="date-rang-small" v-model="postForm.display_time" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期">
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

            <el-form-item label-width="80px" label="活动地点:">
                <el-input class="article-textarea" size="small" v-model="postForm.content_short">
                </el-input>
                <span class="word-counter" v-show="contentShortLength">{{contentShortLength}}字</span>
            </el-form-item>

            <div class="postInfo-container">
                <el-row>
                    <el-col :span="7">
                        <el-form-item label-width="80px" label="参加学生:" class="postInfo-container-item">
                            <el-select v-model="value9" multiple collapse-tags @change="selectStudentChanged" filterable="" remote="" reserve-keyword placeholder="请输入学生姓名" :remote-method="remoteMethod" :loading="loading">
                                <el-option v-for="item in options4" :key="item.value" :label="item.label" :value="item.value">
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
import Tinymce from '@/components/Tinymce'
import MDinput from '@/components/MDinput'
// import Multiselect from 'vue-multiselect'// 使用的一个多选框组件，element-ui的select不能满足所有需求
// import 'vue-multiselect/dist/vue-multiselect.min.css'// 多选框组件css
import Sticky from '@/components/Sticky' // 粘性header组件
import {
    validateURL
} from '@/utils/validate'

const defaultForm = {
    status: 'draft',
    title: '', // 文章题目
    content: '', // 文章内容
    content_short: '', // 文章摘要
    image_uri: '', // 文章图片
    display_time: undefined, // 前台展示时间
    id: undefined,
    platforms: ['a-platform'],
    comment_disabled: false,
    courseCount: 0
}

export default {
    name: 'articleDetail',
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
            if (value === '') {
                this.$message({
                    message: rule.field + '为必填项',
                    type: 'error'
                })
                callback('活动主题为必填项')
            } else {
                //callback()
            }
        }
        const validateSourceUri = (rule, value, callback) => {
            if (value) {
                if (validateURL(value)) {
                    callback()
                } else {
                    this.$message({
                        message: '外链url填写不正确',
                        type: 'error'
                    })
                    callback(null)
                }
            } else {
                callback()
            }
        }
        return {
            options4: [],
            value9: [],
            list: [{ value: '01', label: '许小念' },{ value: '02', label: '秦若雯' },{ value: '03', label: '王浩然' }],
            loading: true,
            studentActivityList: [],
            postForm: Object.assign({}, defaultForm),
            loading: false,
            userListOptions: [],
            rules: {
                image_uri: [{
                    validator: validateRequire
                }],
                title: [{
                    validator: validateRequire
                }],
                content: [{
                    validator: validateRequire
                }]
            }
        }
    },
    computed: {
        contentShortLength() {
            return this.postForm.content_short.length
        }
    },
    created() {
        // if (this.isEdit) {
        //   const id = this.$route.params && this.$route.params.id
        //   this.fetchData(id)
        // } else {
        //   this.postForm = Object.assign({}, defaultForm)
        // }
    },
    methods: {
        fetchData(id) {
            fetchArticle(id).then(response => {
                this.postForm = response.data
                // Just for test
                this.postForm.title += `   Article Id:${this.postForm.id}`
                this.postForm.content_short += `   Article Id:${this.postForm.id}`
            }).catch(err => {
                console.log(err)
            })
        },
        submitForm() {
            this.postForm.display_time = parseInt(this.display_time / 1000)
            console.log(this.postForm)
            this.$refs.postForm.validate(valid => {
                if (valid) {
                    this.loading = true
                    this.$notify({
                        title: '成功',
                        message: '发布文章成功',
                        type: 'success',
                        duration: 2000
                    })
                    this.postForm.status = 'published'
                    this.loading = false
                } else {
                    console.log('error submit!!')
                    return false
                }
            })
        },
        draftForm() {
            if (this.postForm.content.length === 0 || this.postForm.title.length === 0) {
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
        getRemoteUserList(query) {
            userSearch(query).then(response => {
                if (!response.data.items) return
                this.userListOptions = response.data.items.map(v => v.name)
            })
        },
        remoteMethod(query) {
            if (query !== '') {
                this.loading = true;
                setTimeout(() => {
                    this.loading = false;
                    this.options4 = this.list.filter(item => {
                        return item.label.toLowerCase()
                            .indexOf(query.toLowerCase()) > -1;
                    });
                }, 200);
            } else {
                this.options4 = [];
            }
        },
        selectStudentChanged() {
            this.studentActivityList = this.value9.map(item => {
                let studentName = 'NULL'
                for (let s of this.list){
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

            console.log( this.studentActivityList)
        },

        // tag
        removeStudentTag(val) {
            console.log(val)
            this.value9 = []
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
