<template>
<div class="fallcontain">
    <el-row type="flex" class="row-bg" justify="space-around">
        <el-col :span="7">
            <el-card :body-style="{ padding: '0px' }">
                <img src="../../static/image/test.jpg" class="image">
                <div style="padding: 14px;">
                    <span>动物——老虎</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ currentDate }}</time>
                        <el-rate v-model="achievementRate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
            <div style="height:5px"></div>
            <el-card :body-style="{ padding: '0px' }">
                <img src="../../static/image/test2.jpg" class="image">
                <div style="padding: 14px;">
                    <span>老鼠的早餐</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ currentDate }}</time>
                        <el-rate v-model="achievementRate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="7">
            <el-card :body-style="{ padding: '0px' }">
                <img src="../../static/image/test2.jpg" class="image">
                <div style="padding: 14px;">
                    <span>老鼠的早餐</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ currentDate }}</time>
                        <el-rate v-model="achievementRate" :allow-half = "true" class="right"></el-rate>
                    </div>
                </div>
            </el-card>
        </el-col>
        <el-col :span="7">
            <el-card :body-style="{ padding: '0px' }">
                <img src="../../static/image/test3.jpg" class="image">
                <div style="padding: 14px;">
                    <span>小幅山水画</span>
                    <div class="bottom clearfix">
                        <time class="time">{{ currentDate }}</time>
                        <el-rate v-model="achievementRate" :allow-half = "true" class="right"></el-rate>
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
            currentDate: '2018-08-07',
            achievementRate: null, 
            achievementList:[{
                achievementRate:'',
            },{
                achievementRate:'',
            }]
        }
    }
}
</script>

<style lang="less" scoped>
.fallcontain{
    height: 100%;
    overflow-y: auto;
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
