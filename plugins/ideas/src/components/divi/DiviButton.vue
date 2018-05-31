<template>
  <a class="et_pb_button et_pb_more_button et_pb_button_one" :href="href">
    <slot></slot>
  </a>
</template>

<script>

export default {
  name: 'DiviButton',
  props: {
    href: String
  }
}
</script>

<style lang="scss" scoped>

  /* Undo/block material design CSS */
  /* TODO Delete this */
  .et_pb_button {
    font-weight: bold;
    font-style: normal;
    text-transform: none;
    text-decoration: none;
  }

  .et_pb_more_button {
    display: inline-block;
    margin-top: 0px;
    color: inherit;
    text-shadow: none;
    text-decoration: none;
  }

  .et_pb_button {
    position: relative;
    padding: .3em 1em;
    border: 2px solid;
    -webkit-border-radius: 3px;
    -moz-border-radius: 3px;
    border-radius: 3px;
    background-color: transparent;
    background-repeat: no-repeat;
    background-position: center;
    background-size: cover;
    font-size: 20px;
    font-weight: 500;
    line-height: 1.7em!important;
    -webkit-transition: all .2s;
    -moz-transition: all .2s;
    transition: all .2s;
  }

  .et_pb_button:hover, .et_pb_module .et_pb_button:hover {
    padding: .3em 2em .3em .7em;
    border: 2px solid transparent;
  }

  .et_pb_button:hover {
    background-color: rgba(255,255,255,.2);
  }

  a:hover {
    cursor: pointer;
    text-decoration: none;
  }

</style>