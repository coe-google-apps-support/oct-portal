// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import AppIdeas from './AppIdeas'
import router from './router'
import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'
import ServiceLoader from './services/service-loader.js'

Vue.config.productionTip = false
Vue.use(VueMaterial)
Vue.use(ServiceLoader)

/* eslint-disable no-new */
new Vue({
  el: '#coe-idea-new',
  router,
  template: '<AppIdeas/>',
  components: { AppIdeas }
})
