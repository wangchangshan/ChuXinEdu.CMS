<template>
  <activity-detail :is-edit='false'></activity-detail>
</template>

<script>
import ActivityDetail from './activityDetail'

export default {
  name: 'createForm',
  components: { ActivityDetail }
}
</script>