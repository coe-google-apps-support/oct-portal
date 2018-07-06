<template>
  <md-card class="card hvr-float md-with-hover">
    <md-card-header>
        <md-card-header-text class="title-container">
        <div class="filler"></div>
        <div class="md-title title">{{ issue.title }}</div>            
        </md-card-header-text>
    </md-card-header>
    <div class="card-secondary-info">
      <div class="description-container">
        <div class ="description-text">{{ issue.description }}</div>
      </div>
      <div class="date-text md-subhead">{{ issue.date | formatDate }}</div>
    </div>
    <md-divider></md-divider>
    <div>
      Assigned to:
      <md-avatar>
        <img src="https://media.forgecdn.net/avatars/124/768/636424778749237239.jpeg" alt="Avatar">
        <md-tooltip md-direction="right">Someone's name</md-tooltip>
      </md-avatar>
    </div>
    <br>
    <div class="description-text">Status: {{ issue.status }}</div>
    <!-- <md-progress-bar v-if="issue.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar> -->
  </md-card>
</template>
<script>
import formatDate from '@/utils/format-date-since'
import DiviButton from '@/components/divi/DiviButton'

export default {
  name: 'Issue',
  props: [
    'issue'
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
    testMethod () {
      console.log('YAY')
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
  color: #252525;
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
