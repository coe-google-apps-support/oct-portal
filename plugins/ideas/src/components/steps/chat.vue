<template>
  <div>
    There is chat here
  </div>
</template>

<script>
export default {
  name: 'ChatStep',
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
