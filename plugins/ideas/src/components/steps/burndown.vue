<template>
  <div>
    There is burndown here
    <md-card class="oct-container">
      <md-card-header class="oct-header" :style="{backgroundColor: color}">
        <md-card-header-text class="oct-title-container">
          <div class="md-title oct-title">{{ title }}</div>
        </md-card-header-text>

        <div class="oct-burndown">
          <div class="oct-horz-spacer"></div>
          <div v-for="day in burndown.data" :key="day.date" @click="openURL(day.url)" class="oct-day-holder">
            <div class="oct-bar-container" :style="{backgroundColor: colorDark}">
              <div class="oct-bar-stub" :style="{backgroundColor: colorLight, height: reduceWork(day) + '%'}"></div>
            </div>
            <div class="oct-baby-date">{{ day.date | dayOfWeek }}</div>
            <md-tooltip md-direction="bottom">{{ `${day.workRemoved} issues closed, ${day.workAdded} issues added` }}</md-tooltip>
          </div>
          <div class="oct-horz-spacer"></div>
        </div>
      </md-card-header>

      <div class="card-secondary-info">
        <div class ="description-text">{{ description }}</div>
        <div class="date-text md-subhead">{{ date | formatDate }}</div>
      </div>

      <md-divider></md-divider>

      <div class="oct-actions">
        <md-button @click="openURL(burndown.url)" :style="{color: color}">View</md-button>
      </div>
    </md-card>
  </div>
</template>

<script>
import formatDate from '@/utils/format-date-since'
import colorMod from '@/utils/shade-blend-convert'

export default {
  name: 'BurndownStep',
  props: [
    'color',
    'burndown',
    'title',
    'description',
    'date'
  ],
  computed: {
    colorLight: function () {
      return colorMod(0.2, this.color)
    },
    colorDark: function () {
      return colorMod(-0.2, this.color)
    }
  },
  data: () => ({
    remainingWork: -1,
    maxWork: -1
  }),
  created () {
    console.log('created')
    this.remainingWork = this.burndown.initialWork
    this.calculateMaxWork()
  },
  filters: {
    formatDate,
    dayOfWeek: (dateString) => {
      const codes = [
        'Su',
        'M',
        'T',
        'W',
        'Th',
        'F',
        'Sa'
      ]
      let date = new Date(dateString)
      return codes[date.getDay()]
    }
  },
  methods: {
    reduceWork: function (byStep) {
      this.remainingWork += byStep.workAdded - byStep.workRemoved

      return (this.remainingWork / this.maxWork) * 100
    },
    calculateMaxWork: function () {
      this.maxWork = this.remainingWork
      let currentWork = this.maxWork
      for (let i = 0; i < this.burndown.data.length; i++) {
        currentWork += this.burndown.data[i].workAdded - this.burndown.data[i].workRemoved
        if (currentWork > this.maxWork) {
          this.maxWork = currentWork
        }
      }
    },
    openURL: function (url) {
      window.location.href = url
    }
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";

  .oct-container {
    display: flex;
    flex-direction: column;
  }

  .oct-header {
    height: 200px;
    flex-direction: column;
  }

  .oct-title-container {
    flex-grow: 0;
  }

  // The actual burndown
 
  .oct-burndown {
    display: flex;
    flex-direction: row;
    height: 100%;
  }

  .oct-horz-spacer {
    flex-grow: 1;
  }

  .oct-day-holder {
    display: flex;
    flex-flow: column;
    width: 40px;
    height: 100%;
    cursor: pointer;
  }

  .oct-bar-container {
    flex: 2;
    margin: 0 6px;
    border-radius: 4px;
    overflow: hidden;
    flex-direction: column;
    display: flex; 
    justify-content: flex-end;
    height: 100%;

    .oct-bar-stub {

    }
  }

  .oct-baby-date {
    color: #fefefe;
    text-align: center;
    min-height: 20px;
  }

  .oct-title {
    color: #fefefe;
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
