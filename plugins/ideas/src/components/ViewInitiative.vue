<template>
  <div class="md-layout md-alignment-top-center" v-if="initiative && initiative.title" :style="{backgroundColor: getColor(initiative)}">
    <div id="oct-base" class="md-layout-item md-size-66">
      <div>
        <span class="md-display-2">{{ initiative.title }}</span>
        <div class="underline"></div>
        <div class="md-caption">{{ initiative.createdDate | formatDate}}</div>
      </div>

      <md-steppers md-vertical :md-active-step="active">
        <md-step v-for="step in steps" 
          :key="step.step" 
          :id="step.step | formatNumber" 
          :md-label="step.name" 
          :md-active-step.sync="active"
          md-description="Description">

          <TextStep v-if="step.type==='text'">{{ step.data }}</TextStep>
          <ChatStep v-else-if="step.type==='chat'"></ChatStep>
          <BurndownStep v-else-if="step.type==='burndown'"></BurndownStep>
        </md-step>
      </md-steppers>
    </div>    
  </div>
</template>

<script>
import formatDate from '@/utils/format-date-since'
import formatNumber from '@/utils/format-number-long'
import BurndownStep from '@/components/steps/burndown'
import ChatStep from '@/components/steps/chat'
import TextStep from '@/components/steps/text'

export default {
  name: 'ViewInitiative',
  props: ['id'],
  data: () => ({
    active: 'first',
    initiative: {},
    steps: {},
    errors: []
  }),
  components: {
    BurndownStep,
    ChatStep,
    TextStep
  },
  // Fetches ideas when the component is created.
  created () {
    console.log('created')
    this.initiative = {}
    this.services.ideas.getInitiative(this.id).then((response) => {
      this.initiative = response
    })

    this.services.ideas.getInitiativeSteps(this.id).then((response) => {
      this.steps = response.data
    })
  },
  filters: {
    formatDate,
    formatNumber,
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
