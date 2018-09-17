<template>
<div class="fallcontain">
    <el-row type="flex" class="row-bg" :gutter="10" :max-height="pageHeight">
        <el-col :span="6" >
            <el-card v-for="achievement in achievementList1" 
                :key="achievement.achievement_code" :body-style="{ padding: '0px' }" style="margin-bottom:5px">
                <img :src="achievement.achievement_path" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.achievement_desc}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.achievement_date }}</time>
                        <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in achievementList2" 
                :key="achievement.achievement_code" :body-style="{ padding: '0px' }">
                <img :src="achievement.achievement_path" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.achievement_desc}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.achievement_date }}</time>
                        <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in achievementList3" 
                :key="achievement.achievement_code" :body-style="{ padding: '0px' }">
                <img :src="achievement.achievement_path" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.achievement_desc}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.achievement_date }}</time>
                        <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="6">
            <el-card v-for="achievement in achievementList3" 
                :key="achievement.achievement_code" :body-style="{ padding: '0px' }">
                <img :src="achievement.achievement_path" class="image">
                <div style="padding: 14px;">
                    <span>{{achievement.achievement_desc}}</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ achievement.achievement_date }}</time>
                        <el-rate v-model="achievement.achievement_rate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
    </el-row>
</div>
</template>

<script>
export default {
    data(){
        return {
            pageHeight: this.$store.state.page.win_content.height - 308,
            achievementList1:[],
            achievementList2:[],
            achievementList3:[],
            achievementList4:[],
            achievementList:[{
                achievement_code: '1',
                achievement_path:'../../static/image/test.jpg',
                achievement_desc:'动物——老虎',
                achievement_date:'2018-08-07',
                achievement_rate:5,
            },{
                achievement_code: '2',
                achievement_path:'../../static/image/test2.jpg',
                achievement_desc:'老鼠的早餐',
                achievement_date:'2018-08-07',
                achievement_rate:4.5,
            },{
                achievement_code: '3',
                achievement_path:'../../static/image/test3.jpg',
                achievement_desc:'小幅山水画',
                achievement_date:'2018-08-07',
                achievement_rate:null,
            },{
                achievement_code: '4',
                achievement_path:'../../static/image/test3.jpg',
                achievement_desc:'小幅山水画',
                achievement_date:'2018-08-07',
                achievement_rate:null,
            }], 
        }
    },
    created(){
        this.achievementList.forEach((item, index) => {
            if((index + 1) % 3 === 1){
                this.achievementList1.push(item);
            }
            else if((index + 1) % 3 === 2){
                this.achievementList2.push(item);
            }
            else {
                this.achievementList3.push(item);
            }
        });
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
