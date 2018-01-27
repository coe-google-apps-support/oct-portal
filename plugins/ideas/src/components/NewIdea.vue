<template>
  <div>
    <form novalidate class="md-layout-row md-gutter">
      <md-card>
        <md-card-header id="custom-header">
          <div class="md-title">Submit an initiative!</div>
        </md-card-header>

        <md-steppers :md-active-step.sync="active" md-alternative md-linear>

          <md-step id="first" md-label="Personalize" :md-error="firstStepError" :md-done.sync="first">
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
                <label for="idea-desc">Description</label>
                <md-textarea name="description" id="idea-desc" v-model="form.description" />
                <span class="md-error" v-if="!$v.form.description.required">Description is required</span>
                <span class="md-error" v-else-if="!$v.form.description.minlength">Invalid description</span>
              </md-field>
            </div>
            <md-chips name="tags" id="idea-tags" v-model="form.tags" md-placeholder="Add tag..." />
            <div class="md-flex md-flex-small-100">
              <md-field>
                <label>Attachments</label>
                <md-file v-model="fileAttachments" multiple />
              </md-field>
            </div>
            <md-button class="md-raised md-primary" @click="setDone('first', 'second')">Continue</md-button>
          </md-step>

          <md-step id="second" md-label="Details" :md-done.sync="second">
            <div class="md-flex md-flex-small-100">
              <md-checkbox v-model="form.hasSponsorship" class="md-primary">Do you have executive sponsorship for this initiative?</md-checkbox>
            </div>
            <div v-if="form.hasSponsorship" class="md-flex md-flex-small-100">
              <md-field>
                <label for="idea-sponsor">Sponsor email</label>
                <md-input name="sponsorEmail" id="idea-sponsor" v-model="form.sponsorEmail" />
              </md-field>
            </div>
            <div class="md-flex md-flex-small-100">
              <md-checkbox v-model="form.hasBudget" class="md-primary">Do you have a budget for this initiative?</md-checkbox>
            </div>
            <md-datepicker v-model="selectedDate" />
            <md-button class="md-raised md-primary" v-on:click.prevent="saveIdea" :disabled="sending">Submit</md-button>
          </md-step>

          <md-step id="third" md-label="Finalize" :md-done.sync="third">
            <div id="whats-next">What's next?</div>
            <StolenFromDivi :url="ideaURL"></StolenFromDivi>
          </md-step>

        </md-steppers>
        <md-progress-bar md-mode="indeterminate" v-if="sending" />
      </md-card>
    </form>
  </div>    
</template>

<script>
import StolenFromDivi from '@/components/StolenFromDivi'
import { HTTP } from '../HttpCommon'
import { validationMixin } from 'vuelidate'
import {
  required,
  minLength,
  maxLength
} from 'vuelidate/lib/validators'

export default {
  name: 'NewIdea',
  mixins: [validationMixin],
  components: {
    StolenFromDivi
  },
  data: () => ({
    active: 'first',
    ideaURL: '',
    first: false,
    second: false,
    third: false,
    firstStepError: null,
    sending: false,
    form: {
      title: null,
      description: null,
      tags: [],
      hasSponsorship: false,
      sponsorEmail: null,
      hasBudget: false,
      deliveryDate: null
    },
    fileAttachments: []
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
  methods: {
    setDone (id, index) {
      this.$v.$touch()

      if (!this.$v.$invalid) {
        this[id] = true

        this.firstStepError = null

        if (index) {
          this.active = index
        }
      } else {
        this.setError()
      }
    },
    setError () {
      this.firstStepError = 'Uh oh!'
    },
    getValidationClass (fieldName) {
      const field = this.$v.form[fieldName]

      if (field) {
        return {
          'md-invalid': field.$invalid && field.$dirty
        }
      }
    },
    clearForm () {
      this.$v.$reset()
      this.form.title = null
      this.form.description = null
      this.form.tags = []
    },
    saveIdea () {
      this.sending = true

      console.log('saving new idea')

      HTTP.post('', {
        title: this.form.title,
        description: this.form.description,
        businessSponsorEmail: this.form.sponsorEmail,
        hasBudget: this.form.hasBudget,
        expectedTargetDate: this.form.deliveryDate
      }).then(x => {
        console.log('new idea saved!')
        this.sending = false
        var idea = x.data
        if (idea && idea.url && idea.url.length > 0) {
          this.ideaURL = idea.url
        }
        this.setDone('second', 'third')
      }).catch((err, y) => {
        this.sending = false
        console.debug(err)
        console.debug(y)
      })
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
  @import "../colors.scss";

  .md-card-header {
    background-color: $oct-primary;

    .md-title {
      color: $oct-offoffwhite;
    }
  }

  .md-steppers {
    :first-child {
      box-shadow: none;
    }
  }

  .md-card {
    margin: 12px;
  }

  #whats-next {
    color: $oct-primary;
    font-size: 48px;
    letter-spacing: 0;
    line-height: 32px;
    font-weight: 400;
    text-align: center;
    margin: 40px 0px 20px 0px;
  }

</style>
<style scoped>

</style>
