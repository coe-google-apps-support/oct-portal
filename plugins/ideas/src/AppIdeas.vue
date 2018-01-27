
<template>
  <div id="app-ideas">
      <router-view :key="$route.name + ($route.params.id || '')"/>
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

<style lang="scss">
  @import "~vue-material/dist/theme/engine"; // Import the theme engine
  @import "./colors.scss";

  @include md-register-theme("default", (
    primary: $oct-primary, // The primary color of your application
    accent: $oct-accent // The accent or secondary color
  ));

  @import "~vue-material/dist/theme/all"; // Apply the theme
</style>
