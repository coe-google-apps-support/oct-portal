import Vue from 'vue'
import Router from 'vue-router'
import NewIdea from '@/components/NewIdea'
import ViewIdeas from '@/components/ViewIdeas'
import ViewInitiative from '@/components/ViewInitiative'
import ViewIssues from '@/components/ViewIssues'

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
        pageSize: Number(route.query.pageSize),
        newInitiative: Number(route.query.newInitiative),
        contains: route.query.contains
      })
    },
    {
      path: '/my-profile',
      name: 'my-profile',
      component: ViewIdeas,
      props: (route) => ({
        filter: 'mine',
        newInitiative: Number(route.query.newInitiative),
        page: Number(route.query.page),
        pageSize: Number(route.query.pageSize),
        contains: route.query.contains
      })
    },
    {
      path: '/initiatives/:slug/',
      name: 'initiatives',
      component: ViewInitiative,
      props: true
    },
    {
      path: '/view-issues',
      name: 'view-issues',
      component: ViewIssues,
      props: (route) => ({
        page: Number(route.query.page),
        pageSize: Number(route.query.pageSize),
        contains: route.query.contains
      })
    }
  ]
})
