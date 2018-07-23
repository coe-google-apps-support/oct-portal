<template>
  <md-card :class="statusClass(issue.remedyStatus) + ' md-with-hover color-corner '">
    <md-card-header>
      <span class="md-title"> {{ issue.title }}</span>
    </md-card-header>
    <div>
      <div class="card-secondary-info">
        <div class ="description-text">{{ issue.description }}</div>
        <div>
          Status: <a class="md-body-2">{{ issue.remedyStatus | displayStatus }}</a>
          <md-progress-bar md-mode="determinate" 
            :md-value="issue.remedyStatus | statusAmount"></md-progress-bar>
        </div>
        <div class="info-line">
          <div class="md-subhead">{{ issue.referenceId }}</div>
          <div class="date-text md-subhead">{{ issue.createdDate | formatDate }}</div>
        </div>
      </div>
    </div>
    <md-divider></md-divider>
    <div v-if="issue.assigneeEmail" class="card-secondary-info" style="text-align:center;">
      <div class="md-body-2">Assigned to</div>
      <v-popover
        :placement="placement"
        :offset="offset"
        :auto-hide="true">
        <md-avatar class="tooltip-target">
          <img src="https://i.imgur.com/FD51R30.png" alt="Avatar">
        </md-avatar>

        <template slot="popover">
          <md-avatar class="center1 md-avatar-icon md-large">
            <img src="https://i.imgur.com/FD51R30.png" alt="Avatar">
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
import { displayStatus, statusAmount, statusClass } from '@/data/issue-status.js'

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
    displayStatus,
    statusAmount,
    formatName: function (value) {
      const name = value.substring(0, value.lastIndexOf('@'))
      const fullName = name.split('.')
      return fullName[0].charAt(0).toUpperCase() + fullName[0].slice(1) + ' ' + fullName[1].charAt(0).toUpperCase() + fullName[1].slice(1)
    }
  },
  data: () => ({
    placement: 'top-center',
    offset: 5,
    stepCancel: false,
    stepInitiate: false
  }),
  methods: {
    statusClass
  },
  created () {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import '../mixins.scss';

.color-corner::after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 0;
  height: 0;
  border-width: 20px;
  border-style: solid;
}

.info-line {
  display: flex;
}

.info-line > * {
  flex-grow: 1;
}

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
