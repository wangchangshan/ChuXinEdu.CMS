<template>
<div class="fillcontain">
    <el-tabs type="card" @tab-click="handleTabClick">
        <el-tab-pane v-for="room in allRooms" :key="room.code" :label="room.title" :name="room.code">
            <span slot="label" @click="toggleTabs(room.code)"><i class="fa fa-bank"></i>{{room.title}}</span>
            <!-- {{room.content}} -->
            <course-arrange-panel :currentRoomCode="selectedRoomCode"></course-arrange-panel>
            <!-- 
            -->
        </el-tab-pane>
    </el-tabs>
</div>
</template>

<script>
import courseArrangePanel from '@/components/courseArrangePanel'

export default {
    data() {
        return {
            defaultRoomCode: '',
            selectedRoomCode: '',
            allRooms: [{
                title: '1号教室',
                code: 'room1',
                content: '1号教室内容'
            },{
                title: '2号教室',
                code: 'room2',
                content: '2号教室内容'
            }],
        }
    },
    components: {
        courseArrangePanel
    },
    methods: {
        toggleTabs(roomCode){
            this.selectedRoomCode = roomCode;
        },
        handleTabClick() {
            
        },
        handleChange(){

        }
    },
    mounted() {
        //this.defaultRoomCode = this.allRooms[0].code;
        this.toggleTabs(this.allRooms[1].code);
    },
}
</script>

<style lang="less" scoped>

</style>
