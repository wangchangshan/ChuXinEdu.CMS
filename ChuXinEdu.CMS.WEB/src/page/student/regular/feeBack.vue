<template>
    <div class="info_container">
        <el-row type="flex" class="row-bg" justify="space-between">
            <div class="tiparea1">{{feebackFormula}}</div>
            <el-button v-noRepeatClick size="small" type="primary" @click="prepareFeeback()">退费计算</el-button>
        </el-row>
        <el-row v-if="feeBackPackage.length > 0">
            <el-card class="box-card" shadow="never">
                <el-collapse v-model="activeNames">
                    <el-collapse-item
                        v-for="fpackage in feeBackPackage"
                        v-bind:key="fpackage.packageCode"
                        :name="fpackage.packageCode"
                    >
                        <template slot="title">
                            <a class="sub-title">
                                套餐《{{ fpackage.packageName }}》 退费
                                <span
                                    style="color:#f56767"
                                >{{fpackage.backFee}}</span> 元
                            </a>
                        </template>
                        <div>实际课程数目：{{fpackage.actualCourseCount}} 节</div>
                        <div>实际缴费金额：{{fpackage.actualPrice}} 元</div>
                        <div>剩余课时数目：{{fpackage.restCourseCount}} 节</div>
                        <div>退费金额：( {{fpackage.actualPrice}} / {{fpackage.actualCourseCount}} ) X {{fpackage.restCourseCount}} X 80% = {{ fpackage.backFee }}</div>
                        <el-button
                            v-noRepeatClick
                            size="small"
                            plain
                            type="primary"
                            @click="packageFeeback(fpackage.id)"
                        >当前套餐退费</el-button>
                    </el-collapse-item>
                </el-collapse>
            </el-card>
        </el-row>
        <el-row v-if="feeBackPackage.length > 0">
            <el-card class="box-card" shadow="never" style="border:0">
                <div slot="header" class="clearfix" style="border:0">
                    <a class="big-title">
                        总计退费
                        <span style="color:#f56767">{{totalFeeback}}</span> 元
                    </a>
                </div>
                <div>
                    <el-button v-noRepeatClick type="primary" @click="submitFeeback()">全部退费</el-button>
                </div>
            </el-card>
        </el-row>
        <el-row v-if="noNeedFeeback">
            <el-card class="box-card" shadow="never" style="border:0">
                <div class="tiparea2">没有需要退还的费用！</div>
            </el-card>
        </el-row>
    </div>
</template>

<script>
import { axios } from "@/utils/index";

export default {
    name: "student-fee-back",
    props: {
        studentCode: String
    },
    data() {
        return {
            activeNames: [],
            feeBackPackage: [],
            feebackFormula:
                "退费金额 = (实际缴费金额 / 实际课程数目) X 剩余课时数目 * 80%",
            totalFeeback: 0,
            noNeedFeeback: false,
            feeBackData: []
        };
    },
    created() {},
    methods: {
        prepareFeeback() {
            this.feeBackData = [];
            this.totalFeeback = 0;
            axios({
                type: "get",
                path: "/api/student/getnofinishpackage/" + this.studentCode,
                fn: result => {
                    if (result.length == 0) {
                        this.noNeedFeeback = true;
                    }
                    result.forEach(item => {
                        this.activeNames.push(item.packageCode);
                        item.backFee = (
                            (item.actualPrice / item.actualCourseCount) *
                            item.restCourseCount *
                            0.8
                        ).toFixed(2);
                        this.totalFeeback += parseFloat(item.backFee);
                        this.feeBackData.push({
                            Id: item.id,
                            feeBackAmount: parseFloat(item.backFee)
                        });
                    });
                    this.feeBackPackage = result;
                }
            });
        },
        packageFeeback(curPId) {
            var _this = this;
            this.$confirm("确定对当前套餐进行退费操作?", "提示", {
                confirmButtonText: "确定",
                cancelButtonText: "取消",
                type: "warning"
            })
                .then(() => {
                    axios({
                        type: "put",
                        path:
                            "/api/student/packagefeeback/" +
                            _this.studentCode +
                            "?packageId=" +
                            curPId,
                        data: this.feeBackData,
                        fn: function(result) {
                            if (result == 1200) {
                                _this.prepareFeeback();
                                _this.$message({
                                    message: "套餐退费成功",
                                    type: "success"
                                });
                            }
                        }
                    });
                })
                .catch(() => {
                    //
                });
        },
        submitFeeback() {
            var _this = this;
            this.$confirm(
                "确定对学员【" +
                    _this.$route.query.studentname +
                    "】进行全部套餐退费操作?",
                "提示",
                {
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                    type: "warning"
                }
            )
                .then(() => {
                    axios({
                        type: "put",
                        path: "/api/student/feeback/" + this.studentCode,
                        data: _this.feeBackData,
                        fn: function(result) {
                            if (result == 1200) {
                                _this.prepareFeeback();
                                _this.$message({
                                    message: "退费成功",
                                    type: "success"
                                });
                            }
                        }
                    });
                })
                .catch(() => {
                    //
                });
        }
    }
};
</script>

<style lang="less" scoped>
.sub-title {
    font-size: 14px;
    font-weight: 600;
}

.big-title {
    font-size: 24px;
    font-weight: 600;
}

.tiparea1 {
    text-align: left;
    font-size: 12px;
    color: #e6a23c;
    padding-left: 20px;
}

.tiparea2 {
    text-align: left;
    font-size: 26px;
    color: #67c23a;
    font-weight: 550;
}
</style>
