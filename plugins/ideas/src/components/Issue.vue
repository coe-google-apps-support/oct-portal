<template>
  <md-card class="md-with-hover">
    <md-card-header>
      <md-toolbar md-elevation="1">
        <span class="md-title"> {{ issue.title }}</span>
      </md-toolbar>
    </md-card-header>
    <div>
      <div class="card-secondary-info">
        <div class ="description-text">{{ issue.description }}</div>
      <div>
        Status: <a class="md-body-2">{{ issue.remedyStatus }}</a>
        <md-progress-bar :class="{ 'submit-off':step1, 'review-off':step3, 'collaborate-off':step2, 'deliver-off':step4 }" md-mode="determinate" :md-value="amount"></md-progress-bar>
      </div>
        <div class="date-text md-subhead">{{ issue.date | formatDate }}</div>
      </div>
    </div>
    <md-divider></md-divider>
    <div class="card-secondary-info" style="text-align:center;">
      <div class="md-body-2">Assigned to</div>
      <v-popover
        :placement="placement"
        :offset="offset"
        :auto-hide="true">
        <md-avatar class="tooltip-target">
          <img src="https://media.forgecdn.net/avatars/124/768/636424778749237239.jpeg" alt="Avatar">
        </md-avatar>

        <template slot="popover">
          <md-avatar class="center1 md-avatar-icon md-large">
            <img src="https://media.forgecdn.net/avatars/124/768/636424778749237239.jpeg" alt="Avatar">
          </md-avatar>
          <br>
          <div style="text-align:center;">
            <div class="card-secondary-info md-title">{{ issue.assigneeEmail | formatName }}</div>
            <div class="card-secondary-info md-subheading">{{ issue.assigneeEmail }}</div>
            <md-button class="md-accent md-raised" v-close-popover>Close</md-button>
          </div>
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
    formatDate,
    formatName: function (value) {
      const name = value.substring(0, value.lastIndexOf('@'))
      const fullName = name.split('.')
      return fullName[0].charAt(0).toUpperCase() + fullName[0].slice(1) + ' ' + fullName[1].charAt(0).toUpperCase() + fullName[1].slice(1)
    }
  },
  data: () => ({
    amount: 0,
    tooltipContent: 'test123',
    placement: 'top-center',
    offset: 5,
    stepCancel: false,
    stepInitiate: false,
    step1: false,
    step2: false,
    step3: false,
    step4: false
    //    Cancelled = 1,
    //    Initiate = 2,
    //    Submit = 3,
    //    Review = 4,
    //    Collaborate = 5,
    //    Deliver = 6,
  }),
  methods: {
    progressBar () {
      if (this.issue.remedyStatus === 'Cancelled') {
        this.amount = 100
        this.stepCancel = true
      } else if (this.issue.remedyStatus === 'Initiate') {
        this.amount = 20
        this.Initiate = true
      } else if (this.issue.remedyStatus === 'Submit') {
        this.amount = 25
        this.step1 = true
      } else if (this.issue.remedyStatus === 'Review') {
        this.amount = 50
        this.step2 = true
      } else if (this.issue.remedyStatus === 'Collaborate') {
        this.amount = 75
        this.step3 = true
      } else if (this.issue.remedyStatus === 'Deliver') {
        this.amount = 100
        this.step4 = true
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
  // @include multiLineEllipsis($lineHeight: 1.2em, $lineCount: 3, $bgColor: #fafafa);
  word-wrap: break-word;
}

.description-container {
  height: 3.6em;
}

.center1 {
  position: relative;
  left: 35%;
}

.review-off {
  background-color: var(--status-review-off) !important;
}

.submit-off {
  background-color: var(--status-submit-off) !important;
}

.deliver-off {
  background-color: var(--status-deliver-off) !important;
}

.collaborate-off {
  background-color: var(--status-collaborate-off) !important;
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
