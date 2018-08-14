<template>
<div class="info_container">
    <el-row class="info_row row" :gutter="10">
        <el-col :span="5">
            <div class="area">
                <div class="imgarea">
                    <img src="../../static/image/test2.jpg">
                    <el-button type="success" size="small">上传<i class="el-icon-upload el-icon--right"></i></el-button>
                </div>
            </div>
        </el-col>

        <el-col :span="10">
            <div class="area">
                <div class="namearea">
                    <p>姓名：王小川</p>
                    <p>性别：男</p>
                    <p>出生日期：2006-07-19</p>
                    <p>入学日期：2017-09-19</p>
                    <p>联系电话：189xxxx9028</p>
                    <p>家庭住址：北京市海淀区龙兴园小区</p>
                    <p class="awards"><i class="el-icon-edit el-icon--left"></i>编辑</p>
                </div>
            </div>
        </el-col>
        <el-col :span="9">
            <div class="area">
                <div class="dataarea">
                    <p class="gtitle"><i class="el-icon-date el-icon--left"></i>课程数据</p>
                    <div class="gdataarea clear">
                        <div class="gdata left">
                            <p class="num">40</p>
                            <p class="title">总课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">18</p>
                            <p class="title">完成课时数</p>
                        </div>
                        <div class="gdata left">
                            <p class="num">￥3000</p>
                            <p class="title">缴费金额</p>
                        </div>
                    </div>
                </div>
            </div>
        </el-col>
    </el-row>
    <el-row class="info_row row" :gutter="10">
        <el-col :span="24">
            <template>
                <el-table :data="arrangementList" stripe border style="width: 100%">                    
                    <el-table-column prop="package_name" align='center' label="课程套餐" min-width="200" fixed>
                    </el-table-column>
                    <el-table-column prop="package_type" align='center' label="课程类别" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_date" align='center' label="缴费日期" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_mount" align='center' label="缴费金额" min-width="180">
                    </el-table-column>
                    <el-table-column prop="free_type" align='center' label="缴费方式" min-width="180">
                    </el-table-column>
                    <el-table-column prop="teacher_name" align='center' label="收款人" min-width="180">
                    </el-table-column>
                </el-table>
            </template>
        </el-col>
    </el-row>
</div>
</template>

<script>
export default {
    name: 'student-base-info',
    data() {
        return {
            arrangementList:[{

            },{

            }]
        }
    }
}
</script>

<style lang="less" scoped>
.info_container {
    padding: 0;
    margin: 0;
    overflow-x: hidden;
}

.row {
    margin: 20px;
}

.info_row {
    .area {
        border: 1px solid #dfdfdf;
        height: 200px;
        overflow: hidden;
        .imgarea {
            text-align: center;
            padding: 8px;
            img {
                width: 150px;
                height: 150px;
                border-radius: 50%;
            }
        }
        .namearea {
            padding: 10px;
            font-size: 14px;
            p {
                line-height: 24px;
            }
            .awards {
                text-align: center;
                width: 100%;
                height: 30px;
                line-height: 30px;
                cursor: pointer;
                background-color: #3bc5ff;
                border: 1px solid #3bc5ff;
                color: white;
                display: block;
            }
            .awards:hover {
                background-color: #f9c855;
                border: 1px solid #f9c855;
            }
        }
        .dataarea {
            padding: 10px;
            text-align: center;
            font-size: 14px;
            .gtitle {
                width: 100%;
                height: 30px;
                line-height: 30px;
                cursor: pointer;
                background-color: #3bc5ff;
                color: white;
                display: block;
            }
            .gdataarea {
                padding-left: 25px;
                p {
                    line-height: 38px;
                }
                .num {
                    font-weight: bolder;
                    color: #67c23a;
                }
                .title {
                    color: #3bc5ff;
                }
                .gdata {
                    margin: 10px;
                    float: left;
                }
            }
            .morearea {
                a {
                    color: #3bc5ff;
                }
            }
        }
    }
}
</style>
