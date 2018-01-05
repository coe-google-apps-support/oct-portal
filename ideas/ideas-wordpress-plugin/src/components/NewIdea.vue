<template>
  <div>
    <form novalidate class="md-layout-row md-gutter">
      <!-- <md-card class="md-flex-50 md-flex-small-100">
        <md-card-header>
          <div class="md-title">New Idea</div>
        </md-card-header>
      </md-card> -->

      <md-card-content>
        <div class="md-layout-row md-layout-wrap md-gutter">
          <div class="md-flex md-flex-small-100">
            <md-field :class="getValidationClass('title')">
              <label for="idea-title">Idea</label>
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
              <span class="md-error" v-else-if="!$v.form.description.minlength">Invalid Description</span>
            </md-field>
          </div>

          <md-chips name="tags" id="idea-tags" v-model="form.tags" md-placeholder="Add tag..." />


        </div>
      </md-card-content>

      <md-progress-bar md-mode="indeterminate" v-if="sending" />

      <md-card-actions>
        <md-button type="submit" class="md-primary" v-on:click.prevent="saveIdea" :disabled="sending">Submit idea</md-button>
      </md-card-actions>

    </form>
    <!-- <md-whiteframe md-elevation="2">
        <md-toolbar>
        <h1 class="md-title">New Idea</h1>
        </md-toolbar>
    </md-whiteframe> -->

    <!-- <md-layout md-gutter class="main-content">
        <md-layout md-flex="35" class="form-content">
            <md-layout md-column>
                <md-input-container>
                    <label for="title">Idea</label>
                    <md-input v-model="title" id="idea-title" name="title" required></md-input>
                </md-input-container>

                <md-input-container>
                    <label for="idea-description">Description</label>
                    <md-textarea v-model="description" id="idea-description" name="description" required></md-textarea>
                </md-input-container>

                <md-input-container>
                    <label for="idea-tags">Tags</label>
                    <md-chips v-model="tags" id="idea-tags" name="tags" md-placeholder="Add tag..."></md-chips>
                </md-input-container>            
            </md-layout>
        </md-layout>
    </md-layout> -->

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
    sending: false,
    form: {
      title: null,
      description: null,
      tags: []
    }
  }),
  validations: {
    form: {
      title: {
        required,
        minLength: minLength(3),
        maxLength: maxLength(255)
      },
      description: {
        required,
        minLength: minLength(3)
      }
    }
  },
  methods: {
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
      }).catch((err, y) => {
        this.sending = false
        console.debug(err)
        console.debug(y)
      })
    },
    validateIdea () {
      this.$v.$touch()

      if (!this.$v.$invalid) {
        this.saveIdea()
      }
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
