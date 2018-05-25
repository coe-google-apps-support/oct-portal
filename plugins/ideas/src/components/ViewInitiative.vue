<template>
  <div>
    <div v-if="isLoading" class="oct-loader">
      <md-progress-spinner md-mode="indeterminate"></md-progress-spinner>
    </div>
    <div v-else class="md-layout md-alignment-top-center oct-min-size">
      <div v-if="initiative != null" class="md-layout-item md-size-60 md-small-size-90">
        <div class="oct-content-block">
          <span class="md-display-2">{{ initiative.title }}</span>
          <md-divider class="oct-divider"></md-divider>
          <div class="md-caption">{{ initiative.createdDate | formatDate }}</div>
        </div>
        <div class="md-body-1 oct-content-block">{{ initiative.description }}</div>
        <!-- <md-button class="md-raised md-primary"> Clear Form </md-button> -->
        <div v-if="resources">
          <div class="md-headline">Resources</div>
          <md-table>
            <md-table-row>
              <md-table-cell>Assigned to</md-table-cell>
              <md-table-cell>
                <Assignee v-if="resources.assignee" :user="resources.assignee"></Assignee>
                <div v-else>Unassigned</div>
              </md-table-cell>
            </md-table-row>
          </md-table>
        </div>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps" :isEditable="canEditSteps" v-on:description-updated="updateDescription"></Steps>
      </div>
    </div>
    <br><br><br><br><br><br><br><br><br> 
  </div>
</template>

<script>
import Assignee from '@/components/Assignee'
import Steps from '@/components/stepper/Steps'
import formatDate from '@/utils/format-date-since'
import SupportingDocs from '@/components/SupportingDocs'

export default {
  name: 'ViewInitiative',
  props: [
    'slug'
  ],
  data: () => ({
    errors: [],
    initiative: null,
    assignee: null,
    steps: null,
    isLoading: true,
    resources: null,
    activeUser: null,
    canEditSteps: false,
    supportingDocs: null,
    showModal: false
  }),
  components: {
    Assignee,
    Steps,
    SupportingDocs
  },
  created () {
    console.log(this.slug)
    this.services.user.getMe().then((user) => {
      this.activeUser = user
      this.canEditSteps = user.permissions.indexOf('editStatusDescription') !== -1
    }, (err) => {
      this.errors.push(err)
      console.err('ViewInitiative: failed getMe().')
    })

    this.services.ideas.getInitiative(this.slug).then((initiative) => {
      this.initiative = initiative
      return Promise.all([this.services.ideas.getResources(initiative.id), this.services.ideas.getInitiativeSteps(initiative.id)])
    }).then((response) => {
      if (response && response[0]) {
        this.resources = response[0]
      }
      if (response && response[1]) {
        this.steps = response[1]
      }

      this.isLoading = false
    })
  },
  filters: {
    formatDate
  },
  methods: {
    updateDescription (stepIndex) {
      console.log('update description')
      let newDescription = this.steps[stepIndex].description
      let stepId = this.steps[stepIndex].stepId
      console.log(`Okay for real now, setting to "${newDescription}"`)
      this.services.ideas.updateStatusDescription(this.initiative.id, stepId, newDescription).then(() => {
        console.log('Status description update successful.')
      }, (err) => {
        console.error('Failed updating status description.')
        this.errors.push(err)
      })
    }
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";
  @import "../colors.scss";

  .oct-loader {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }
  
  .oct-steps {
    margin: 14px;
  }

  .oct-divider {
    background-color: $oct-primary;
  }

  .oct-content-block {
    margin: 14px 0px;
  }

  .oct-min-size {
    min-width: 400px;
  }
  
  .sd-headline {
    font-size: 25px;
  }

  tbody .md-table-row td {
    border-top: 0px;
  }

  .sd-add-button {
    position: relative;
    margin: auto;
  }
</style>