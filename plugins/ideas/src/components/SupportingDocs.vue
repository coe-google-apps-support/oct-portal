<template>
  <transition name="modal">
    <div class="modal-mask">
      <div class="modal-container">
        <div class="modal-header">
          <h3>New Supporting Document</h3> 
        </div>
        <div class="modal-body">
          <md-field class="form-control" :class="getValidationClass('title')">
            <label for="supdoc-title">What is the title of your supporting document?</label>
            <md-input name="title" id="supdoc-title" v-model="form.title" />
            <span class="md-error" v-if="!$v.form.title.required">Title is required</span>
            <span class="md-error" v-else-if="!$v.form.title.minlength">Invalid title</span>
          </md-field>
          <md-field class="form-control" :class="getValidationClass('url')">
            <label for="supdoc-url">Supporting documents or links - Please enter the URL</label>
            <md-input name="url" id="supdoc-url" v-model="form.url" />
            <span class="md-error" v-if="!$v.form.url.required">URL is required</span>
            <span class="md-error" v-else-if="!$v.form.url.minlength">Invalid URL</span>
          </md-field>
          <md-field class="form-control" :class="getValidationClass('type')">
            <label for="supdoc-type">What type of supporting document are you adding?</label>
            <md-select name="type" id="supdoc-type" v-model="form.type">
              <md-option value="BusinessCases">Business Cases</md-option>
              <md-option value="TechnologyInvestmentForm">Technology Investment Form</md-option>
              <md-option value="Other">Other</md-option>
            </md-select>
            <span class="md-error" v-if="!$v.form.type.required">Type is required</span>
          </md-field>
        </div>
        <div class="modal-footer text-right">
          <divi-button v-if="sending == false" @click.native="$emit('close')">Cancel</divi-button>
          <divi-button @click.native="savePost">Save</divi-button>
        </div>
        <md-progress-bar md-mode="indeterminate" class="md-primary" v-if="sending" />
      </div>
    </div>
  </transition>
</template>

<script>
import DiviButton from '@/components/divi/DiviButton'
import { validationMixin } from 'vuelidate'
import {
  required,
  minLength,
  maxLength
} from 'vuelidate/lib/validators'

export default {
  name: 'SupportingDocs',
  components: {
    DiviButton
  },
  mixins: [validationMixin],
  props: [
    'id',
    'newInit'
  ],
  data: () => ({
    form: {
      title: null,
      url: null,
      type: null
    },
    sending: false,
    valid: null
  }),
  validations: {
    form: {
      title: {
        required,
        minLength: minLength(3),
        maxLength: maxLength(50)
      },
      url: {
        required,
        minLength: minLength(3)
      },
      type: {
        required
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
    savePost () {
      this.$v.$touch()
      if (this.$v.$invalid) {
        return
      }
      this.sending = true
      if (!this.newInit) {
        this.services.ideas.createSupportingDoc(
          this.id,
          this.form.title,
          this.form.url,
          this.form.type
        ).then(x => {
          this.sending = false
          this.$emit('close', this.form.title, this.form.url, this.form.type)
        }).catch((err) => {
          this.sending = false
          console.debug(err)
        })
      } else if (this.newInit === true) {
        this.sending = true
        this.$emit('close', this.form.title, this.form.url, this.form.type)
      }
    }
  }
}
</script>

<style lang="scss" scoped>
  @import "~vue-material/dist/theme/engine";

  * {
      position: relative;
      box-sizing: border-box;
  }

  .md-menu-content {
    z-index: 200;
  }

  .modal-mask {
    position: fixed;
    z-index: 5;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    transition: opacity .3s ease;
  }

  .modal-container {
    width: 80%;
    margin: 40px auto 0;
    padding: 20px 30px;
    background-color: #fff;
    border-radius: 2px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, .33);
    transition: all .3s ease;
  }

  .modal-header h3 {
    text-align: center;
    // margin-top: 0;
    color: #3f64df;
  }

  .modal-body {
    margin: 20px 0;
  }

  .text-right {
    text-align: right;
  }

  .form-label {
    display: block;
    margin-bottom: -0.5em;
  }

  .form-label > .form-control {
    margin-top: 0.5em;
  }

  .form-control {
    position: relative;
    width: 80%;
  }

  .modal-enter {
    opacity: 0;
  }

  .modal-leave-active {
    opacity: 0;
  }

  .modal-enter .modal-container,
  .modal-leave-active .modal-container {
    -webkit-transform: scale(1.1);
    transform: scale(1.1);
  }

  .error-message {
    text-align: center;
    color: #f03a3a;
    font-size: 14px;
  }
</style>