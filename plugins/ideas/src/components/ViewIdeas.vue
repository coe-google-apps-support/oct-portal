<template>
  <div>
    <br>
    <transition name="fade">
      <div class="min-height">
        <div v-if="ideas && ideas.length" class="md-layout md-alignment-top-center">
          <initiative v-for="idea in ideas"
            :key="idea.id" 
            :initiative="idea"
            :isNewIdea="idea.id === newInitiative"
            class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100">
          </initiative>
        </div>
        <div v-if="ideas.length === 0 && isLoading == false && !filter">
          <img src="https://octava.blob.core.windows.net/cdn-store/empty-state-coe.png" class="center">
          <md-empty-state
            md-label="No initiatives found!"
            md-description="Oops! We couldn't find anything.">
          </md-empty-state>
        </div>
        <div v-if="ideas.length === 0 && isLoading == false && filter === 'mine'">
          <img src="https://octava.blob.core.windows.net/cdn-store/empty-state-coe.png" class="center">
          <md-empty-state
            md-label="You have not made any initiatives!"
            md-description="Oops! We couldn't find anything.">
            <md-button v-on:click="openURL('/new-idea') " class="md-primary md-raised">Create first initiative</md-button>
          </md-empty-state>
        </div>
        <div v-if="isLoading" class="md-layout md-alignment-center-center">
          <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
        </div>
        <div class="center-children" v-if="!isLoading && !isLast && (!this.$options.propsData.page && !this.$options.propsData.pageSize)">
          <divi-button class="md-accent" @click.native="infiniteHandler">Load more</divi-button>
        </div>
        <md-snackbar v-if="newInitiative" md-position="center" :md-duration="Infinity" :md-active.sync="showSnackbar" md-persistent>
          <span>Initiative successfully submitted!</span>
          <md-button class="md-accent" @click="showSnackbar = false">Close</md-button>
        </md-snackbar>
      </div>
    </transition>
    
  </div>    
</template>

<script>

// TODO: Add a watcher for the components properties
// https://stackoverflow.com/questions/49041787/child-component-doesnt-get-updated-when-props-changes
// This could be important if we wanted to allow paging from outside an iframe, for example.

import Initiative from '@/components/initiative'
import DiviButton from '@/components/divi/DiviButton'

export default {
  name: 'ViewIdeas',
  props: {
    filter: String,
    contains: String,
    newInitiative: Number,
    page: Number,
    pageSize: Number
  },
  data: () => ({
    ideas: [],
    errors: [],
    dataPage: null,
    dataPageSize: null,
    initiativeFunction: null,
    isLast: false,
    isLoading: true,
    showSnackbar: false
  }),
  components: {
    Initiative,
    DiviButton
  },
  methods: {
    openURL (url) {
      window.open(url, '_top')
    },
    checkIsLast (response) {
      var x = response.headers['x-is-last-page']
      if (x === 'False') {
        this.isLast = false
      } else if (x === 'True') {
        this.isLast = true
      }
    },
    requestAPI (page, pageSize, contains) {
      this.isLoading = true
      return this.initiativeFunction(page, pageSize, contains).then((response) => {
        this.checkIsLast(response)
        this.ideas = this.ideas.concat(response.data)
        this.isLoading = false
        return response
      }, (e) => {
        this.errors.push(e)
      })
    },
    infiniteHandler () {
      // PR: Investigate actual debouncing https://medium.com/vuejs-tips/tiny-debounce-for-vue-js-cea7a1513728
      setTimeout(() => {
        if (!this.isLast) {
          this.dataPage++
          this.requestAPI(this.dataPage, this.dataPageSize, this.contains)
        }
      }, 1000)
    }
  },
  created () {
    console.log('created')
    // I couldn't get prop defaults to play nicely so I went with this.
    // TODO figure out better defaults props/data
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
    this.ideas.splice(0, this.ideas.length)

    if (this.filter === 'mine') {
      this.initiativeFunction = this.services.ideas.getMyInitiatives
    } else {
      this.initiativeFunction = this.services.ideas.getIdeas
    }

    // this.requestAPI(this.dataPage, this.dataPageSize, this.contains)
    this.requestAPI(this.dataPage, this.dataPageSize, this.contains).then((response) => {
      if (!isNaN(this.newInitiative)) {
        this.showSnackbar = true
      }
    })
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

  .center {
    display: block;
    margin-left: auto;
    margin-right: auto;
    width: 500px;
    height: auto;
  }

  .center-children {
    padding: 30px;
    display: flex;
    justify-content: center;
  }
</style>
