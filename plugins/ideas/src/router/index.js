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
      path: '/my-profile',
      name: 'my-profile',
      component: ViewIdeas,
      props: {
        filter: 'mine'
      }
    },
    {
      path: '/initiatives/:slug/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    },
    {
      path: '/plugins/initiatives/new-idea',
      name: 'new-idea',
      component: NewIdea
    },
    {
      path: '/plugins/initiatives/view-ideas',
      name: 'view-ideas',
      component: ViewIdeas
    },
    {
      path: '/plugins/initiatives/my-profile',
      name: 'my-profile',
      component: ViewIdeas,
      props: {
        filter: 'mine'
      }
    },
    {
      path: '/plugins/initiatives/initiatives/:slug/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    }
  ]
})
