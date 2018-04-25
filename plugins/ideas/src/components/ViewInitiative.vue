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
            <md-table-row>
              <md-table-cell>Business case</md-table-cell>
              <md-table-cell>
                <attach-file-button :url="resources.businessCaseUrl" @click.native="attachBusinessCaseClicked"></attach-file-button>
              </md-table-cell>
            </md-table-row>
            <md-table-row>
              <md-table-cell>Investment Request Form</md-table-cell>
              <md-table-cell>
                <attach-file-button :url="resources.investmentRequestFormUrl" @click.native="attachInvestmentFormClicked"></attach-file-button>
              </md-table-cell>
            </md-table-row>            
          </md-table>
        </div>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps" :isEditable="canEditSteps" v-on:description-updated="updateDescription"></Steps>
      </div>
    </div>    

    <md-dialog v-if="initiative && resources" :md-active.sync="showBusinessCaseDialog" class="oct-business-case">
      <md-dialog-title>Business Case</md-dialog-title>
      <md-field>
        <label>URL</label>
        <md-input v-model="temp_businessCaseUrl"></md-input>
      </md-field>
      <md-button @click="attachBusinessCase" class="md-primary oct-attach-button">Attach</md-button>
      <md-progress-bar v-if="busCaseLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
    </md-dialog>

    <md-dialog v-if="initiative && resources" :md-active.sync="showInvestmentFormDialog" class="oct-investment-form">
      <md-dialog-title>Investment Form</md-dialog-title>
      <md-field>
        <label>URL</label>
        <md-input v-model="temp_investmentFormUrl"></md-input>
      </md-field>
      <md-button @click="attachInvestmentForm" class="md-primary oct-attach-button">Attach</md-button>
      <md-progress-bar v-if="invFormLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
    </md-dialog>    
  </div>
</template>

<script>
import Assignee from '@/components/Assignee'
import Steps from '@/components/stepper/Steps'
import AttachFileButton from '@/components/AttachFileButton'
import formatDate from '@/utils/format-date-since'

export default {
  name: 'ViewInitiative',
  props: [
    'slug'
  ],
  data: () => ({
    temp_businessCaseUrl: null,
    temp_investmentFormUrl: null,
    errors: [],
    initiative: null,
    assignee: null,
    steps: null,
    showBusinessCaseDialog: false,
    showInvestmentFormDialog: false,
    busCaseLoading: false,
    invFormLoading: false,
    isLoading: true,
    resources: null,
    activeUser: null,
    canEditSteps: false
  }),
  components: {
    Assignee,
    Steps,
    AttachFileButton
  },
  created () {
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
    attachBusinessCaseClicked () {
      if (this.resources.businessCaseUrl) {
        window.open(this.resources.businessCaseUrl)
      } else {
        this.showBusinessCaseDialog = true
      }
    },
    attachBusinessCase () {
      this.busCaseLoading = true
      this.services.ideas.updateBusinessCase(this.initiative.id, this.temp_businessCaseUrl).then(() => {
        this.busCaseLoading = false
        this.showBusinessCaseDialog = false
        this.resources.businessCaseUrl = this.temp_businessCaseUrl
      }, (err) => {
        this.errors.push(err)
        console.log(err)
      })
    },
    attachInvestmentFormClicked () {
      if (this.resources.investmentFormUrl) {
        window.open(this.resources.investmentFormUrl)
      } else {
        this.showInvestmentFormDialog = true
      }
    },
    attachInvestmentForm () {
      this.invFormLoading = true
      this.services.ideas.updateInvestmentForm(this.initiative.id, this.temp_investmentFormUrl).then(() => {
        this.invFormLoading = false
        this.showInvestmentFormDialog = false
        this.resources.investmentFormUrl = this.temp_investmentFormUrl
      }, (err) => {
        this.errors.push(err)
        console.log(err)
      })
    },
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

  .oct-attach-button {
    width: 100px;
    margin-left: auto;
  }

  .oct-business-case {
    padding: 22px;
  }

 .oct-investment-form {
    padding: 22px;
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

  tbody .md-table-row td {
    border-top: 0px;
  }
</style>