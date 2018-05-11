<template>
  <div>
    <form novalidate class="md-layout-row md-gutter">
      <md-card>
        <md-card-header id="custom-header">
          <div class="md-title">Submit an initiative!</div>
        </md-card-header>
        <div>
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
          <md-button class="md-raised md-primary"  v-on:click="saveIdea"> Submit </md-button>
        </div>
        <md-progress-bar md-mode="indeterminate" class="md-accent" v-if="sending" />
      </md-card>
    </form>
  </div>    
</template>

<script>
import StolenFromDivi from '@/components/StolenFromDivi'
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
    ideaURL: '',
    sending: false,
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
  methods: {
    setDone () {
      this.$v.$touch()
    },
    getValidationClass (fieldName) {
      const field = this.$v.form[fieldName]

      if (field) {
        return {
          'md-invalid': field.$invalid && field.$dirty
        }
      }
    },
    saveIdea () {
      this.sending = true

      console.log('saving new idea')
      this.services.ideas.createInitiative(
        this.form.title,
        this.form.description
      ).then(x => {
        console.log('new idea saved!')
        this.sending = false
        var idea = x.data
        if (idea && idea.url && idea.url.length > 0) {
          this.ideaURL = idea.url
        }
        this.setDone()
        // If title/description are entered, then go to view-ideas.
        if (!this.form.description.required && !this.form.title.required) {
          // this.$toasted.show('Initiative successfully submitted!', {
          //   theme: 'primary',
          //   position: 'top-right',
          //   icon: 'check_circle',
          //   action: {
          //     text: 'Close',
          //     onClick: (e, toastObject) => {
          //       toastObject.goAway(0)
          //     }
          //   }
          // })
          // document.location.href = this.ideaURL
          this.$router.push({path: '/my-profile', query: {isNewIdea: true}})
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
<style lang="scss" scoped>
  @import "../colors.scss";

  .md-card-header {
    background-color: $oct-primary;

    .md-title {
      color: $oct-offoffwhite;
    }
  }

  .md-card {
    margin: 12px;
  }

</style>
<style scoped>

</style>
