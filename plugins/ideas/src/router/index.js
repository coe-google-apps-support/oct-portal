import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'
import ViewIdeas from '@/components/ViewIdeas'
import ViewInitiative from '@/components/ViewInitiative'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/new-idea',
      name: 'new-idea',
      component: NewIdea
    },
    {
      path: '/view-ideas',
      name: 'view-ideas',
      component: ViewIdeas,
      props: (route) => ({
        page: Number(route.query.page),
        pageSize: Number(route.query.pageSize)
      })
    },
    {
      path: '/my-profile',
      name: 'my-profile',
      component: ViewIdeas,
      props: (route) => ({
        filter: 'mine',
        newInitiative: Number(route.query.newInitiative)
      })
        // filter: 'mine',
        // newInitiative: route.newInitiative
    },
    {
      path: '/initiatives/:slug/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    }
  ]
})
