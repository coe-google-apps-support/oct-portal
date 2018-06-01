<template>
  <div>
    <form novalidate class="md-layout-row md-gutter">
      <md-card>
        <div class="form-content">
          <div class="md-flex md-flex-small-100">
            <md-field :class="getValidationClass('title')">
              <label for="idea-title">What is your technology initiative?</label>
              <md-input name="title" id="idea-title" v-model="form.title" />
              <span class="md-error" v-if="!$v.form.title.required">Title is required</span>
              <span class="md-error" v-else-if="!$v.form.title.minlength">Invalid title</span>
            </md-field>
          </div>
          <div class="md-flex md-flex-small-100">
            <md-field :class="getValidationClass('description')">
              <label for="idea-desc">What is the purpose for this request?</label>
              <md-textarea name="description" id="idea-desc" v-model="form.description" />
              <span class="md-error" v-if="!$v.form.description.required">Description is required</span>
              <span class="md-error" v-else-if="!$v.form.description.minlength">Invalid description</span>
            </md-field>
          </div>
          <div class="min-height">
            <md-button @click="showModal = true">
              <label> Add a supporting document</label>
              <md-icon>
                add
              </md-icon>
            </md-button>
            <md-table v-model="supportingDocs">
              <md-table-row slot="md-table-row" slot-scope="{ item }">
                <md-table-cell md-label="Title" md-sort-by="title">{{ item.title }}</md-table-cell>
                <md-table-cell md-label="URL" md-sort-by="url">{{ item.url }}</md-table-cell>
                <md-table-cell md-label="Type" md-sort-by="type">{{ item.type | displayDocType }}</md-table-cell>
              </md-table-row>
            </md-table>
            <SupportingDocs v-if="showModal" :newInit="newInit = true" @close="getSupportingDocs"></SupportingDocs>
          </div>
          <divi-button @click.native="saveIdea">Submit</divi-button>
        </div>
        <md-progress-bar md-mode="indeterminate" class="md-accent" v-if="sending" />
      </md-card>
    </form>
  </div>    
</template>

<script>
import StolenFromDivi from '@/components/StolenFromDivi'
import DiviButton from '@/components/divi/DiviButton'
import { validationMixin } from 'vuelidate'
import SupportingDocs from '@/components/SupportingDocs'
import {
  required,
  minLength,
  maxLength
} from 'vuelidate/lib/validators'

export default {
  name: 'NewIdea',
  mixins: [validationMixin],
  components: {
    StolenFromDivi,
    DiviButton,
    SupportingDocs
  },
  data: () => ({
    ideaURL: '',
    sending: false,
    showModal: false,
    supportingDocs: [],
    form: {
      title: null,
      description: null,
      tags: [],
      hasSponsorship: false,
      sponsorEmail: null,
      hasBudget: false,
      deliveryDate: null
    }
  }),
  validations: {
    form: {
      title: {
        required,
        minLength: minLength(3),
        maxLength: maxLength(140)
      },
      description: {
        required,
        minLength: minLength(3)
      }
    }
  },
  filters: {
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
    openUrl (url) {
      window.open(url, '_top')
    },
    getValidationClass (fieldName) {
      const field = this.$v.form[fieldName]

      if (field) {
        return {
          'md-invalid': field.$invalid && field.$dirty
        }
      }
    },
    getSupportingDocs (title, url, type) {
      this.showModal = false
      if (title || url || type) {
        this.supportingDocs.push({title, url, type})
      }
    },
    saveIdea () {
      // Validate and verify form prior to submission.
      this.$v.$touch()
      if (this.$v.$invalid) {
        return
      }

      this.sending = true

      console.log('saving new idea')
      this.services.ideas.createInitiative(
        this.form.title,
        this.form.description,
        this.supportingDocs
      ).then(x => {
        console.log('new idea saved!')
        this.sending = false
        var idea = x.data
        if (idea && idea.url && idea.url.length > 0) {
          this.ideaURL = idea.url
        }
        // TODO Don't hardcode /you
        if (idea.id) {
          this.openUrl(`/you?newInitiative=${idea.id}`)
        }
      }).catch((err) => {
        this.sending = false
        console.error(err)
      })
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
  @import "../colors.scss";

  .md-card {
    margin: 12px;
  }

  .min-height {
    min-height: 100px;
  }

  .form-content {
    padding: 16px;
  }
</style>
<style scoped>

</style>
