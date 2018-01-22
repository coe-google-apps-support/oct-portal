<template>
  <div>
    <div v-if="ideas && ideas.length">
      <md-card v-for="idea in ideas" :key="idea.id" >
        <md-card-header :style="{backgroundColor: getColor(idea)}">
          <md-card-header-text class="title-container">
            <div class="filler"></div>
            <div class="md-title title">{{ idea.title }}</div>            
          </md-card-header-text>

          <div class="big-media">
            <img v-bind:src="getImage(idea)" alt="Avatar">
          </div>
        </md-card-header>

        <div class="card-secondary-info">
          <div class ="description-text">{{ idea.description | truncate }}</div>
          <div class="date-text md-subhead">{{ formatDate(idea) }}</div>
        </div>
        
        <md-divider></md-divider>

        <md-card-actions>
          <md-button @click="openCard(idea.url)" :style="{color: getColor(idea)}">View</md-button>
        </md-card-actions>
      </md-card>
    </div>
  </div>    
</template>

<script>
/* eslint-disable */

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
    this.services.ideas.getIdeas().then((response) => {
      console.log('received ideas!!')
      this.ideas = response.data
    }, (e) => {
      this.errors.push(e)
    })
  },
  filters: {
    truncate(str) {
      const MAX_LENGTH = 300
      if (str.length < MAX_LENGTH) {
        return str;
      }
      else {
        return str.slice(0, MAX_LENGTH) + '...'
      }      
    }
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
      const root = process.env.STATIC_ASSETS
      const images = [
        `${root}/assets/temp/balance-200-white.png`,
        `${root}/assets/temp/card-travel-200-white.png`,
        `${root}/assets/temp/explore-200-white.png`,
        `${root}/assets/temp/help-200-white.png`,
        `${root}/assets/temp/house-200-white.png`
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % images.length
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

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % colors.length
      return colors[randIndex]
    },
    openCard (url) {
      this.$router.push(url)
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

.md-card {
  width: 360px;
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  background-color: #fafafa;
}

.title {
  color: #fefefe;
  text-overflow: ellipsis;
  white-space: nowrap;
  width: 180px;
  overflow: hidden;
}

.title-container {
  flex-direction: column;
  display: flex;
}

.filler {
  flex-grow: 1;
}

.big-media {
  position: relative;
  width: 120px;
  height: 120px;
  margin-left: 16px;
  -webkit-box-flex: 0;
  flex: 0 0 inherit;
}

.card-secondary-info {
  margin: 14px;
}

.card-secondary-info > * {
  margin-bottom: 8px;
  margin-top: 8px;
}

.date-text {
  text-align: right;
}

</style>
