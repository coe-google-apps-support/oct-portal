<template>
  <div>
    <transition name="fade">
      <div>
        <div v-if="currentCards && currentCards.length" class="md-layout md-alignment-top-center">
          <initiative v-for="idea in currentCards"
            :key="idea.id" 
            :initiative="idea"
            :isNewIdea='idea.id === newInitiative && filter === "mine"'
            class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100">
          </initiative>
        </div>
        <div v-if="ideas[currentCards.length + 1] != null">
          <md-button class='loadMore md-raised md-secondary' v-on:click='infiniteHandler'> Load More </md-button></div>
        <infinite-loading @infinite="infiniteHandler">
          <span slot="no-more">
            There are no more initiatives!
          </span>
        </infinite-loading> 
      </div>
    </transition>
    
  </div>    
</template>

<script>
import Initiative from '@/components/initiative'
import InfiniteLoading from 'vue-infinite-loading'
import vue from 'vue'

export default {
  name: 'ViewIdeas',
  props: {
    filter: String,
    newInitiative: Number
  },
  data: () => ({
    ideas: [],
    errors: [],
    showDialog: false,
    shownInitiative: null,
    activeStep: null,
    shownSteps: null,
    firstInit: null,
    currentCards: []
  }),
  components: {
    Initiative,
    InfiniteLoading
  },
  methods: {
    setLoading (initiative, state) {
      let foundInit = this.getInitiativeByID(initiative.id)
      let index = this.ideas.indexOf(foundInit)
      let newInit = vue.util.extend({}, this.ideas[index])
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
    },
    infiniteHandler ($state) {
      const self = this
      setTimeout(() => {
        const temp = []
        for (let i = self.currentCards.length; i <= self.currentCards.length + 19; i++) {
          if (self.ideas[i] != null) {
            temp.push(self.ideas[i])
          }
        }
        self.currentCards = self.currentCards.concat(temp)
        if ($state.loaded) {
          $state.loaded()
        }
        if (self.ideas[self.currentCards.length + 1] == null) {
          if ($state.complete) {
            $state.complete()
          }
        }
      }, 1000)
    }
  },
  created () {
    console.log(this.newInitiative)
    console.log(this.filter)
    if (this.newInitiative !== null && this.filter === 'mine') {
      this.$toasted.show('Initiative successfully submitted!', {
        theme: 'primary',
        position: 'top-right',
        icon: 'check_circle',
        action: {
          text: 'Close',
          onClick: (e, toastObject) => {
            toastObject.goAway(0)
          }
        }
      })
    }
    this.ideas.splice(0, this.ideas.length)

    let initiativeFunction = null
    if (this.filter === 'mine') {
      initiativeFunction = this.services.ideas.getMyInitiatives
    } else {
      initiativeFunction = this.services.ideas.getIdeas
    }

    initiativeFunction().then((response) => {
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
  .loadMore {
    position: relative;
    width: 20%;
    right: -40%;
  }

</style>
