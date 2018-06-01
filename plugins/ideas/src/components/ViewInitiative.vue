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
        <br>
        <button v-on:click="accordion" class="accordion">Supporting Documents</button>
        <div class="panel">
          <div>
            <SupportingDocs v-if="showModal" :id="slug" @close="updateSupportingDocs"></SupportingDocs>
          </div>
            <md-table v-model="supportingDocs">
              <md-table-row slot="md-table-row" slot-scope="{ item }">
                <md-table-cell md-label="Title" md-sort-by="title">{{ item.title }}</md-table-cell>
                <md-table-cell md-label="URL" md-sort-by="url">{{ item.url }}</md-table-cell>
                <md-table-cell md-label="Type" md-sort-by="type">{{ item.type | displayDocType }}</md-table-cell>
              </md-table-row>
            </md-table>
            <div v-if="supportingDocs.length === 0 && isLoading == false">
              <md-empty-state
                md-icon="location_city"
                md-label="You have no supporting documents!"
                md-description="Click the + button to get started.">
              </md-empty-state>
            </div>
            <md-button class="md-fab md-accent sd-add-button" @click="showModal = true">
              <md-icon>add</md-icon>
            </md-button>
        </div>
      </div>
      <div v-if="steps != null" class="md-layout-item md-size-30 md-small-size-90 oct-steps">
        <Steps :steps="steps" :isEditable="canEditSteps" v-on:description-updated="updateDescription"></Steps>
      </div>
    </div>
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
      }

      this.isLoading = false
    })
  },
  filters: {
    formatDate,
    displayDocType: function (value) {
      const docTypes = {
        'BusinessCases': 'Business Cases',
        'TechnologyInvestmentForm': 'Technology Investment Form',
        'Other': 'Other'
      }

      if (!docTypes[value]) {
        return 'Unknown'
      }

      return docTypes[value]
    }
  },
  methods: {
    updateDescription (stepIndex) {
      let newDescription = this.steps[stepIndex].description
      let stepId = this.steps[stepIndex].stepId
      this.services.ideas.updateStatusDescription(this.initiative.id, stepId, newDescription).then(() => {
      }, (err) => {
        this.errors.push(err)
      })
    },
    updateSupportingDocs (title, url, type) {
      this.showModal = false
      if (title || url || type) {
        this.supportingDocs.push({title, url, type})
      }
    },
    accordion () {
      var acc = document.getElementsByClassName('accordion')[0]
      acc.classList.toggle('active')
      var panel = acc.nextElementSibling
      if (panel.style.maxHeight) {
        panel.style.maxHeight = null
      } else {
        panel.style.maxHeight = panel.scrollHeight + 'px'
      }
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
    float: right;
  }

  .center {
    display: block;
    margin-left: auto;
    margin-right: auto;
    width: 280px;
    height: auto;
  }

  .accordion {
    background-color: #eee;
    color: #444;
    cursor: pointer;
    padding: 18px;
    width: 100%;
    border: none;
    text-align: left;
    outline: none;
    font-size: 15px;
    transition: 0.4s;
  }

  .active, .accordion:hover {
    background-color: #ccc;
  }

  .accordion:after {
    content: '\002B';
    color: #777;
    font-weight: bold;
    float: right;
    margin-left: 5px;
  }

  .active:after {
    content: "\2212";
  }

  .panel {
    padding: 0 18px;
    background-color: white;
    max-height: 0;
    overflow: hidden;
    transition: max-height 0.2s ease-out;
  }
</style>