// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import AppNewIdea from './AppNewIdea'
import router from './router'
import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'
import 'vue-material/dist/theme/black-green-light.css'
import { HTTP } from './HttpCommon'

Vue.config.productionTip = false
Vue.use(VueMaterial)

/* eslint-disable no-new */
var app1 = new Vue({
  el: '#coe-idea-new',
  router,
  template: '<AppNewIdea/>',
  components: { AppNewIdea },
  methods: {
    setUserInfo: userInfo => {
      var that = this
      console.log(that)
      // debugger
      // console.log(HTTP)
      HTTP.setUserInfo(userInfo)
    }
  }
})

var coe = window.coe
if (!coe) {
  coe = {}
  window.coe = coe
}

var ideas = coe.ideas
if (!ideas) {
  ideas = {}
  coe.ideas = ideas
}

ideas.appNew = app1
