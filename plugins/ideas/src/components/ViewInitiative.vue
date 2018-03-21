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
                <attach-file-button :url="resources.businessCaseUrl" @click.native="attachClicked"></attach-file-button>
              </md-table-cell>
            </md-table-row>
          </md-table>
        </div>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps"></Steps>
      </div>
    </div>    

    <md-dialog v-if="initiative && resources" :md-active.sync="showDialog" class="oct-business-case">
      <md-dialog-title>Business Case</md-dialog-title>
      <md-field>
        <label>URL</label>
        <md-input v-model="temp_businessCaseUrl"></md-input>
      </md-field>
      <md-button @click="attachBusinessCase" class="md-primary oct-attach-button">Attach</md-button>
      <md-progress-bar v-if="busCaseLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
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
    errors: [],
    initiative: null,
    assignee: null,
    steps: null,
    showDialog: false,
    busCaseLoading: false,
    isLoading: true,
    resources: null
  }),
  components: {
    Assignee,
    Steps,
    AttachFileButton
  },
  created () {
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
    attachClicked () {
      if (this.resources.businessCaseUrl) {
        window.open(this.resources.businessCaseUrl)
      } else {
        this.showDialog = true
      }
    },
    attachBusinessCase () {
      this.busCaseLoading = true
      this.services.ideas.updateInitiative(this.initiative).then(() => {
        this.busCaseLoading = false
        this.showDialog = false
        this.resources.businessCaseUrl = this.temp_businessCaseUrl
      }, (err) => {
        this.errors.push(err)
        console.log(err)
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