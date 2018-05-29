<template>
  <div class="min-height">
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
          </md-table>
        </div>
        <!-- <md-divider class="oct-divider"></md-divider> -->
        <br>
        <div class="md-headline"> Supporting Documents
          <SupportingDocs v-if="showModal" :id="slug" @close="updateSupportingDocs"></SupportingDocs>
          <md-button class="sd-add-button" @click="showModal = true">
            <md-icon>add</md-icon>
          </md-button>
        </div>
        <md-divider class="oct-divider"></md-divider>
          <md-table v-model="supportingDocs">
            <md-table-row slot="md-table-row" slot-scope="{ item }">
              <md-table-cell md-label="Title" md-sort-by="title">{{ item.title }}</md-table-cell>
              <md-table-cell md-label="URL" md-sort-by="url">{{ item.url }}</md-table-cell>
              <md-table-cell md-label="Type" md-sort-by="type">{{ item.type }}</md-table-cell>
            </md-table-row>
          </md-table>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps" :isEditable="canEditSteps" v-on:description-updated="updateDescription"></Steps>
      </div>
    </div>
    <!-- TODO set minimum iframe height instead of <br> -->
    <!-- <br><br><br><br><br><br><br><br><br> -->
  </div>
</template>

<script>
import Assignee from '@/components/Assignee'
import Steps from '@/components/stepper/Steps'
import formatDate from '@/utils/format-date-since'
import SupportingDocs from '@/components/SupportingDocs'
import DiviButton from '@/components/divi/DiviButton'

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
    supportingDocs: [],
    showModal: false
  }),
  components: {
    Assignee,
    Steps,
    SupportingDocs,
    DiviButton
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
      return Promise.all([this.services.ideas.getResources(initiative.id), this.services.ideas.getInitiativeSteps(initiative.id), this.services.ideas.getSupportingDoc(initiative.id)])
    }).then((response) => {
      if (response && response[0]) {
        this.resources = response[0]
        console.log(response[0])
      }
      if (response && response[1]) {
        this.steps = response[1]
      }
      if (response && response[2]) {
        this.supportingDocs = response[2]
        this.updateSupportingDocType()
        console.log(this.supportingDocs)
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
    },
    updateSupportingDocType () {
      let i
      for (i in this.supportingDocs) {
        if (this.supportingDocs[i].type === 'BusinessCases') {
          this.supportingDocs[i].type = 'Business Cases'
        } else if (this.supportingDocs[i].type === 'TechnologyInvestmentForm') {
          this.supportingDocs[i].type = 'Technology Investment Form'
        }
      }
    },
    updateSupportingDocs (title, url, type) {
      this.showModal = false
      this.supportingDocs.push({title: title, url: url, type: type})
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
  
  .min-height {
    min-height: 700px;
  }

  .oct-steps {
    margin: 14px;
  }

  .oct-divider {
    background-color: var(--primary-color);
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