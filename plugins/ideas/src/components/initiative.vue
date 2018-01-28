<template>
  <md-card>
    <md-card-media-cover>
      <md-card-media md-ratio="16:9">
        <img :src="getImage()" alt="Skyscraper">
        <div class="oct-cover" :style="{ backgroundColor: getColor(initiative) }"></div>
      </md-card-media>

      <md-card-area>
        <md-card-header>
          <md-card-header-text class="title-container">
            <div class="filler"></div>
            <div class="md-title title">{{ initiative.title }}</div>            
          </md-card-header-text>
        </md-card-header>
      </md-card-area>
    </md-card-media-cover>

    <div class="card-secondary-info">
      <div class ="description-text">{{ initiative.description | truncate }}</div>
      <div class="date-text md-subhead">{{ initiative.createdDate | formatDate }}</div>
    </div>
    
    <md-divider></md-divider>

    <md-card-actions>
      <md-button @click="$emit('onView', initiative)" :style="{ color: getColor(initiative) }">View</md-button>
    </md-card-actions>
    <md-progress-bar v-if="initiative.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>
<script>
import formatDate from '@/utils/format-date-since'

export default {
  name: 'Initiative',
  props: [
    'initiative'
  ],
  filters: {
    truncate (str) {
      const MAX_LENGTH = 300
      if (str.length < MAX_LENGTH) {
        return str
      } else {
        return str.slice(0, MAX_LENGTH) + '...'
      }
    },
    formatDate
  },
  methods: {
    getImage () {
      const root = process.env.STATIC_ASSETS
      const images = [
        `${root}/assets/cards/card1.png`,
        `${root}/assets/cards/card2.png`,
        `${root}/assets/cards/card3.png`,
        `${root}/assets/cards/card4.png`,
        `${root}/assets/cards/card5.png`,
        `${root}/assets/cards/card6.png`,
        `${root}/assets/cards/card7.png`,
        `${root}/assets/cards/card8.png`,
        `${root}/assets/cards/card9.png`,
        `${root}/assets/cards/card10.png`,
        `${root}/assets/cards/card11.png`,
        `${root}/assets/cards/card12.png`,
        `${root}/assets/cards/card13.png`,
        `${root}/assets/cards/card14.png`
      ]

      const randIndex = (this.initiative.title.charCodeAt(0) + this.initiative.title.charCodeAt(1) + this.initiative.id) % images.length
      return images[randIndex]
    },
    getColor (idea) {
      // const colors = [
      //   '#e57373',
      //   '#F06292',
      //   '#BA68C8',
      //   '#9575CD',
      //   '#7986CB',
      //   '#64B5F6',
      //   '#4FC3F7',
      //   '#4DD0E1',
      //   '#4DB6AC',
      //   '#81C784',
      //   '#AED581',
      //   '#DCE775',
      //   '#FFF176',
      //   '#FFD54F',
      //   '#FFB74D',
      //   '#FF8A65'
      // ]
      const colors = [
        '#4DB6AC'
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % colors.length
      return colors[randIndex]
    },
    openCard (idea) {
      this.$router.push(`/ViewIdeas/${idea.id}`)
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

.oct-cover {
  position: absolute;
  width: 100%;
  height: 100%;
  opacity: 0.8;
  top: 50%;
  right: 0;
  left: 0;
  transform: translateY(-50%);
  z-index: 1;
}

.md-card {
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  background-color: #fafafa;
}

.title {
  color: #fefefe;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.title-container {
  flex-direction: column;
  display: flex;
}

.filler {
  flex-grow: 1;
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
