
<template>
  <div id="app-ideas">
      <router-view :key="$route.name + ($route.params.id || '')"/>
  </div>
</template>

<script>

export default {
  name: 'app'
}
</script>

<style lang="scss">
  @import "~vue-material/dist/theme/engine"; // Import the theme engine
  @import "./colors.scss";

  @include md-register-theme("default", (
    primary: $oct-primary, // The primary color of your application
    accent: $oct-accent // The accent or secondary color
  ));

  // TRANSITIONS

  .fade-enter-active, .fade-leave-active {
  transition: opacity 1s;
  }
  .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
    opacity: 0;
  }
  .review-off .md-progress-bar-fill {
    background-color: var(--status-review) !important;
  }
  .complete-off .md-progress-bar-fill {
    background-color: var(--status-complete) !important;
  }
  .submit-off .md-progress-bar-fill {
    background-color: var(--status-submit) !important;
  }
  .tooltip {
    display: block !important;
    z-index: 10000;
    .tooltip-inner {
      background: #f9f9f9;
      color: #000;
      padding: 24px;
      border-radius: 5px;
      box-shadow: 0 5px 30px rgba(0,0,0,.1);
    }
    .tooltip-arrow {
      width: 0;
      height: 0;
      border-style: solid;
      position: absolute;
      margin: 5px;
      border-color: #f9f9f9;
      z-index: 1;
    }
    &[x-placement^="top"] {
      margin-bottom: 5px;
      .tooltip-arrow {
        border-width: 5px 5px 0 5px;
        border-left-color: transparent !important;
        border-right-color: transparent !important;
        border-bottom-color: transparent !important;
        bottom: -5px;
        left: calc(50% - 5px);
        margin-top: 0;
        margin-bottom: 0;
      }
    }
    &[aria-hidden='true'] {
      visibility: hidden;
      opacity: 0;
      transition: opacity .15s, visibility .15s;
    }
    &[aria-hidden='false'] {
      visibility: visible;
      opacity: 1;
      transition: opacity .15s;
    }
  }

  @import "~vue-material/dist/theme/all"; // Apply the theme
</style>
