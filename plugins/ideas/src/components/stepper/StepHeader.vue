<template>
  <md-button class="md-stepper-header" 
    :class="classes" 
    :disabled="state === 'disabled'" 
    @click.native="clicked">

    <div class="md-stepper-number">
      <EditIcon class="md-stepper-editable" v-if="state === 'active'" />
      <CheckIcon class="md-stepper-done" v-else-if="state === 'done'" />
      <template v-else>{{ index }}</template>
    </div>

    <div class="md-stepper-text">
      <span class="md-stepper-label">{{ label }}</span>
      <span class="md-stepper-description">{{ description }}</span>
    </div>
  </md-button>
</template>

<script>
  import CheckIcon from '@/components/icons/CheckIcon'
  import EditIcon from '@/components/icons/EditIcon'
  export default {
    name: 'StepperHeader',
    components: {
      CheckIcon,
      EditIcon
    },
    props: {
      index: {
        type: Number,
        required: true
      },
      state: {
        type: String,
        required: true
      },
      disabled: {
        type: Boolean,
        default: false
      },
      active: {
        type: Boolean,
        default: false
      },
      done: {
        type: Boolean,
        default: false
      },
      label: {
        type: String,
        default: 'label'
      },
      description: {
        type: String,
        default: 'description'
      }
    },
    computed: {
      classes () {
        return {
          'md-active': this.state === 'active',
          'md-done': this.state === 'done'
        }
      }
    },
    methods: {
      clicked () {
        this.$emit('expand', this.index)
      }
    }
  }
</script>