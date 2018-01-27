<template>
  <div>
    <transition name="fade">
      <div v-if="ideas && ideas.length" class="md-layout md-alignment-top-center">
        <initiative v-for="idea in ideas"
          :key="idea.id" 
          :initiative="idea"
          class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100"
          @onView="openDialog">
        </initiative>
      </div>
    </transition> 
    
    <md-dialog :md-active.sync="showDialog">
      <InitiativeInfo :initiative="shownInitiative" :steps="shownSteps" :active="activeStep"></InitiativeInfo>
    </md-dialog>
  </div>    
</template>

<script>
import Initiative from '@/components/initiative'
import InitiativeInfo from '@/components/ViewInitiative'
import formatNumber from '@/utils/format-number-long'
import Vue from 'Vue'

export default {
  name: 'ViewIdeas',
  data: () => ({
    ideas: [],
    errors: [],
    showDialog: false,
    shownInitiative: null,
    activeStep: null,
    shownSteps: null
  }),
  components: {
    Initiative,
    InitiativeInfo
  },
  methods: {
    openDialog (initiative) {
      console.log('Opening: ' + initiative.id)
      this.setLoading(initiative, true)

      this.services.ideas.getInitiativeSteps(initiative.id).then((response) => {
        let steps = response.data
        let active = 'first'

        for (let i = 0; i < steps.length; i++) {
          // TODO externalise this status somehow.
          if (steps[i].status !== 'done') {
            active = formatNumber(steps[i].step)
            break
          }
        }

        this.activeStep = active
        this.shownSteps = steps
        this.shownInitiative = initiative
        this.setLoading(initiative, false)
        this.showDialog = true
      }).catch((err) => {
        this.errors.push(err)
        this.setLoading(initiative, false)
      })
    },
    setLoading (initiative, state) {
      let foundInit = this.getInitiativeByID(initiative.id)
      let index = this.ideas.indexOf(foundInit)
      let newInit = Vue.util.extend({}, this.ideas[index])
      newInit.isLoading = state
      this.ideas.splice(index, 1, newInit)
    },
    getInitiativeByID (id) {
      for (let i = 0; i < this.ideas.length; i++) {
        if (this.ideas[i].id === id) {
          return this.ideas[i]
        }
      }

      return null
    }
  },
  created () {
    this.ideas.splice(0, this.ideas.length)
    this.services.ideas.getIdeas().then((response) => {
      console.log('received ideas!!')
      this.ideas = response.data
      for (let i = 0; i < this.ideas.length; i++) {
        this.ideas[i].isLoading = false
      }
    }, (e) => {
      this.errors.push(e)
    })
  }
}
</script>