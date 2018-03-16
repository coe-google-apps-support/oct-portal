<template>
  <div class="md-layout md-alignment-top-center oct-min-size">
      <div v-if="initiative != null" class="md-layout-item md-size-60 md-small-size-90">
        <div class="oct-content-block">
          <span class="md-display-2">{{ initiative.title }}</span>
          <md-divider class="oct-divider"></md-divider>
          <div class="md-caption">{{ initiative.createdDate | formatDate}}</div>
        </div>
        <div class="md-body-1 oct-content-block">{{ initiative.description }}</div>
        <div>
          <div class="md-headline">Resources</div>
          <md-table>
            <md-table-row>
              <md-table-cell>Assigned to</md-table-cell>
              <md-table-cell>
                <Assignee v-if="assignee" :user="assignee"></Assignee>
                <div v-else>Unassigned</div>
              </md-table-cell>
            </md-table-row>
            <md-table-row>
              <md-table-cell>Business case</md-table-cell>
              <md-table-cell>
                <attach-file-button :url="this.initiative.businessCaseUrl" @click.native="attachClicked"></attach-file-button>
              </md-table-cell>
            </md-table-row>
          </md-table>
        </div>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps"></Steps>
      </div>

      <md-dialog v-if="initiative" :md-active.sync="showDialog" class="oct-business-case">
        <md-dialog-title>Business Case</md-dialog-title>
        <md-field>
          <label>URL</label>
          <md-input v-model="initiative.businessCaseUrl"></md-input>
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
    errors: [],
    initiative: null,
    assignee: null,
    steps: null,
    showDialog: false,
    busCaseLoading: false
  }),
  components: {
    Assignee,
    Steps,
    AttachFileButton
  },
  created () {
    this.services.ideas.getInitiative(this.slug).then((initiative) => {
      this.initiative = initiative
      if (!this.initiative.businessCaseUrl) {
        this.initiative.businessCaseUrl = ''
      }
      return this.services.ideas.getAssignee(initiative.id)
    }).then((response) => {
      if (response && response.data) {
        this.assignee = response.data
      }
      return this.services.ideas.getInitiativeSteps(this.initiative.id)
    }).then((response) => {
      this.steps = response
    })
  },
  filters: {
    formatDate
  },
  methods: {
    attachClicked () {
      if (this.initiative.businessCaseUrl) {
        window.open(this.initiative.businessCaseUrl)
      } else {
        this.showDialog = true
      }
    },
    attachBusinessCase () {
      this.busCaseLoading = true
      this.services.ideas.updateInitiative(this.initiative).then(() => {
        this.busCaseLoading = false
        this.showDialog = false
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