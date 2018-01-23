<template>
  <div>
    There is burndown here
  </div>
</template>

<script>
export default {
  name: 'BurndownStep',
  data: () => ({
    contents: {}
  }),
  created () {
    console.log('created')
  },
  filters: {

  },
  methods: {

  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";
</style>
