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
      path: '*',
      component: ViewIdeas
    },
    {
      path: '*/new-idea',
      name: 'new-idea',
      component: NewIdea
    },
    {
      path: '*/view-ideas',
      name: 'view-ideas',
      component: ViewIdeas
    },
    {
      path: '*/my-profile',
      name: 'my-profile',
      component: ViewIdeas,
      props: {
        filter: 'mine'
      }
    },
    {
      path: '*/initiatives/:initiativeId/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    }
  ]
})
