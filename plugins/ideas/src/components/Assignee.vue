<template>
  <div class="assignee-container">
    <md-avatar class="assignee-avatar">
      <img :src="user.avatarURL" alt="?">      
    </md-avatar>
    <div>
      <a :href="'mailto:' + user.email" class="md-body-1">{{ user.name }}</a>
      <div class="md-caption">{{ user.phoneNumber }}</div>
      <md-divider :style="{backgroundColor: color}"></md-divider>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Assignee',
  props: [
    'color',
    'user'
  ],
  created () {
    if (!this.user.avatarURL) {
      console.log('No avatarURL provided.')
      this.user.avatarURL = 'https://i.imgur.com/FD51R30.png'
    }
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";

  .assignee-container {
    display: flex;
    flex-direction: row;
  }

  .assignee-avatar {
    margin: auto 12px;
  }
</style>
