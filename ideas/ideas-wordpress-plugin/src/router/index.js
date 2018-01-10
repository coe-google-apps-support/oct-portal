import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'
import ViewIdeas from '@/components/ViewIdeas'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'NewIdea',
      component: NewIdea
    },
    {
      path: '/ViewIdeas',
      name: 'ViewIdeas',
      component: ViewIdeas
    }
  ]
})
