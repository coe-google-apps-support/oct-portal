<template>
  <div class="oct-scrolly md-scrollbar md-content md-layout md-alignment-top-center" v-if="initiative && initiative.title">    
    <div id="oct-base" class="md-layout-item md-size-66">
      <div class="oct-title-content">
        <span class="md-display-2">{{ initiative.title }}</span>
        <md-divider class="oct-divider"></md-divider>
        <div class="md-caption">{{ initiative.createdDate | formatDate}}</div>
      </div>

      <md-steppers md-vertical :md-active-step="active">
        <md-step v-for="step in steps" 
          :key="step.step" 
          :id="step.step | formatNumber" 
          :md-label="step.name" 
          :md-active-step.sync="active"
          :md-description="getCompletion(step.completedDate)">

          <TextStep v-if="step.type==='text'">{{ step.data }}</TextStep>
          <ChatStep v-else-if="step.type==='chat'"></ChatStep>
          <ResourceStep v-else-if="step.type==='resource'" :color="getColor(initiative)" :users="step.data"></ResourceStep>
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
</template>

<script>
import formatDate from '@/utils/format-date-since'
import formatNumber from '@/utils/format-number-long'
import BurndownStep from '@/components/steps/burndown'
import ChatStep from '@/components/steps/chat'
import TextStep from '@/components/steps/text'
import ResourceStep from '@/components/steps/resource'

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
    TextStep,
    ResourceStep
  },
  // Fetches ideas when the component is created.
  created () {
    console.log('created')
    this.initiative = {}
    this.services.ideas.getInitiative(this.id).then((response) => {
      this.initiative = response
    }).catch((err) => {
      this.errors.push(err)
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
    }).catch((err) => {
      this.errors.push(err)
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
    getColor (idea) {
      // const colors = [
      //   '#e57373',
      //   '#F06292',
      //   '#BA68C8',
      //   '#9575CD',
      //   '#7986CB',
      //   '#64B5F6',
      //   '#4FC3F7',
      //   '#4DD0E1',
      //   '#4DB6AC',
      //   '#81C784',
      //   '#AED581',
      //   '#DCE775',
      //   '#FFF176',
      //   '#FFD54F',
      //   '#FFB74D',
      //   '#FF8A65'
      // ]
      const colors = [
        '#4DB6AC'
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % colors.length
      return colors[randIndex]
    },
    getCompletion (date) {
      if (!date) {
        return ''
      }

      return `Completed ${formatDate(date)}`
    },
    goBack () {
      this.$router.push('/ViewIdeas')
    }
  }
}
</script>

<style lang="scss" scoped>
  @import "../colors.scss";

  .oct-scrolly {
    overflow-y: scroll;
  }

  .oct-divider {
    background-color: $oct-primary;
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
    background-color: #FEFEFE;
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
