<template>
  <div class="md-layout md-alignment-top-center" v-if="initiative && initiative.title" :style="{backgroundColor: getColor(initiative)}">
    <div id="oct-base" class="md-layout-item md-size-66">
      <div>
        <span class="md-display-2">{{ initiative.title }}</span>
        <div class="underline"></div>
        <div class="md-caption">{{ initiative.createdDate | formatDate}}</div>
      </div>
    </div>
    <md-steppers md-vertical>
      <md-step id="first" md-label="Submit" md-description="Optional">
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
      </md-step>

      <md-step id="second" md-label="Review">
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
      </md-step>

      <md-step id="third" md-label="Collaborate">
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
      </md-step>

      <md-step id="fourth" md-label="Deliver">
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
        <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Molestias doloribus eveniet quaerat modi cumque quos sed, temporibus nemo eius amet aliquid, illo minus blanditiis tempore, dolores voluptas dolore placeat nulla.</p>
      </md-step>
    </md-steppers>
  </div>
</template>

<script>
import formatDate from '@/utils/format-date-since'

export default {
  name: 'ViewInitiative',
  props: ['id'],
  data: () => ({
    initiative: {},
    errors: []
  }),
  // Fetches ideas when the component is created.
  created () {
    console.log('created')
    this.initiative = {}
    this.services.ideas.getInitiative(this.id).then((result) => {
      this.initiative = result
    })
  },
  filters: {
    formatDate,
    truncate (str) {
      const MAX_LENGTH = 300
      if (str.length < MAX_LENGTH) {
        return str
      } else {
        return str.slice(0, MAX_LENGTH) + '...'
      }
    }
  },
  methods: {
    getImage (idea) {
      const root = process.env.STATIC_ASSETS
      const images = [
        `${root}/assets/temp/balance-200-white.png`,
        `${root}/assets/temp/card-travel-200-white.png`,
        `${root}/assets/temp/explore-200-white.png`,
        `${root}/assets/temp/help-200-white.png`,
        `${root}/assets/temp/house-200-white.png`
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % images.length
      return images[randIndex]
    },
    getColor (idea) {
      const colors = [
        '#3F51B5',
        '#009688',
        '#4CAF50',
        '#607D8B',
        '#ef5350',
        '#00C853',
        '#FF5722',
        '#E91E63'
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % colors.length
      return colors[randIndex]
    }
  }
}
</script>

<style lang="scss" scoped>
  //@import "~vue-material/dist/theme/engine";

  .underline {
    width: 140px;
    height: 2px;
    border-radius: 1px;
    background-color: currentColor;
    margin: 12px;
  }

  .md-steppers {
    margin: 24px 0px;
  }
</style>
