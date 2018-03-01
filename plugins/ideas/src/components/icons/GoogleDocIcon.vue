<template>
  <div>
    <img width=64 height=64 v-if="imgSrc" :src="imgSrc"/>
  </div>
</template>

<script>
  export default {
    name: 'GoogleDocIcon',
    data: () => ({
      imgSrc: null
    }),
    created: function () {
      this.imgSrc = `${process.env.STATIC_ASSETS}/assets/icons/docs_64dp.png`
    }
  }
</script>