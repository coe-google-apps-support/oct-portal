// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import AppIdeas from './AppIdeas'
import router from './router'
import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'
import { HTTP } from './HttpCommon'
import ServiceLoader from './services/service-loader.js'

Vue.config.productionTip = false
Vue.use(VueMaterial)
Vue.use(ServiceLoader)

export const bus = new Vue()

/* eslint-disable no-new */
var appIdeas = new Vue({
  el: '#coe-idea-new',
  router,
  template: '<AppIdeas/>',
  components: { AppIdeas },
  methods: {
    initialize (config) {
      if (config.userInfo) {
        HTTP.setUserInfo(config.userInfo)
      }
      if (config.ideaApi) {
        HTTP.setIdeaApi(config.ideaApi)
      }
      bus.$emit('initialize-ideas', config)
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

ideas.app = appIdeas
