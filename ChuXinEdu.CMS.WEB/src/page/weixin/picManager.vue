<template>
<div class="fallcontain">
    <el-row type="flex" class="row-bg" :gutter="10" style="overflow:auto" v-bind:style="{height: pageHeight + 'px'}">
        <el-col :span="6">
            <el-card v-for="picure in picList1" :key="picure.id" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.picturePath" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.subject}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.studentName }}</time>
                        <span class="time">{{picure.studentAge}}岁</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.id)"></el-button>
                        <!-- <el-rate v-model="picure.achievement_rate" :allow-half = "true" class="right"></el-rate> -->
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList2" :key="picure.id" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.picturePath" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.subject}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.studentName }}</time>
                        <span class="time">{{picure.studentAge}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.id)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList3" :key="picure.id" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.picturePath" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.subject}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.studentName }}</time>
                        <span class="time">{{picure.studentAge}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.id)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList4" :key="picure.id" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.picturePath" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.subject}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.studentName }}</time>
                        <span class="time">{{picure.studentAge}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.id)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
    </el-row>

    <div class="footer_container">
        <el-button type="primary" size="small" @click='showUploadDialog()'>上传图片<i class="el-icon-upload el-icon--right"></i></el-button>
    </div>

    <el-dialog :title="uploadDialog.title" :visible.sync="uploadDialog.isShow" :width="uploadDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form v-if="wx_pic_code == '02'" ref="wxPicture" :model="uploadDialog.params" size="mini" :label-width="'100px'" :label-position="'right'" label-suffix='：' style="margin-right:20px">
                <el-form-item label="姓名">
                    <el-input v-model="uploadDialog.params.studentName"></el-input>
                </el-form-item>
                <el-form-item label="性别">
                    <el-radio-group v-model="uploadDialog.params.studentSex">
                        <el-radio label="男"></el-radio>
                        <el-radio label="女"></el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="绘画年龄">
                    <el-input v-model="uploadDialog.params.studentAge"></el-input>
                </el-form-item>
                <el-form-item label="作品主题">
                    <el-input v-model="uploadDialog.params.subject"></el-input>
                </el-form-item>
            </el-form>
            <el-upload :on-error="handleError" class="upload-demo" :multiple="uploadDialog.multiple" :action="uploadDialog.actionUrl" :data="uploadDialog.params" :file-list="uploadDialog.thumbnailList" :on-success="uploadSuccess" list-type="picture">
                <el-button size="mini" type="primary">选择上传<i class="el-icon-upload el-icon--right"></i></el-button>
            </el-upload>
            <el-form style="margin:10px;width:auto;">
                <el-form-item class="text_right">
                    <el-button v-noRepeatClick size="small" type="primary" @click="btnSubmitUpload()">确 定</el-button>
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
    props: {
        'wx_pic_code': String,
    },
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height - 96,
            picList: [],
            picList1: [],
            picList2: [],
            picList3: [],
            picList4: [],

            uploadDialog: {
                title: '上传图片',
                isShow: false,
                multiple: true,
                actionUrl: '/api/upload/uploadwxpic',
                fileCount: 0,
                thumbnailList: [],
                params: {
                    studentCode: '',
                    studentName: '',
                    studentAge: 0,
                    studentSex: '',
                    teacherCode: '',
                    subject: '',
                    wxPictureType: ''
                },
            }
        }
    },
    created() {
        this.getAllPictures();
    },
    methods: {
        dataInit() {
            this.picList = [];
            this.picList1 = [];
            this.picList2 = [];
            this.picList3 = [];
            this.picList4 = [];
        },

        getAllPictures() {
            axios({
                type: 'get',
                path: '/api/wxpicture/getwxpicture/' + this.wx_pic_code,
                fn: result => {
                    this.dataInit();
                    this.picList = result;
                    this.GenerateColumn();
                }
            });
        },

        GenerateColumn() {
            this.picList.forEach((item, index) => {
                if ((index + 1) % 4 === 1) {
                    this.picList1.push(item);
                } else if ((index + 1) % 4 === 2) {
                    this.picList2.push(item);
                } else if ((index + 1) % 4 === 3) {
                    this.picList3.push(item);
                } else {
                    this.picList4.push(item);
                }
            });
        },

        showUploadDialog() {
            this.uploadDialog.params.wxPictureType = this.wx_pic_code;
            this.uploadDialog.thumbnailList = [];
            this.uploadDialog.fileCount = 0;
            this.uploadDialog.isShow = true;
        },

        handleError(err, file, fileList) {
            this.$notify({
                title: '上传失败',
                message: err,
                type: 'error'
            });
        },

        uploadSuccess(response, file, fileList) {
            // -1 文件存储错误； -2 数据库插入错误   
            if (response == -1 || response == -2) {
                this.$notify({
                    title: '上传失败',
                    message: '返回值：' + response,
                    type: 'error'
                });
            } else {
                this.uploadDialog.fileCount = fileList.length;
            }
        },

        btnSubmitUpload() {
            this.getAllPictures();
            this.uploadDialog.thumbnailList = [];
            this.uploadDialog.isShow = false;
            this.uploadDialog.params = {
                studentCode: '',
                studentName: '',
                studentAge: 0,
                studentSex: '',
                teacherCode: '',
                subject: '',
                wxPictureType: ''
            }
        },

        removeAchievement(id) {
            var _this = this;
            this.$confirm('确定删除这张图片吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/upload/delwxpicture/' + id,
                    fn: function (result) {
                        if (result === 1200) {
                            _this.getAllPictures();
                            _this.$message({
                                message: '删除成功！',
                                type: 'success'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        }
    }
}
</script>

<style lang="less" scoped>
.fallcontain {
    overflow-y: auto;
    overflow-x: hidden;
}

.time {
    font-size: 13px;
    color: #999;
}

.bottom {
    margin-top: 13px;
    line-height: 12px;
}

.button {
    padding: 0;
    float: right;
}

.right {
    padding: 0;
    float: right;
    margin-top: -3px;
}

.image {
    width: 100%;
    display: block;
}

.clearfix:before,
.clearfix:after {
    display: table;
    content: "";
}

.clearfix:after {
    clear: both
}

.footer_container {
    height: 36px;
    line-height: 36px;
    text-align: left;
}
</style>
