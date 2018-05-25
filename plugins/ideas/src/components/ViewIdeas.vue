<template>
  <div>
    <br>
    <transition name="fade">
      <div class="min-height">
        <div v-if="ideas && ideas.length" class="md-layout md-alignment-top-center">
          <initiative v-for="idea in ideas"
            :key="idea.id" 
            :initiative="idea"
            :isNewIdea='idea.id === newInitiative && filter === "mine"'
            class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100">
          </initiative>
        </div>
        <div v-if="isLoading" class="md-layout md-alignment-center-center">
          <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
        </div>
        <div v-if="!isLoading && !isLast && (!this.$options.propsData.page && !this.$options.propsData.pageSize)">
          <md-button class='loadMore md-raised md-secondary' v-on:click='infiniteHandler'>Load More</md-button>
        </div>
      </div>
    </transition>
    
  </div>    
</template>

<script>

// TODO: Add a watcher for the components properties
// https://stackoverflow.com/questions/49041787/child-component-doesnt-get-updated-when-props-changes
// This could be important if we wanted to allow paging from outside an iframe, for example.

import Initiative from '@/components/initiative'
import InfiniteLoading from 'vue-infinite-loading'
import vue from 'vue'

export default {
  name: 'ViewIdeas',
  props: {
    filter: String,
    newInitiative: Number,
    page: Number,
    pageSize: Number
  },
  data: () => ({
    ideas: [],
    errors: [],
    dataPage: null,
    dataPageSize: null,
    showDialog: false,
    shownInitiative: null,
    activeStep: null,
    shownSteps: null,
    firstInit: null,
    redir: false,
    initiativeFunction: null,
    isLast: false,
    newInitId: null,
    isLoading: true
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
    checkIslast (page) {
      let checkdata = null

      this.initiativeFunction(page + 1, this.pageSize).then((response) => {
        checkdata = response.data
        if (checkdata.length === 0) {
          this.isLast = true
        } else {
          this.isLast = false
        }
      })
    },
    requestAPI (page, pageSize) {
      this.isLoading = true
      this.initiativeFunction(page, pageSize).then((response) => {
        this.ideas = this.ideas.concat(response.data)
        this.isLoading = false
      }, (e) => {
        this.errors.push(e)
      })
    },
    infiniteHandler ($state) {
      console.log(this.redir)
      // PR: Investigate actual debouncing https://medium.com/vuejs-tips/tiny-debounce-for-vue-js-cea7a1513728
      setTimeout(() => {
        if (!this.isLast) {
          this.dataPage++
          console.log(`Loading page ${this.dataPage}.`)
          this.requestAPI(this.dataPage, this.dataPageSize)
        }
        if ($state.loaded) {
          $state.loaded()
        }
        this.checkIslast(this.dataPage)
        if (this.isLast && $state.complete) {
          $state.complete()
        }
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
    console.log('created')
    // I couldn't get prop defaults to play nicely so I went with this.
    if (this.page) {
      this.dataPage = this.page
    } else {
      this.dataPage = 1
    }
    if (this.pageSize) {
      this.dataPageSize = this.pageSize
    } else {
      this.dataPageSize = 20
    }
    console.log(this.dataPageSize)
    console.log(this.dataPage)
    this.ideas.splice(0, this.ideas.length)

    if (this.filter === 'mine') {
      this.initiativeFunction = this.services.ideas.getMyInitiatives
      this.requestAPI(this.dataPage, this.dataPageSize).then((response) => {
        this.ideas = this.ideas.concat(response.data)
        this.newInitId = this.ideas[0].id
        if (!isNaN(this.newInitiative)) {
          this.toastMessage('Initiative successfully submitted!')
        }
      }, (e) => {
        this.errors.push(e)
      })
    } else {
      this.initiativeFunction = this.services.ideas.getIdeas
      this.requestAPI(this.dataPage, this.dataPageSize)
    }
  }
}
</script>
<style>
  .min-height {
    min-height: 400px;
  }
  .md-overlay.md-fixed.md-dialog-overlay {
    z-index: 9!important;
  }
  .loadMore {
    position: relative;
    width: 20%;
    right: -40%;
  }

</style>
