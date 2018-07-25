<template>
  <div>
    <md-card class="item wf-transition">{{ item.id }} {{ item.description }}
    </md-card>
  </div>
</template>
<script>
// import formatDate from '@/utils/format-date-since'
import DiviButton from '@/components/divi/DiviButton'

export default {
  name: 'test',
  props: [
    'item'
  ],
  components: {
    DiviButton
  },
  data: () => ({
  }),
  methods: {
  },
  created () {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import '../mixins.scss';
  body {
    margin: 5px;
    font-family: Helvetica, sans-serif;
  }
  .item {
    position: absolute;
    top: 5px;
    left: 5px;
    right: 5px;
    bottom: 5px;
    // border: 5px solid black;
  }
  .item:after {
    content: attr(index);
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    -webkit-transform: translate(-50%, -50%);
    -ms-transform: translate(-50%, -50%);
  }
  .wf-transition {
    transition: opacity .3s ease;
    -webkit-transition: opacity .3s ease;
  }
  .wf-enter {
    opacity: 0;
  }
</style>
