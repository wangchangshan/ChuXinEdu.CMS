<template>
<el-row :gutter="10">
    <el-col :xs="12" :sm="12" :lg="12">
        <el-card shadow="never" class="card-student-course">
        <el-calendar>
            <template slot="dateCell" slot-scope="{date, data}">
                <p @click="getSchedule(data.day)" :class="data.isSelected ? 'is-selected' : ''">
                    {{ data.day.split('-').slice(2).join('-') }} {{ data.isSelected ? '✔️' : ''}}
                </p>
                <div @click="getSchedule(data.day)" style="height:20px"></div>
            </template>
        </el-calendar>
        </el-card>
    </el-col>

    <el-col :xs="12" :sm="12" :lg="12">
        <el-card shadow="never" class="card-student-course">
            <div class="schedule-contain">
                <p class="schedule-title"><i class="el-icon-notebook-1 el-icon--left"></i>{{selectedDay}} 课程安排</p>

                <el-table v-loading="loading" :span-method="objectSpanMethod" :data="courseSchedule" style="width: 100%" :height="tableHeight" border align="center" size="small">
                    <el-table-column prop="coursePeriod" :label="selectedDay" align='center' width="100">
                    </el-table-column>
                    <el-table-column prop="courseFolderName" label="课程内容" width="100" align='center'>
                    </el-table-column>
                    <el-table-column prop="studentName" label="学生姓名" align='center'>
                    </el-table-column>
                </el-table>
            </div>
        </el-card>
    </el-col>
</el-row>
</template>

<script>
import {
    axios,
    dateHelper
} from "@/utils/";

export default {
    data() {
        return {
            loading: false,
            tableHeight: this.$store.state.page.win_content.height - 80,
            selectedDay: '',
            courseSchedule: [],
            timeRowSpanArray: [],
            folderRowSpanArray: []
        }
    },
    created() {
        // 获取今天的课程表
        this.getSchedule(dateHelper.getDate(new Date()));
    },
    methods: {
        getSchedule(selectedDay) {
            this.loading = true;
            this.selectedDay = selectedDay;
            this.courseSchedule = [];
            axios({
                type: 'get',
                path: '/api/coursearrange/getcoursearrangedbyday/' + selectedDay,
                fn: (result) => {
                    this.courseSchedule = result;
                    this.getRowSpanInfo();
                    this.loading = false;
                }
            })
        },

        objectSpanMethod({
            row,
            column,
            rowIndex,
            columnIndex
        }) {
            if (columnIndex === 0) {
                return {
                    rowspan: this.timeRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
            if (columnIndex === 1) {
                return {
                    rowspan: this.folderRowSpanArray[rowIndex],
                    colspan: 1
                };
            }
        },

        getRowSpanInfo() {
            this.timeRowSpanArray = [];
            this.folderRowSpanArray = [];
            let strTime = '';
            let strFolder = '';
            let timeIndex = 0;
            let folderIndex = 0;
            this.courseSchedule.forEach((item, index, array) => {
                this.timeRowSpanArray.push(1);
                this.folderRowSpanArray.push(1);
                if (index === 0) {
                    strTime = item.coursePeriod;
                    strFolder = item.courseFolderCode;
                } else {
                    if (item.coursePeriod === strTime) {
                        this.timeRowSpanArray[timeIndex] += 1;
                        this.timeRowSpanArray[index] = 0;

                        if (item.courseFolderCode === strFolder) {
                            this.folderRowSpanArray[folderIndex] += 1;
                            this.folderRowSpanArray[index] = 0;
                        } else {
                            strFolder = item.courseFolderCode;
                            folderIndex = index;
                        }
                    } else {
                        strTime = item.coursePeriod;
                        timeIndex = index;

                        strFolder = item.courseFolderCode;
                        folderIndex = index;
                    }
                }
            });
        },
    },

}
</script>

<style lang="less" scoped>
.is-selected {
    color: #1989FA;
}

.schedule-contain {
    padding: 10px;
    text-align: center;
    font-size: 14px;

    .schedule-title {
        width: 100%;
        height: 45px;
        line-height: 45px;
        cursor: pointer;
        background-color: #409EFF;
        color: white;
        display: block;
    }
}
</style>
