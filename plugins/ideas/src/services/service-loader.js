/// Partially inspired by this:
/// https://codeburst.io/dependency-injection-with-vue-js-f6b44a0dae6d
/// I don't think this is the best way to do this, but it works for now.

// Services
import { IdeasService } from './ideas/IdeasService'
import { StubbedIdeasService } from './ideas/StubbedIdeasService'
let idea

if (process.env.NODE_ENV === 'development') {
  idea = StubbedIdeasService
} else if (process.env.NODE_ENV === 'production') {
  idea = IdeasService
} else {
  throw new Error(`Unknown environment: ${process.NODE_ENV}`)
}

const ServiceLoader = {
  install (Vue, options) {
    Vue.mixin({
      beforeCreate () {
        if (!this.services) {
          this.services = {}
        }
        this.services.ideas = idea
      }
    })
  }
}

export default ServiceLoader
