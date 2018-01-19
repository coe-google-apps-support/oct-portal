<template>
  <div>
    <form novalidate class="md-layout-row md-gutter">
      <md-steppers :md-active-step.sync="active" md-alternative md-linear>

        <md-step id="first" md-label="Details" :md-error="firstStepError" :md-done.sync="first">
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
          <md-button class="md-raised md-primary" @click="setDone('first', 'second')">Continue</md-button>
        </md-step>

        <md-step id="second" md-label="Personalize" :md-done.sync="second">
          <md-chips name="tags" id="idea-tags" v-model="form.tags" md-placeholder="Add tag..." />
          <div class="md-flex md-flex-small-100">
            <md-field>
              <label>Attachments</label>
              <md-file v-model="fileAttachments" multiple />
            </md-field>
          </div>
          <md-button class="md-raised md-primary" v-on:click.prevent="saveIdea" :disabled="sending">Done</md-button>
        </md-step>

      </md-steppers>
      <md-progress-bar md-mode="indeterminate" v-if="sending" />      
    </form>
  </div>    
</template>

<script>
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
  data: () => ({
    active: 'first',
    first: false,
    second: false,
    firstStepError: null,
    sending: false,
    form: {
      title: null,
      description: null,
      tags: []
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
        description: this.form.description
      }).then(x => {
        console.log('new idea saved!')
        this.sending = false
        var idea = x.data

        if (idea && idea.url && idea.url.length > 0) {
          window.location.href = idea.url
        }
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
<style scoped>
/* h1, h2 {
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
} */
</style>
