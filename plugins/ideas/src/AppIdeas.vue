
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

  // Issue Status background colors

  .issue-new .md-progress-bar {
    background-color: var(--status-new-alt) !important;
  }
  .issue-new .md-progress-bar-fill {
    background-color: var(--status-new) !important;
  }
  .issue-new.color-corner:after {
    border-color: var(--status-new) var(--status-new) transparent transparent;
  }

  .issue-assigned .md-progress-bar-fill {
    background-color: var(--status-assigned) !important;
  }
  .issue-assigned .md-progress-bar {
    background-color: var(--status-assigned-alt) !important;
  }
  .issue-assigned.color-corner:after {
    border-color: var(--status-assigned) var(--status-assigned) transparent transparent;
  }

  .issue-inprogress .md-progress-bar-fill {
    background-color: var(--status-inprogress) !important;
  }
  .issue-inprogress .md-progress-bar {
    background-color: var(--status-inprogress-alt) !important;
  }
  .issue-inprogress.color-corner:after {
    border-color: var(--status-inprogress) var(--status-inprogress) transparent transparent;
  }

  .issue-resolved .md-progress-bar-fill {
    background-color: var(--status-resolved) !important;
  }
  .issue-resolved .md-progress-bar {
    background-color: var(--status-resolved-alt) !important;
  }
  .issue-resolved.color-corner:after {
    border-color: var(--status-resolved) var(--status-resolved) transparent transparent;
  }

  .issue-closed .md-progress-bar-fill {
    background-color: var(--status-closed) !important;
  }
  .issue-closed .md-progress-bar {
    background-color: var(--status-closed-alt) !important;
  }
  .issue-closed.color-corner:after {
    border-color: var(--status-closed) var(--status-closed) transparent transparent;
  }

  .issue-cancelled .md-progress-bar-fill {
    background-color: var(--status-cancelled) !important;
  }
  .issue-cancelled .md-progress-bar {
    background-color: var(--status-cancelled-alt) !important;
  }
  .issue-cancelled.color-corner:after {
    border-color: var(--status-cancelled) var(--status-cancelled) transparent transparent;
  }

  .issue-pending .md-progress-bar-fill {
    background-color: var(--status-pending) !important;
  }
  .issue-pending .md-progress-bar {
    background-color: var(--status-pending-alt) !important;
  }
  .issue-pending.color-corner:after {
    border-color: var(--status-pending) var(--status-pending) transparent transparent;
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
