<template>
  <div>
      <md-button id="attach-file">
          <google-doc-icon style="margin-bottom: 6px"></google-doc-icon>
          <div v-if="!url">Attach</div>
          <div v-else>Open</div>
      </md-button>
  </div>
</template>

<script>
import GoogleDocIcon from '@/components/icons/GoogleDocIcon'

export default {
  name: 'AttachFileButton',
  props: {
    url: String
  },
  components: {
    GoogleDocIcon
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";

  #attach-file {
    height: 100px;
    width: 100px;
  }
</style>
