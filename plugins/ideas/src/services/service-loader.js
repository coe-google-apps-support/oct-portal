/// Partially inspired by this:
/// https://codeburst.io/dependency-injection-with-vue-js-f6b44a0dae6d
/// I don't think this is the best way to do this, but it works for now.

// Services
import { IdeasService } from './ideas/IdeasService'
import { UserService } from './user/UserService'
import { IssueService } from './issues/IssueService'
// import { StubbedIdeasService } from './ideas/StubbedIdeasService'
// import { StubbedUserService } from './user/StubbedUserService'
// import { StubbedIssueService } from './issues/StubbedIssueService'

let idea
let user
let issue

if (process.env.NODE_ENV === 'development') {
  idea = IdeasService
  user = UserService
  issue = IssueService
} else if (process.env.NODE_ENV === 'production') {
  idea = IdeasService
  user = UserService
  issue = IssueService
} else if (process.env.NODE_ENV === 'integration') {
  idea = IdeasService
  user = UserService
  issue = IssueService
} else if (process.env.NODE_ENV === 'local') {
  idea = IdeasService
  user = UserService
  issue = IssueService
} else {
  throw new Error(`Unknown environment: ${process.env.NODE_ENV}`)
}

const ServiceLoader = {
  install (Vue, options) {
    Vue.mixin({
      beforeCreate () {
        if (!this.services) {
          this.services = {}
        }
        this.services.ideas = idea
        this.services.user = user
        this.services.issues = issue
      }
    })
  }
}

export default ServiceLoader
