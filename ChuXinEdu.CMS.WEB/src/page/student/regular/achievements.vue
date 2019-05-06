<template>
<div class="fallcontain">
    <el-row type="flex" class="row-bg" :gutter="10" style="overflow:auto" v-bind:style="{height: pageHeight + 'px'}">
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList1" :key="achievement.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="achievement.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                        <span class="time">{{achievement.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(achievement.artworkId)"></el-button>
                        <!-- <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate> -->
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList2" :key="achievement.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="achievement.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                        <span class="time">{{achievement.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(achievement.artworkId)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList3" :key="achievement.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="achievement.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                        <span class="time">{{achievement.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(achievement.artworkId)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList4" :key="achievement.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="achievement.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                        <span class="time">{{achievement.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(achievement.artworkId)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
    </el-row>

    <div class="footer_container">
        <el-button type="primary" size="small" @click='showUploadDialog()'>上传作品<i class="el-icon-upload el-icon--right"></i></el-button>
    </div>

    <el-dialog :title="uploadDialog.title" :visible.sync="uploadDialog.isShow" :width="uploadDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-upload :on-error="handleError" class="upload-demo" :multiple="uploadDialog.multiple" :action="uploadDialog.actionUrl" :data="uploadDialog.params" :file-list="uploadDialog.thumbnailList" :on-remove="handleImgRemove" :before-upload="beforeUpload" :on-success="uploadSuccess" list-type="picture">
                <el-button size="mini" type="primary">选择上传<i class="el-icon-upload el-icon--right"></i></el-button>
            </el-upload>
            <el-form style="margin:10px;width:auto;">
                <el-form-item class="text_right">
                    <el-button size="small" @click="btnCancelUpload()">取 消</el-button>
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
        'studentCode': String,
    },
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height - 96,
            artWorkList: [],
            artWorkList1: [],
            artWorkList2: [],
            artWorkList3: [],
            artWorkList4: [],

            uploadDialog: {
                title: '批量上传作品',
                isShow: false,
                multiple: true,
                actionUrl: '/api/upload/uploadartworksimple',
                fileCount: 0,
                fileUIds: [],
                thumbnailList: [],
                params: {
                    studentCode: '',
                    studentName: '',
                    uid: ''
                },
            }
        }
    },
    created() {
        this.getAllArtWorks();
    },
    methods: {
        dataInit() {
            this.artWorkList = [];
            this.artWorkList1 = [];
            this.artWorkList2 = [];
            this.artWorkList3 = [];
            this.artWorkList4 = [];
        },
        getAllArtWorks() {
            var _this = this;
            axios({
                type: 'get',
                path: '/api/student/getartworklist',
                data: {
                    studentCode: _this.studentCode
                },
                fn: function (result) {
                    _this.dataInit();
                    result.forEach(item => {
                        item.finishDate = item.finishDate.split('T')[0];
                    });
                    _this.artWorkList = result;
                    _this.GenerateColumn();
                }
            });
        },
        GenerateColumn() {
            this.artWorkList.forEach((item, index) => {
                if ((index + 1) % 4 === 1) {
                    this.artWorkList1.push(item);
                } else if ((index + 1) % 4 === 2) {
                    this.artWorkList2.push(item);
                } else if ((index + 1) % 4 === 3) {
                    this.artWorkList3.push(item);
                } else {
                    this.artWorkList4.push(item);
                }
            });
        },

        showUploadDialog() {
            this.uploadDialog.params = {
                studentCode: this.studentCode,
                studentName: this.$route.query.studentname,
            }
            this.uploadDialog.thumbnailList = [];
            this.uploadDialog.fileUIds = [];
            this.uploadDialog.fileCount = 0;
            this.uploadDialog.isShow = true;
        },

        beforeUpload(file) {
            this.uploadDialog.params.uid = file.uid;
        },

        handleError(err, file, fileList){
            this.$notify({
                title: '上传失败',
                message: err,
                type: 'error'
            });
        },

        uploadSuccess(response, file, fileList) {            
            // -1 文件存储错误； -2 数据库插入错误   
            if(response == -1 || response == -2){
                this.$notify({
                    title: '上传失败',
                    message: '返回值：' + response,
                    type: 'error'
                });
            }
            else{             
                this.uploadDialog.fileCount = fileList.length;
                this.uploadDialog.fileUIds = [];
                for (let f of fileList) {
                    this.uploadDialog.fileUIds.push(f.uid);
                }
            }
        },

        handleImgRemove(file, fileList) {
            this.uploadDialog.fileCount = fileList.length;
            this.uploadDialog.fileUIds = [];
            for (let f of fileList) {
                this.uploadDialog.fileUIds.push(f.uid);
            }

            axios({
                type: 'delete',
                path: '/api/upload/deltempfile',
                data: {
                    courseId: 0,
                    uid: file.uid
                },
                fn: function (result) {}
            });
        },
        btnSubmitUpload() {
            let fileUIds = this.uploadDialog.fileUIds;
            axios({
                type: 'put',
                path: '/api/upload/artwork2yes',
                data: fileUIds,
                fn: result => {
                    if (result == 1200) {
                        this.getAllArtWorks();
                        this.$message({
                            message: '全部上传成功',
                            type: 'success'
                        });
                        this.uploadDialog.thumbnailList = [];
                        this.uploadDialog.isShow = false;
                    }
                }
            });
        },
        btnCancelUpload() {
            this.uploadDialog.isShow = false;
            this.uploadDialog.thumbnailList = [];
            let fileUIds = this.uploadDialog.fileUIds;
            
            if (fileUIds.length > 0) {
                let _this = this;
                axios({
                    type: 'delete',
                    path: '/api/upload/delalltempfile',
                    data: fileUIds,
                    fn: function (result) {}
                });
            }

        },
        removeAchievement(achievementId){
            var _this = this;
            this.$confirm('确定删除这个作品吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/upload/delachievement/' + achievementId,
                    fn: function (result) {
                        if (result === 1200) {
                            _this.getAllArtWorks();
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
