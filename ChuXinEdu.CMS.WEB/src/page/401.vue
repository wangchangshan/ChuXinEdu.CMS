<template>
<div class="wscn-http404-container">
    <div class="wscn-http404">
        <div class="bullshit">
            <div class="bullshit__oops">初心工作室</div>
            <div class="bullshit__headline">{{ message }}</div>
            <div class="bullshit__info">您太久没有活动了，请重新登陆</div>
            <a @click="reLogin" class="bullshit__return-login">重新登陆</a>
        </div>
    </div>
</div>
</template>

<script>
import {
    LocalDB
} from "@/utils/";


export default {
    data() {
        return {}
    },
    computed: {
        message() {
            return '401......'
        }
    },
    methods: {
        reLogin() {
            this.$store.state.menu.isCollapse = false;
            LocalDB.instance("MENU_").remove("LEFTMENU");
            LocalDB.instance("USER_").remove("BASEINFO");
            this.$router.push("/");
            document.location.reload();
        }
    },
}
</script>

<style lang="scss" scoped>
.wscn-http404-container {
    transform: translate(-50%, -50%);
    position: absolute;
    top: 40%;
    left: 50%;
}

.wscn-http404 {
    position: relative;
    padding: 0 50px;
    overflow: hidden;

    .bullshit {
        position: relative;
        float: left;
        width: 300px;
        padding: 30px 0;
        overflow: hidden;

        &__oops {
            font-size: 32px;
            font-weight: bold;
            line-height: 40px;
            color: #1482f0;
            opacity: 0;
            margin-bottom: 20px;
            animation-name: slideUp;
            animation-duration: 0.5s;
            animation-fill-mode: forwards;
        }

        &__headline {
            font-size: 20px;
            line-height: 24px;
            color: #222;
            font-weight: bold;
            opacity: 0;
            margin-bottom: 10px;
            animation-name: slideUp;
            animation-duration: 0.5s;
            animation-delay: 0.1s;
            animation-fill-mode: forwards;
        }

        &__info {
            font-size: 13px;
            line-height: 21px;
            color: grey;
            opacity: 0;
            margin-bottom: 30px;
            animation-name: slideUp;
            animation-duration: 0.5s;
            animation-delay: 0.2s;
            animation-fill-mode: forwards;
        }

        &__return-login {
            display: block;
            float: left;
            width: 110px;
            height: 36px;
            background: #67C23A;
            border-radius: 100px;
            text-align: center;
            color: #ffffff;
            opacity: 0;
            font-size: 14px;
            line-height: 36px;
            cursor: pointer;
            animation-name: slideUp;
            animation-duration: 0.5s;
            animation-delay: 0.3s;
            animation-fill-mode: forwards;
        }

        @keyframes slideUp {
            0% {
                transform: translateY(60px);
                opacity: 0;
            }

            100% {
                transform: translateY(0);
                opacity: 1;
            }
        }
    }
}
</style>
