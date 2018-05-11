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
        <div v-if="ideas[currentCards.length + 1] != null"><button id='loadMore' v-on:click='infiniteHandler'>Load More </button></div>
        <infinite-loading @infinite="infiniteHandler">
         <!--  -->
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
// import axios from 'axios'
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
      // console.log('starting infinite handler')
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
        if (self.currentCards.length === 0) {
          // console.error('currentCards length is 0.')
        } else {
          // console.log('All good! currentCards length is not 0!')
          // console.log(self.currentCards)
        }
        if (self.ideas[self.currentCards.length + 1] == null) {
          if ($state.complete) {
            $state.complete()
          }
        }
      }, 1000)
      // console.log('finishing infinite handler')
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
      // console.log('Getting my inits')
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

#loadMore {
  position: relative;
  margin: auto;
  width: 20%;
  right: -30%;
  padding: 10px;
  transform: translate(50%, 10%);
  background: rgb(133, 223, 223);
  border: 2px solid #383838;
  border-radius: 4px;
  overflow: hidden;
  transition: .6s;
}
#loadMore:focus {
  outline: none;
}
#loadMore:before {
  content: '';
  display: block;
  position: absolute;
  background: rgba(255,255,255,.5);
  width: 60px;
  height: 100%;
  left: 0;
  top: 0;
  opacity: .5s;
  filter: blur(30px);
  transform: translateX(-130px) skewX(-25deg);
}
#loadMore:after {
  content: '';
  display: block;
  position: absolute;
  background: rgba(255,255,255,.2);
  width: 30px;
  height: 100%;
  left: 30px;
  top: 0;
  opacity: 0;
  filter: blur(30px);
  transform: translate(-100px) scaleX(-15deg);
}
#loadMore:hover {
  background: rgb(0, 182, 182);
  cursor: pointer;
}
#loadMore:hover:before {
  transform: translate(300px) skewX(-15deg);
  opacity: .6;
  transition: 1.5s;
}
#loadMore:hover:after {
  transform: translate(300px) skewX(-15deg);
  opacity: 1;
  transition:.7s;
}
</style>
