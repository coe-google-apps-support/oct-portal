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
  </div>    
</template>

<script>
import Initiative from '@/components/initiative'
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
    Initiative
  },
  methods: {
    openDialog (initiative) {
      console.log('Opening: ' + initiative.id)
      window.location.href = initiative.url
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
    console.log('ViewIdeas')
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
<style>
  .md-overlay.md-fixed.md-dialog-overlay {
    z-index: 9!important;
  }
</style>
