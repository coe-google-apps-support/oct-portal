<template>
  <md-card class="card hvr-float" :class="{ 'tada' : isNewIdea }">
    <md-card-media-cover>
      <md-card-media md-ratio="16:9">
        <img class="cardImage" :src="getImage()" alt="Skyscraper">
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
      <divi-button @click.native="openUrl">View</divi-button>
    </md-card-actions>
    <md-progress-bar v-if="initiative.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>
<script>
import formatDate from '@/utils/format-date-since'
import DiviButton from '@/components/divi/DiviButton'

export default {
  name: 'Initiative',
  props: [
    'initiative',
    'isNewIdea'
  ],
  components: {
    DiviButton
  },
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
      window.open(this.initiative.url, '_top')
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
  @include multiLineEllipsis($lineHeight: 1.2em, $lineCount: 3, $bgColor: #fafafa);
}

.description-container {
  height: 3.6em;
}

.cardImage {
  filter: blur(5px);
}
.card:hover .cardImage {
  filter: blur(1.5px);
}

.oct-cover {
  position: absolute;
  width: 100%;
  height: 100%;
  opacity: 0.3;
  top: 50%;
  right: 0;
  left: 0;
  transform: translateY(-50%);
  z-index: 1;
  background-color: #444444;
}

.md-card {
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  background-color: #fafafa;
}

@-webkit-keyframes tada {
  from {
    -webkit-transform: scale3d(1, 1, 1);
    transform: scale3d(1, 1, 1);
  }

  10%,
  20% {
    -webkit-transform: scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg);
    transform: scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg);
  }

  30%,
  50%,
  70%,
  90% {
    -webkit-transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg);
    transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg);
  }

  40%,
  60%,
  80% {
    -webkit-transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg);
    transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg);
  }

  to {
    -webkit-transform: scale3d(1, 1, 1);
    transform: scale3d(1, 1, 1);
  }
}

@keyframes tada {
  from {
    -webkit-transform: scale3d(1, 1, 1);
    transform: scale3d(1, 1, 1);
  }

  10%,
  20% {
    -webkit-transform: scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg);
    transform: scale3d(0.9, 0.9, 0.9) rotate3d(0, 0, 1, -3deg);
  }

  30%,
  50%,
  70%,
  90% {
    -webkit-transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg);
    transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, 3deg);
  }

  40%,
  60%,
  80% {
    -webkit-transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg);
    transform: scale3d(1.1, 1.1, 1.1) rotate3d(0, 0, 1, -3deg);
  }

  to {
    -webkit-transform: scale3d(1, 1, 1);
    transform: scale3d(1, 1, 1);
  }
}

.tada {
  -webkit-animation-name: tada;
  animation-name: tada;
  -webkit-animation-duration: 1.8s;
  animation-duration: 1.8s;
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

.hvr-float {
  display: inline-block;
  vertical-align: middle;
  -webkit-transform: perspective(1px) translateZ(0);
  transform: perspective(1px) translateZ(0);
  box-shadow: 0 0 1px rgba(0, 0, 0, 0);
  -webkit-transition-duration: 0.3s;
  transition-duration: 0.3s;
  -webkit-transition-property: transform;
  transition-property: transform;
  -webkit-transition-timing-function: ease-out;
  transition-timing-function: ease-out;
}
.hvr-float:hover, .hvr-float:focus, .hvr-float:active {
  -webkit-transform: translateY(-8px);
  transform: translateY(-8px);
}

</style>
