
<template>
  <div id="app-ideas">
      <router-view/>
  </div>
</template>

<script>

import {bus} from './main'

export default {
  name: 'app',
  mounted () {
    bus.$on('initialize-ideas', config => {
      if (config.route) {
        this.$router.push(config.route)
      }
    })
  }
}
</script>

<style>
/* #app-ideas {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
} */
</style>
