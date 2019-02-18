<template>
  <activity-detail :is-edit='true'></activity-detail>
</template>

<script>
import ActivityDetail from './activityDetail'

export default {
  name: 'createForm',
  components: { ActivityDetail }
}
</script>