<template>
<div class="fallcontain">
    <el-row type="flex" class="row-bg" :gutter="10" :max-height="pageHeight">
        <el-col :span="6" >
            <el-card v-for="achievement in artWorkList1" 
                :key="achievement.artworkId" :body-style="{ padding: '0px' }" style="margin-bottom:5px">
                <img :src="achievement.showURL" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                        <!-- <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate> -->
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList2" 
                :key="achievement.artworkId" :body-style="{ padding: '0px' }">
                <img :src="achievement.showURL" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList3" 
                :key="achievement.artworkId" :body-style="{ padding: '0px' }">
                <img :src="achievement.showURL" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in artWorkList4" 
                :key="achievement.artworkId" :body-style="{ padding: '0px' }">
                <img :src="achievement.showURL" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.artworkTitle}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.finishDate }}</time>
                    </div>
                </div>
            </el-card>
        </el-col>
    </el-row>
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
    data(){
        return {
            pageHeight: this.$store.state.page.win_content.height - 308,
            artWorkList: [],
            artWorkList1:[],
            artWorkList2:[],
            artWorkList3:[],
            artWorkList4:[],
        }
    },
    created(){
        var _this = this;
        axios({
            type: 'get',
            path: '/api/student/getartworklist',
            data: {
                studentCode: _this.studentCode
            },
            fn: function (result) {
                result.forEach(item => {
                    item.finishDate = item.finishDate.split('T')[0];
                });
                _this.artWorkList = result;
                _this.GenerateColumn();
            }
        });

       
    },
    methods: {
        GenerateColumn() {
            this.artWorkList.forEach((item, index) => {
            if((index + 1) % 4 === 1){
                this.artWorkList1.push(item);
            }
            else if((index + 1) % 4 === 2){
                this.artWorkList2.push(item);
            }
            else if((index + 1) % 4 === 3){
                this.artWorkList3.push(item);
            }
            else
            {
                this.artWorkList4.push(item);
            }
        });
        }
    }
}
</script>

<style lang="less" scoped>
.fallcontain{
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

  .right{
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

</style>
