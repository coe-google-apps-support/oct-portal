<template>
  <div class="md-steppers" :class="[steppersClasses]">
    <div class="md-steppers-wrapper" :style="contentStyles">
      <div class="md-steppers-container" :style="containerStyles">
        <div class="md-stepper" 
          v-for="(_, index) in steps" 
          :key="index" >

          <StepHeader 
            :index="index + 1" 
            :state="getHeaderState(steps[index])" 
            :label="steps[index].title"
            :description="getCompletion(steps[index].completionDate)"
            v-on:expand="expand"/>

          <div class="md-stepper-content" :class="{ 'md-active': stepStates[index] }">
            {{ steps[index].description }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import formatDate from '@/utils/format-date-since'
import formatNumber from '@/utils/format-number-long'
import StepHeader from '@/components/stepper/StepHeader'

export default {
  name: 'Steps',
  props: [
    'steps'
  ],
  data: () => ({
    containerStyles: {},
    contentStyles: {},
    stepStates: [],
    headerState: {
      ACTIVE: 'active',
      DISABLED: 'disabled',
      DONE: 'done'
    }
  }),
  computed: {
    steppersClasses () {
      return {
        'md-vertical': true,
        'md-dynamic-height': true,
        'md-theme-default': true
      }
    }
  },
  components: {
    StepHeader
  },
  created: function () {
    for (let i = 0; i < this.steps.length; i++) {
      if (this.steps[i].startDate && !this.steps[i].completionDate) {
        this.stepStates[i] = true
      } else {
        this.stepStates[i] = false
      }
    }
  },
  filters: {
    formatDate,
    formatNumber
  },
  methods: {
    expand (stepNumber) {
      this.updateArray(this.stepStates, stepNumber - 1, !this.stepStates[stepNumber - 1])
    },
    getCompletion (date) {
      if (!date) {
        return ''
      }

      return `Completed ${formatDate(date)}`
    },
    getHeaderState (step) {
      if (step.completionDate) {
        return 'done'
      } else if (step.startDate) {
        return 'active'
      } else {
        return 'disabled'
      }
    },
    updateArray (array, index, value) {
      array.splice(index, 1, value)
    }
  }
}
</script>
<style lang="scss">
  @import "node_modules/vue-material/src/components/MdAnimation/variables";
  @import "node_modules/vue-material/src/components/MdElevation/mixins";
  @import "node_modules/vue-material/src/components/MdLayout/mixins";
  
  $md-stepper-icon-size: 24px;
  .md-steppers {
    transition: .3s $md-transition-default-timing;
    transition-property: color, background-color;
    will-change: color, background-color;
    &.md-no-transition * {
      transition: none !important;
    }
    &.md-dynamic-height .md-steppers-wrapper {
      transition: height .3s $md-transition-default-timing;
      will-change: height;
    }
    &.md-horizontal.md-alternative {
      .md-stepper-header {
        height: 104px;
        &:first-of-type {
          .md-stepper-icon,
          .md-stepper-number {
            &:before {
              content: none;
            }
          }
        }
        &:last-of-type {
          .md-stepper-icon,
          .md-stepper-number {
            &:after {
              content: none;
            }
          }
        }
        .md-ripple {
          justify-content: center;
        }
        .md-button-content {
          padding-top: 16px;
          flex-direction: column;
          &:before,
          &:after {
            content: none;
          }
        }
        .md-stepper-text {
          height: 32px;
          justify-content: flex-start;
          text-align: center;
        }
        .md-stepper-icon,
        .md-stepper-number {
          margin: 0 8px 8px;
          position: relative;
          &:after,
          &:before {
            width: 9999%;
            height: 1px;
            position: absolute;
            top: 50%;
            z-index: 2;
            transition: background-color .3s $md-transition-default-timing;
            will-change: background-color;
            content: " ";
          }
          &:after {
            left: calc(100% + 8px);
          }
          &:before {
            right: 32px;
          }
        }
      }
    }
    &.md-vertical {
      .md-stepper-header {
        height: 56px;
        .md-ripple {
          padding: 0 24px 0 16px;
        }
      }
      .md-steppers-container {
        flex-direction: column;
      }
      .md-button-content {
        &:before,
        &:after {
          content: none;
        }
      }
      .md-stepper-icon,
      .md-stepper-number {
        margin-right: 12px;
      }
      .md-stepper {
        padding: 0;
        position: relative;
        &:last-of-type:after {
          content: none;
        }
        &:after {
          width: 1px;
          position: absolute;
          top: 48px;
          bottom: -8px;
          left: 36px;
          z-index: 2;
          transition: background-color .3s $md-transition-default-timing;
          will-change: background-color;
          content: " ";
        }
      }
    }
  }
  .md-steppers-navigation {
    @include md-elevation(2);
    display: flex;
    .md-stepper-header {
      width: auto;
    }
  }
  .md-stepper-header {
    width: 100%;
    height: 72px;
    margin: 0;
    flex: 1;
    border-radius: 0;
    font-weight: 400;
    text-align: left;
    text-transform: none;
    &:first-of-type .md-button-content:before {
      content: none;
    }
    &:last-of-type .md-button-content:after {
      content: none;
    }
    &.md-active,
    &.md-error {
      font-weight: 500;
    }
    .md-ripple {
      padding: 0 16px;
      justify-content: flex-start;
    }
    .md-button-content {
      padding: 0 8px;
      display: flex;
      align-items: center;
      transition: color .3s $md-transition-default-timing;
      will-change: color;
      &:after,
      &:before {
        height: 1px;
        position: absolute;
        top: 50%;
        transition: background-color .3s $md-transition-default-timing;
        will-change: background-color;
        content: " ";
      }
      &:after {
        width: 9999%;
        left: 100%;
      }
      &:before {
        width: 16px;
        left: -16px;
      }
      svg {
        transition: .3s $md-transition-default-timing;
        transition-property: color, fill;
        will-change: color, fill;
      }
    }
  }
  .md-stepper-text {
    display: flex;
    flex-direction: column;
    justify-content: center;
    line-height: 16px;
    white-space: nowrap;
  }
  .md-stepper-icon,
  .md-stepper-number {
    margin-right: 8px;
    transition: color .3s $md-transition-default-timing;
    will-change: color;
  }
  .md-stepper-number {
    width: $md-stepper-icon-size;
    height: $md-stepper-icon-size;
    border-radius: $md-stepper-icon-size;
    transition: .3s $md-transition-default-timing;
    transition-property: color, background-color;
    will-change: color, background-color;
    font-size: 12px;
    line-height: $md-stepper-icon-size;
    text-align: center;
  }
  .md-stepper-done {
    width: 20px;
    height: 20px;
    transform: translateY(-1px);
  }
  .md-stepper-editable {
    width: 14px;
    height: 14px;
    transform: translateY(-1px);
  }
  .md-stepper-error,
  .md-stepper-description {
    font-size: 12px;
    font-weight: 400;
    line-height: 16px;
  }
  .md-stepper-description {
    opacity: .54;
  }
  .md-steppers-wrapper {
    overflow: hidden;
    transition: none;
    will-change: height;
  }
  .md-steppers-container {
    display: flex;
    align-items: flex-start;
    flex-wrap: nowrap;
    transform: translateZ(0);
    transition: transform .35s $md-transition-default-timing;
    will-change: transform;
  }
  .md-stepper {
    width: 100%;
    flex: 1 0 100%;
    padding: 16px 24px;
    @include md-layout-small {
      padding: 8px 16px;
    }
  }
</style>