<template>
<!-- <div class="createPost-container">
    <comment :time="'2019-01-11'" :id="1" :content="'测试一下'"></comment>
  </div> -->
<div class="table_container">
    <el-table id="capture" :data="commentList" v-loading="loading" class="table-comment" size="mini" align="left" stripe border :height="tableHeight">
        <el-table-column type="index" width="50" align='center'></el-table-column>
        <el-table-column prop="courseDate" label="时间" align='center' width="120">
        </el-table-column>
        <el-table-column prop="content" label="课堂评语" align='left' min-width="200">
        </el-table-column>
        <el-table-column prop="teacherName" label="点评教师" align='center' width="100">
        </el-table-column>
        <el-table-column prop="operation" align='center' label="操作" fixed="right" width="130">
            <template slot-scope='scope'>
                    <el-button type="primary" size="mini" icon='el-icon-edit' @click='editComment(scope.row)'></el-button>
                    <el-button v-noRepeatClick type="danger" size="mini" icon='el-icon-delete' @click='removeComment(scope.row.commentId)'></el-button>
                </template>
            </el-table-column>
    </el-table>
    <div class="footer_container">
        <el-button v-noRepeatClick type="primary" size="small" @click='addComment()'><i class="fa fa-plus" aria-hidden="true"></i> 添加课堂评语</el-button>
        <el-button v-noRepeatClick type="primary" size="small" @click='export2Excle()' :loading="downloadLoading"><i class="fa fa-file-excel-o" aria-hidden="true"></i> 导出Excel</el-button>
        <el-button v-noRepeatClick type="primary" size="small" @click='captureCourse()' :loading="captureLoading"><i class="fa fa-camera" aria-hidden="true"></i> 导出图片</el-button>
    </div>

    <el-dialog :title="commentDialog.title" :visible.sync="commentDialog.isShow" :width="commentDialog.width" :close-on-click-modal='false' :close-on-press-escape='false' :modal-append-to-body="false">
        <div class="form">
            <el-form ref="comment" :model="commentDialog.commentInfo" :rules="commentDialog.commentInfoRules" :label-width="commentDialog.formLabelWidth" :label-position='commentDialog.labelPosition' size="mini" style="margin:10px;width:auto;" label-suffix='：'>
                <el-form-item prop="courseDate" label="时间">
                    <el-date-picker v-model="commentDialog.commentInfo.courseDate" type="date" placeholder="选择日期" :editable="false" size="mini" value-format="yyyy-MM-dd" > </el-date-picker>
                </el-form-item>
                <el-form-item prop="content" label="评语">
                    <el-input type="textarea" :rows="6" placeholder="请输入教师评语" v-model="commentDialog.commentInfo.content"></el-input>
                </el-form-item>
                <el-form-item class="text_right">
                    <el-button size="small" @click="resetForm('comment')">取 消</el-button>
                    <el-button v-noRepeatClick size="small" type="primary" @click="submitForm('comment')">确 定</el-button>
                </el-form-item>
            </el-form>
        </div>
    </el-dialog>
</div>
</template>

<script>
import {
    dateHelper,
    LocalDB,
    axios
} from '@/utils/index'

import html2canvas from 'html2canvas';

// import Comment from '@/components/Comment'

export default {
    //   components: { Comment },
    props: {
        'studentCode': String,
    },
    data() {
        return {
            studentName: '',
            commentList: [],
            loading: true,
            downloadLoading: false,
            captureLoading: false,
            tableHeight: this.$store.state.page.win_content.height - 100,
            curTeacherLoginCode: '',
            commentDialog:{
                width: '500px',
                isShow: false,
                title: '添加评语',
                labelPosition: 'right',
                formLabelWidth: '70px',
                panelType: 'edit',
                curCommentId: '',
                commentInfo: {
                    courseDate: '',
                    studentCode: '',
                    studentName: '',
                    teacherCode: '',
                    teacherName: '',
                    content: ''
                },
                commentInfoRules: {
                    courseDate: [{
                        required: true,
                        message: '课程日期不能为空',
                        trigger: 'blur'
                    }],
                    content: [{
                        required: true,
                        message: '评语不能为空',
                        trigger: 'blur'
                    }]
                }
            }
        }
    },
    created() {
        this.fetchData();
        this.studentName = this.$route.query.studentname;
        this.commentDialog.commentInfo.studentCode = this.studentCode;
        this.commentDialog.commentInfo.studentName = this.$route.query.studentname;
    },
    methods: {
        fetchData() {
            axios({
                type: 'get',
                path: '/api/comment/' + this.studentCode,
                fn: result => {
                    result.forEach(item => {
                        item.courseDate = item.courseDate.split('T')[0];
                    });
                    this.commentList = result;
                    this.loading = false;
                }
            });
        },

        addComment() {
            if(!this.curTeacherLoginCode) {
                let strUser = LocalDB.instance("USER_").getValue("BASEINFO").value;
                this.curTeacherLoginCode = JSON.parse(strUser).username;
            }

            this.commentDialog.title = "添加评语";
            this.commentDialog.panelType = "add";            
            this.commentDialog.isShow = true;

            this.commentDialog.commentInfo.courseDate = dateHelper.getDate();
            this.commentDialog.commentInfo.teacherName = this.curTeacherLoginCode;
            this.commentDialog.commentInfo.content = '';
        },

        editComment(row) {
            this.commentDialog.title = "修改评语";
            this.commentDialog.panelType = "edit";
            this.commentDialog.curCommentId = row.commentId;
            this.commentDialog.commentInfo = {
                courseDate: row.courseDate,
                studentCode: row.studentCode,
                studentName: row.studentName,
                teacherCode: row.teacherCode,
                teacherName: row.teacherName,
                content: row.content
            };
            this.commentDialog.isShow = true;
        },

        removeComment(commentId) {
            this.$confirm('确定删除当前评语吗?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                axios({
                    type: 'delete',
                    path: '/api/comment/' + this.commentDialog.curCommentId,
                    fn: result => {
                        if (result === 1200) {
                            this.fetchData();
                            this.$message({
                                message: '删除评语成功！',
                                type: 'success'
                            });
                            this.commentDialog.isShow = false;
                        }else {
                            this.$message({
                                message: '删除评语失败！',
                                type: 'error'
                            });
                        }
                    }
                });
            }).catch(() => {
                //
            });
        },

        resetForm(form) {
            this.$refs[form].resetFields();
            this.commentDialog.isShow = false;
        },

        submitForm(form) {
            this.$refs[form].validate((valid) => {
                if (valid) {
                    this.commentSubmit();
                }
            });
        },     
        
        commentSubmit() {
            if (this.commentDialog.panelType == 'add') {
                axios({
                    type: 'post',
                    path: '/api/comment',
                    data: this.commentDialog.commentInfo,
                    fn: result => {
                        if (result === 1200) {
                            this.fetchData();
                            this.$message({
                                message: '添加评语成功',
                                type: 'success'
                            });
                            this.commentDialog.isShow = false;
                        }
                        else {
                            this.$message({
                                message: '添加评语失败',
                                type: 'error'
                            });
                        }
                    }
                });
            }
            else {
                axios({
                    type: 'put',
                    path: '/api/comment/' + this.commentDialog.curCommentId,
                    data: this.commentDialog.commentInfo,
                    fn: result => {
                        if (result === 1200) {
                            this.fetchData();
                            this.$message({
                                message: '修改评语成功',
                                type: 'success'
                            });
                            this.commentDialog.isShow = false;
                        }
                        else {
                            this.$message({
                                message: '修改评语失败',
                                type: 'error'
                            });
                        }
                    }
                });
            }
        },

        // 前端导出
        export2Excle() {
            if (this.commentList.length == 0) {
                this.$message({
                    message: '没有课堂评语需要导出！',
                    type: 'success'
                });
                return;
            }
            var filename = this.studentName + "_课堂评语";
            this.downloadLoading = true;
            import('@/vendor/Export2Excel').then(excel => {
                const tHeader = ['日期', '课堂评语', '评语教师'];
                const filterVal = ['courseDate', 'content', 'teacherName']
                const data = this.formatJson(filterVal, this.commentList)
                excel.export_json_to_excel({
                    header: tHeader,
                    data,
                    filename: filename,
                    autoWidth: true,
                    bookType: 'xlsx'
                })
                this.downloadLoading = false;
            })
        },

        formatJson(filterVal, jsonData) {
            return jsonData.map(v => filterVal.map(j => {
                if (j === 'courseDate') {
                    return v[j] && v[j].split('T')[0] || ''; //parseTime(v[j])
                } else {
                    return v[j]
                }
            }))
        },

        captureCourse() {
            this.captureLoading = true;
            let dom = document.querySelector("#capture");
            let width = dom.offsetWidth;

            // clone dom
            let cloneDom = dom.cloneNode(true);
            cloneDom.style.height = 'auto';
            cloneDom.childNodes[2].style.height = 'auto';
            cloneDom.style.zIndex = "-1";
            document.body.appendChild(cloneDom);
            let height = cloneDom.offsetHeight;

            // create canvas
            let canvas = document.createElement("canvas");
            let scale = 1.8; //定义比例
            canvas.width = width * scale; //定义canvas 宽度 * 缩放
            canvas.height = height * scale; //定义canvas高度 * 缩放
            canvas.getContext("2d").scale(scale, scale); //获取context,设置比例

            let h2cOpts = {
                scale: scale, // 比例
                canvas: canvas, //自定义 canvas
                width: width - 130, // 原始宽度 - 按钮区域宽度， HTML2canvas后的canvas宽度，会 * scale
                height: height, // 原始高度
                useCORS: true, // 开启跨域配置
                dpi: window.devicePixelRatio * 2
            };

            let waterMarkOpts = {
                fontStyle: "76px 幼圆", //水印字体设置
                rotateAngle: -20 * Math.PI / 180, //水印字体倾斜角度设置
                fontColor: "#7eb00a", //水印字体颜色设置
                linePositionX: (width - 130) * scale / 2, //canvas第一行文字起始X坐标
                firstLinePositionY: height * scale - 220,
                secondLinePositionY: height * scale - 130,
                thirdLinePositionY: height * scale - 40,
            };

            html2canvas(cloneDom, h2cOpts).then(canvas => {
                var img = new Image();
                var ctx = canvas.getContext('2d');
                if (img.complete) {
                    img.src = canvas.toDataURL("image/png", 1.0);
                    img.onload = () => {
                        ctx.drawImage(img, 0, 0);
                        ctx.font = waterMarkOpts.fontStyle;
                        //文字倾斜角度
                        //ctx.rotate(waterMarkOpts.rotateAngle);
                        ctx.fillStyle = waterMarkOpts.fontColor;

                        ctx.fillText(this.studentName, waterMarkOpts.linePositionX, waterMarkOpts.firstLinePositionY);
                        ctx.fillText("初心工作室", waterMarkOpts.linePositionX, waterMarkOpts.secondLinePositionY);
                        ctx.fillText(dateHelper.getDate(), waterMarkOpts.linePositionX, waterMarkOpts.thirdLinePositionY);
                        //坐标系还原
                        //ctx.rotate(-waterMarkOpts.rotateAngle);

                        // 关闭抗锯齿
                        ctx.mozImageSmoothingEnabled = false;
                        ctx.webkitImageSmoothingEnabled = false;
                        ctx.msImageSmoothingEnabled = false;
                        ctx.imageSmoothingEnabled = false;

                        this.downloadScreenShot(canvas);
                        document.body.removeChild(cloneDom);
                        this.captureLoading = false;
                    }
                }
            });
        },

        downloadScreenShot(canvas) {
            let url = canvas.toDataURL("image/png", 1.0)
            let link = document.createElement('a')
            link.style.display = 'none'
            link.href = url
            link.setAttribute('download', this.studentName + '的课堂评语.png')

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
    }
}
</script>

<style lang="less" scoped>
.footer_container {
    height: 36px;
    line-height: 36px;
    text-align: left;
}
</style>
