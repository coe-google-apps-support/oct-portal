// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './AppIdeas'
import router from './router'
import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'
// import 'vue-material/dist/theme/black-green-light.css'

Vue.config.productionTip = false
Vue.use(VueMaterial)

/* eslint-disable no-new */
new Vue({
  el: '#app-ideas',
  router,
  template: '<App/>',
  components: { App }
})
