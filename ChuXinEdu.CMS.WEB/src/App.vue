<template>
  <div id="app">
    <router-view/>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style lang="less">
    @import './style/cover/element-ui';//修正element-ui的自定义样式
    @import './style/base/common';//修正element-ui的自定义样式
</style>
