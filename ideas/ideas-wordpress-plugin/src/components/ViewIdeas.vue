<template>
  <div>
    <div v-if="ideas && ideas.length">
      <md-card v-for="idea in ideas" :key="idea.id" class="md-primary" :style="{backgroundColor: getColor(idea)}">
        <md-card-header>
          <md-card-header-text>
            <div class="md-title">{{ idea.title }}</div>
            <div class="md-subhead">{{ formatDate(idea) }} by {{ idea.stakeholders[0].userName }}</div>
          </md-card-header-text>

          <md-card-media>
            <img v-bind:src="getImage(idea)" alt="Avatar">
          </md-card-media>
        </md-card-header>

        <md-card-actions>
          <md-button>+1</md-button>
          <md-button v-bind:href="idea.url">View</md-button>
        </md-card-actions>
      </md-card>
    </div>
  </div>    
</template>

<script>
/* eslint-disable */
import { IdeasService } from '../services/IdeasService.js'

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
    IdeasService.getIdeas().then((response) => {
      console.log('received ideas!!')
      this.ideas = response.data
    }, (e) => {
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
    },
    getImage (idea) {
      const images = [
        '/static/assets/temp/balance-200-white.png',
        '/static/assets/temp/card-travel-200-white.png',
        '/static/assets/temp/explore-200-white.png',
        '/static/assets/temp/help-200-white.png',
        '/static/assets/temp/house-200-white.png'
      ]

      const randIndex = Math.floor(Math.random() * images.length)
      return images[randIndex]
    },
    getColor (idea) {
      const colors = [
        '#3F51B5',
        '#009688',
        '#4CAF50',
        '#607D8B',
        '#ef5350',
        '#00C853',
        '#FF5722',
        '#E91E63',
      ]

      const randIndex = (Math.floor(Math.random() * colors.length) + idea.id) % colors.length
      return colors[randIndex]
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

.md-card {
  width: 320px;
  margin: 4px;
  display: inline-block;
  vertical-align: top;
}

</style>
