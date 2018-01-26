<template>
  <div class="resource-container">
    <md-avatar class="resource-avatar">
      <img :src="users[0].avatarURL" alt="Avatar">      
    </md-avatar>
    <div>
      <div class="md-body-1">{{ users[0].user }}</div>
      <div class="md-caption">Assigned {{ users[0].assignedOn | formatDate }}</div>
      <md-divider :style="{backgroundColor: color}"></md-divider>
    </div>
  </div>
</template>

<script>
import formatDate from '@/utils/format-date-since'

export default {
  name: 'ResourceStep',
  props: [
    'color',
    'users'
  ],
  created () {
    console.log('created')
  },
  filters: {
    formatDate
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";

  .resource-container {
    display: flex;
    flex-direction: row;
  }

  .resource-avatar {
    margin: auto 12px;
  }
</style>
