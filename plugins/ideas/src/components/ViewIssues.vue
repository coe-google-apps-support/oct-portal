<template>
  <div>
    <waterfall :line-gap="500" :watch="items" line="v" align="center" fixed-height="false">
      <waterfall-slot
        v-for="(item, index) in items"
        :width="item.width"
        :height="item.height"
        :order="index"
        :key="item.id">
        <test class="md-layout md-alignment-top-center" :item="item"></test>
        <!-- <issue v-for="issue in issues"
          :key="issue.id" 
          :issue="issue"
          >
        </issue> -->
      </waterfall-slot>
    </waterfall>
    <div v-if="issues.length === 0 && isLoading == false">
      <md-empty-state
        md-icon="confirmation_number"
        md-label="No issues found!"
        md-description="Oops! We couldn't find anything.">
      </md-empty-state>
    </div>
    <div v-if="isLoading" class="md-layout md-alignment-center-center">
      <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
    </div>
    <div class="center-children" v-if="!isLoading && !isLast && (!this.$options.propsData.page && !this.$options.propsData.pageSize)">
      <divi-button class="md-accent" @click.native="infiniteHandler">Load more</divi-button>
    </div>
  </div>   
</template>

<script>

// TODO: Add a watcher for the components properties
// https://stackoverflow.com/questions/49041787/child-component-doesnt-get-updated-when-props-changes
// This could be important if we wanted to allow paging from outside an iframe, for example.

import Issue from '@/components/Issue'
import test from '@/components/test'
import DiviButton from '@/components/divi/DiviButton'
import Waterfall from 'vue-waterfall/lib/waterfall'
import WaterfallSlot from 'vue-waterfall/lib/waterfall-slot'

export default {
  name: 'ViewIssues',
  props: {
    filter: String,
    contains: String,
    page: Number,
    pageSize: Number
  },
  data: () => ({
    issues: [],
    errors: [],
    dataPage: null,
    dataPageSize: null,
    issueFunction: null,
    isLast: false,
    isLoading: true,
    showSnackbar: false,
    items: [
      {
        'id': 1,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 2,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 3,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 4,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 5,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 6,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 7,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 300,
        'width': 300},
      {
        'id': 8,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 600},
      {
        'id': 9,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 10,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200},
      {
        'id': 11,
        'description': 'akfjahfkjahsfkjahsfjkah sfa kjhfkjashfjkash fajhsfkjashfj ahskfja hsfkjahsf ahjksfh ajksfhjkasfasfhka jsfhjkahs fkjahsfk hasjkf hajksfh kajsfhjka shfjkash',
        'height': 100,
        'width': 200}
    ]
  }),
  components: {
    Issue,
    DiviButton,
    Waterfall,
    WaterfallSlot,
    test
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
      return this.issueFunction(page, pageSize, contains).then((response) => {
        this.checkIsLast(response)
        this.issues = this.issues.concat(response.data)
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
    this.issues.splice(0, this.issues.length)
    if (this.filter === 'mine') {
      this.issueFunction = this.services.issues.getMyIssues
    } else {
      this.issueFunction = this.services.issues.getMyIssues
    }
    this.issueFunction().then((response) => {
      this.issues = response.data
      // console.log(this.issues)
      for (let i = 0; i < this.issues.length; i++) {
        this.issues[i].isLoading = false
      }
    }, (e) => {
      this.errors.push(e)
    })
    this.isLoading = false
    this.isLast = true
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
