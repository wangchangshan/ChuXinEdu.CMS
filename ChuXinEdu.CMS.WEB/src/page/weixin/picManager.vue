<template>
<div class="fallcontain">
    <div class="search_container">
        <el-form :inline="true" :model="searchField" class="demo-form-inline search-form">
            <!-- <el-form-item label="学号">not show for ipad UI
                <el-input type="text" size="small" v-model="searchField.studentCode" placeholder="请输入学号" class="search_field"></el-input>
            </el-form-item> -->
            <el-form-item label="姓名">
                <el-input type="text" size="small" v-model="searchField.studentName" placeholder="请输入学生姓名" class="search_field"></el-input>
            </el-form-item>
            <el-form-item label="状态">
                <el-select size="small" v-model="searchField.studentStatus" placeholder="请选择学生状态" class="search_field" :clearable="true">
                    <el-option v-for="item in $store.getters['student_status']" :key="item.value" :label="item.label" :value="item.value">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="el-icon-search" size="small" @click='searchStudent()'>查询</el-button>
                <el-button type="warning" icon="el-icon-refresh" size="small" @click='resetStudentList()'>重置</el-button>
            </el-form-item>
            <el-form-item class="btnRight">
                <el-button type="primary" size="small" @click='showAddStudent()'><i class="fa fa-user-plus" aria-hidden="true"></i> 添加</el-button>
                <el-button v-noRepeatClick type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出</el-button>
            </el-form-item>
        </el-form>
    </div>
    <el-row type="flex" class="row-bg" :gutter="10" style="overflow:auto" v-bind:style="{height: pageHeight + 'px'}">
        <el-col :span="6">
            <el-card v-for="picure in picList1" :key="picure.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.finishDate }}</time>
                        <span class="time">{{picure.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.artworkId)"></el-button>
                        <!-- <el-rate v-model="picure.achievement_rate" :allow-half = "true" class="right"></el-rate> -->
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList2" :key="picure.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.finishDate }}</time>
                        <span class="time">{{picure.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.artworkId)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList3" :key="picure.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.finishDate }}</time>
                        <span class="time">{{picure.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.artworkId)"></el-button>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="picure in picList4" :key="picure.artworkId" :body-style="{ padding: '0px' }" shadow="hover" style="margin-bottom:5px">
                <el-image :src="picure.showURL" lazy class="image">
                    <div slot="placeholder" class="image-slot">
                        加载中<span class="dot">...</span>
                    </div>
                </el-image>
                <div style="padding: 14px;">
                    <span>{{picure.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ picure.finishDate }}</time>
                        <span class="time">{{picure.documentSize}}</span>
                        <el-button type="text" icon="el-icon-delete" class="button" @click="removeAchievement(picure.artworkId)"></el-button>
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
    data() {
        return {
            pageHeight: this.$store.state.page.win_content.height - 96,
            picList: [],
            picList1: [],
            picList2: [],
            picList3: [],
            picList4: [],

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
                    _this.picList = result;
                    _this.GenerateColumn();
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
                        this.getAllPictures();
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
            this.$confirm('确定删除这张图片吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/upload/delachievement/' + achievementId,
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

.search_container {
    height: 36px;
    line-height: 36px;
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
