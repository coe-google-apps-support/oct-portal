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
        :auto-hide="true">
        <md-avatar class="tooltip-target">
          <img src="https://media.forgecdn.net/avatars/124/768/636424778749237239.jpeg" alt="Avatar">
        </md-avatar>

        <template slot="popover">
          <input class="tooltip-content" v-model="tooltipContent"/>
          <p>
            {{ tooltipContent }}
          </p>

          <button v-close-popover>Close</button>
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
    offset: 0
  }),
  methods: {
    progressBar () {
      if (this.issue.remedyStatus === 'Submitted') {
        this.amount = 33
      } else if (this.issue.remedyStatus === 'In Review') {
        this.amount = 66
      } else if (this.issue.remedyStatus === 'Completed') {
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
</style>
