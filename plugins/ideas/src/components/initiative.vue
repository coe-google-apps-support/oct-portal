<template>
  <md-card :class="{ 'first-card' : isNewIdea }">
    <md-card-media-cover>
      <md-card-media md-ratio="16:9">
        <img :src="getImage()" alt="Skyscraper">
        <div class="oct-cover"></div>
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
      <div class="description-container">
        <div class ="description-text">{{ initiative.description }}</div>
      </div>
      <div class="date-text md-subhead">{{ initiative.createdDate | formatDate }}</div>
    </div>
    
    <md-divider></md-divider>
  
    <md-card-actions>
      <md-button v-on:click="openUrl">View</md-button>
    </md-card-actions>
    <md-progress-bar v-if="initiative.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>
<script>
import formatDate from '@/utils/format-date-since'

export default {
  name: 'Initiative',
  props: [
    'initiative',
    'isNewIdea'
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
    openUrl () {
      window.open(this.initiative.url, '_blank')
    },
    getImage () {
      const images = [
        `https://octava.blob.core.windows.net/cdn-store/cards/card1.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card2.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card3.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card4.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card5.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card6.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card7.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card8.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card9.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card10.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card11.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card12.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card13.png`,
        `https://octava.blob.core.windows.net/cdn-store/cards/card14.png`
      ]
      const randIndex = (this.initiative.title.charCodeAt(0) + this.initiative.title.charCodeAt(1) + this.initiative.id) % images.length
      return images[randIndex]
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import '../mixins.scss';

.description-text {
  position: relative;
  @include multiLineEllipsis($lineHeight: 1.2em, $lineCount: 3, $bgColor: white);
}

.description-container {
  height: 3.6em;
}

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
  background-color: var(--primary-color);
}

.md-card {
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  background-color: #fafafa;
}
.first-card {
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  animation: pulseanimation 1s linear 3;     // inifinite for testing purposes. Use 3 iterations otherwise.
}
@keyframes pulseanimation {
  0% {
    // TODO figure out how to use global values
    box-shadow: 0 0 0 0 rgba(102, 182, 119, 1);
  }
  40% {
    box-shadow: 0 0 0 20px rgba(102, 182, 119, 0);
  }
  80% {
    box-shadow: 0 0 0 20px rgba(102, 182, 119, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(102, 182, 119, 0);
  }
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
