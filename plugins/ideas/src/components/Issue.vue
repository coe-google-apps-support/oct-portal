<template>
  <md-card class="card md-with-hover">
    <md-card-header>
      <md-toolbar md-elevation="1">
        <span class="md-title"> {{ issue.title }}</span>
      </md-toolbar>
    </md-card-header>
    <div>
      <div class="card-secondary-info">
        <div class="description-container">
          <div class ="description-text">{{ issue.description }}</div>
        </div>
        <div>
          Status: {{ issue.remedyStatus }}
          <md-progress-bar md-mode="determinate" :md-value="amount"></md-progress-bar>
        </div>
        <div class="date-text md-subhead">{{ issue.date | formatDate }}</div>
      </div>
    </div>
    <md-divider></md-divider>
    <div class="card-secondary-info">
      Assigned to:
      <v-popover
        :placement="placement"
        :offset="offset"
        :auto-hide="false">
        <md-avatar class="tooltip-inner popover-inner">
          <img src="https://media.forgecdn.net/avatars/124/768/636424778749237239.jpeg" alt="Avatar">
          <md-tooltip md-direction="right">Someone's name</md-tooltip>
        </md-avatar>

    <template slot="popover">
      <input v-model="tooltipContent" placeholder="Tooltip content" />
      <p>
        {{ tooltipContent }}
      </p>

      <a v-close-popover>Close</a>
    </template>
  </v-popover>
    </div>
    <md-progress-bar v-if="issue.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
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
  data: () => ({
    amount: 0,
    tooltipContent: 'test123',
    placement: 'top-center',
    offset: 10
  }),
  methods: {
    progressBar () {
      if (this.issue.status === 'Submitted') {
        this.amount = 33
      } else if (this.issue.status === 'In Review') {
        this.amount = 66
      } else if (this.issue.status === 'Completed') {
        this.amount = 100
      }
    }
  },
  created () {
    this.progressBar()
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

.tooltip {
  display: block !important;
  z-index: 199;
  .tooltip-inner {
    background: black;
    color: white;
    border-radius: 16px;
    padding: 5px 10px 4px;
  }
  .tooltip-arrow {
    width: 0;
    height: 0;
    border-style: solid;
    position: absolute;
    margin: 5px;
    border-color: black;
    z-index: 1;
  }
  [x-placement^="top"] {
    margin-bottom: 5px;
    .tooltip-arrow {
      border-width: 5px 5px 0 5px;
      border-left-color: transparent !important;
      border-right-color: transparent !important;
      border-bottom-color: transparent !important;
      bottom: -5px;
      left: calc(50% - 5px);
      margin-top: 0;
      margin-bottom: 0;
    }
  }
  [x-placement^="bottom"] {
    margin-top: 5px;
    .tooltip-arrow {
      border-width: 0 5px 5px 5px;
      border-left-color: transparent !important;
      border-right-color: transparent !important;
      border-top-color: transparent !important;
      top: -5px;
      left: calc(50% - 5px);
      margin-top: 0;
      margin-bottom: 0;
    }
  }
  [x-placement^="right"] {
    margin-left: 5px;
    .tooltip-arrow {
      border-width: 5px 5px 5px 0;
      border-left-color: transparent !important;
      border-top-color: transparent !important;
      border-bottom-color: transparent !important;
      left: -5px;
      top: calc(50% - 5px);
      margin-left: 0;
      margin-right: 0;
    }
  }
  [x-placement^="left"] {
    margin-right: 5px;
    .tooltip-arrow {
      border-width: 5px 0 5px 5px;
      border-top-color: transparent !important;
      border-right-color: transparent !important;
      border-bottom-color: transparent !important;
      right: -5px;
      top: calc(50% - 5px);
      margin-left: 0;
      margin-right: 0;
    }
  }
  [aria-hidden='true'] {
    visibility: hidden;
    opacity: 0;
    transition: opacity .15s, visibility .15s;
  }
  [aria-hidden='false'] {
    visibility: visible;
    opacity: 1;
    transition: opacity .15s;
  }
  .info {
    $color: rgba(#004499, .9);
    .tooltip-inner {
      background: $color;
      color: white;
      padding: 24px;
      border-radius: 5px;
      box-shadow: 0 5px 30px rgba(black, .1);
    }
    .tooltip-arrow {
      border-color: $color;
    }
  }
  .popover {
    $color: #f9f9f9;
    .popover-inner {
      background: $color;
      color: black;
      padding: 24px;
      border-radius: 5px;
      box-shadow: 0 5px 30px rgba(black, .1);
    }
    .popover-arrow {
      border-color: $color;
    }
  }
}
</style>
