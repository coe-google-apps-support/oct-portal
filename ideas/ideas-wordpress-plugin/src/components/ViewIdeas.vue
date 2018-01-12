<template>
  <div>
    <div v-if="ideas && ideas.length">

      <div v-for="idea in ideas" :key="idea.id" class="summary">
        <h3><a v-bind:href="idea.url">{{ idea.title }}</a></h3>
        <div class="ideaDetails">
          {{ formatIdeaDescription(idea) }}
        </div>
        <div class="started">
          {{ formatDate(idea) }} by {{ idea.stakeholders[0].userName }}
        </div>
        <hr class="separator" />
      </div>
    </div>
  </div>    
</template>

<script>
import { HTTP } from '../HttpCommon'

const emptyDate = new Date('0001-01-01T00:00:00.000Z')

export default {
  name: 'ViewIdeas',
  data: () => ({
    ideas: [],
    errors: []
  }),
  // Fetches ideas when the component is created.
  created () {
    // clear out the ideas
    this.ideas.splice(0, this.ideas.length)
    HTTP.get('').then(response => {
      // JSON responses are automatically parsed.
      this.ideas = response.data
    })
    .catch(e => {
      this.errors.push(e)
    })
  },
  methods: {
    formatIdeaDescription (idea) {
      var desc = idea.description
      if (desc && desc.length > 140) {
        return desc.substring(0, 140) + '...'
      } else {
        return desc
      }
    },
    formatDate (idea) {
      var now = new Date()
      var d = new Date(idea.createdDate)
      if (d.getTime() === emptyDate.getTime()) {
        return ''
      } else if (d > now) {
        // idea is in the future!
        console.log('idea is was created in the future!')
        console.log(idea)
        return ''
      } else {
        var timeDiff = Math.abs(d.getTime() - now.getTime())
        var seconds = timeDiff / 1000
        if (seconds < 60) {
          return '' + Math.floor(seconds) + ' secs ago'
        } else {
          var minutes = seconds / 60
          if (minutes === 1) {
            return '1 min ago'
          } else if (minutes < 60) {
            return '' + Math.floor(minutes) + ' mins ago'
          } else {
            var hours = minutes / 24
            if (hours === 1) {
              return '1 hour ago'
            } else if (hours < 24) {
              return '' + Math.floor(hours) + ' hours ago'
            } else {
              var days = hours / 24
              if (days === 1) {
                return '1 day ago'
              } else if (days < 365) {
                return '' + Math.floor(days) + ' days ago'
              } else {
                var years = days / 365 // close enough
                if (years === 1) {
                  return '1 year ago'
                } else {
                  return '' + Math.floor(years) + ' years ago'
                }
              }
            }
          }
        }
      }
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.started { 
  float: right;
  
}
.separator { 
  margin-top: 30px;
  clear:both;
}
</style>
