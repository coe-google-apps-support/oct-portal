<template>
  <div>
      <div class="oct-blocker" @click="goBack"></div>
      <div class="oct-content">        
        <div class="md-layout md-alignment-top-center" v-if="initiative && initiative.title" :style="{backgroundColor: getColor(initiative)}">
          <md-button class="md-icon-button oct-back-button" @click="goBack">
            <md-icon class="md-size-2x">arrow_back</md-icon>
          </md-button>          
          <div id="oct-base" class="md-layout-item md-size-66">
            <div class="oct-title-content">
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
                <BurndownStep v-else-if="step.type==='burndown'" 
                  :color="getColor(initiative)" 
                  :burndown="step" 
                  :title="initiative.title"
                  :description="initiative.description"
                  :date="initiative.createdDate">
                </BurndownStep>
              </md-step>
            </md-steppers>
          </div>    
        </div>
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

      for (let i = 0; i < this.steps.length; i++) {
        // TODO externalise this status somehow.
        if (this.steps[i].status !== 'done') {
          this.active = formatNumber(this.steps[i].step)
          break
        }
      }
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
    },
    goBack () {
      this.$router.push('/ViewIdeas')
    }
  }
}
</script>

<style lang="scss" scoped>

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

  /* card overlay */
  .oct-blocker {
    position: absolute;
    top: 0px;
    bottom: 0px;
    right: 0px;
    left: 0px;
    z-index: 1;
    background-color: #090909;
    opacity: 0.2;
  }

  .oct-content {
    position: absolute;
    width: 800px;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -50%);
    z-index: 2;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 3px 1px -2px rgba(0,0,0,.2), 0 2px 2px 0 rgba(0,0,0,.14), 0 1px 5px 0 rgba(0,0,0,.12);
  }

  .oct-title-content {
    margin: 14px 0px;
  }

  .oct-back-button {
    position: absolute;
    top: 28px;
    left: 28px;
  }
</style>
