<template>
  <div>
    <div align='center'> <h5>Page: 1 2 3 4 ... </h5> </div>
    <br>
    <transition name="fade">
      <div>
        <div v-if="ideas && ideas.length" class="md-layout md-alignment-top-center">
          <initiative v-for="idea in ideas"
            :key="idea.id" 
            :initiative="idea"
            :isNewIdea='idea.id === newInitiative && filter === "mine"'
            class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100">
          </initiative>
        </div>
        <!-- <div v-if="ideas[ideas.length + 1] != null"> -->
        <div>  
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
    newInitiative: Number,
    page: {
      default () {
        return 1
      },
      type: Number
    },
    pageSize: {
      default () {
        return 20
      },
      type: Number
    }
  },
  data: () => ({
    ideas: [],
    errors: [],
    showDialog: false,
    shownInitiative: null,
    activeStep: null,
    shownSteps: null,
    firstInit: null,
    // ideas: [],
    redir: false,
    initiativeFunction: null
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
    getNextPage (page, pageSize) {
      this.initiativeFunction('?page=' + String(page) + '&pageSize=' + String(pageSize)).then((response) => {
        this.ideas = this.ideas.concat(response.data)
        for (let i = 0; i < this.ideas.length; i++) {
          this.ideas[i].isLoading = false
        }
      }, (e) => {
        this.errors.push(e)
      })
    },
    infiniteHandler ($state) {
      let page = this.page
      setTimeout(() => {
        if (this.redir === true && this.filter !== 'mine') {
          page++
          this.getNextPage(page, this.pageSize)
          this.$router.push({path: '/view-ideas', query: {page: page, pageSize: this.pageSize}})
        }
        // if ($state.loaded) {
        //   $state.loaded()
        // }
        let checkdata = null
        console.log(this.filter)
        this.initiativeFunction('?page=' + String(page + 1) + '&pageSize=1').then((response) => {
          checkdata = response.data
          console.log(checkdata)
          if (checkdata == null) {
            if ($state.complete) {
              $state.complete()
            }
          }
        })
        this.redir = true
      }, 1000)
    },
    toastMessage (message) {
      this.$toasted.show(message, {
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
  },
  created () {
    if (this.filter !== 'mine') {
      if (isNaN(this.page) || isNaN(this.pageSize)) {
        this.$router.push({path: '/view-ideas', query: {page: 1, pageSize: 20}})
      } else {
        this.$router.push({path: '/view-ideas', query: {page: this.page, pageSize: this.pageSize}})
      }
    }

    if (this.newInitiative !== null && this.filter === 'mine') {
      this.toastMessage('Initiative successfully submitted!')
    }
    this.ideas.splice(0, this.ideas.length)

    if (this.filter === 'mine') {
      this.initiativeFunction = this.services.ideas.getMyInitiatives
    } else {
      this.initiativeFunction = this.services.ideas.getIdeas
    }
    this.getNextPage(this.page, this.pageSize)
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
