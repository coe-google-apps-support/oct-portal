import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'
import ViewIdeas from '@/components/ViewIdeas'
import ViewInitiative from '@/components/ViewInitiative'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/new-idea',
      name: 'new-idea',
      component: NewIdea
    },
    {
      path: '/view-ideas',
      name: 'view-ideas',
      component: ViewIdeas
    },
    {
      path: '/initiatives/:slug/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    }
  ]
})
