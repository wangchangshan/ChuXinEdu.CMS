<template>
<div class="comment-card">
    <div class="bottom clearfix">
        <time class="time">{{ time }}</time>
        <button type="button" class="el-button button el-button--text">
                        <span>编辑</span>
                    </button>
        <button type="button" class="el-button button el-button--text">
                        <span>删除</span>
                    </button>
    </div>
    <span>
                {{ content }}
    </span>
</div>
</template>

<script>
export default {
    name: 'comment',
    props: {
        id: {
            type: Number
        },
        time: {
            type: String
        },
        content: {
            type: String
        }
    },
    data() {
        return {
        }
    },
    mounted() {
        this.initComment()
    },
    methods: {
        initComment() {

        }
    },
}
</script>

<style lang="less" scoped>
.comment-card {
    border: 1px solid #ebeef5;
    background-color: #ffe;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    margin-bottom: 5px;
    box-shadow: 0 0px 0px 0 rgba(0, 0, 0, 0.1);
    padding: 5px 10px 10px 10px;
}

.time {
    font-size: 13px;
    color: #999;
    margin-right: 10px;
}
</style>
