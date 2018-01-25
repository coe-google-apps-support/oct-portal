import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'
import ViewIdeas from '@/components/ViewIdeas'
import ViewInitiative from '@/components/ViewInitiative'

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
      component: ViewIdeas,
      children: [
        {
          path: ':id',
          name: 'ViewInitiative',
          component: ViewInitiative,
          props: true
        }
      ]
    }
  ]
})
