import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'NewIdea',
      component: NewIdea
    }
  ]
})
