<template>
  <div>
    <button v-on:click="accordion" class="accordion">Supporting Documents</button>
    <div class="panel">
      <md-table>
        <md-table-row v-for="(doc, index) in documents" v-bind:key="`document-${index}`">
          <md-table-cell md-label="Title" md-sort-by="title">{{ doc.title }}</md-table-cell>
          <md-table-cell md-label="URL" md-sort-by="url">{{ doc.url }}</md-table-cell>
          <md-table-cell md-label="Type" md-sort-by="type">{{ doc.type | displayDocType }}</md-table-cell>
        </md-table-row>
      </md-table>
      <div v-if="documents.length === 0">
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
    <transition v-if="showModal" name="modal">
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
            <divi-button @click.native="showModal = false">Cancel</divi-button>
            <divi-button @click.native="saveDocument">Save</divi-button>
          </div>
        </div>
      </div>
    </transition>
  </div>
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
    'newInit',
    'documents'
  ],
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
  data: () => ({
    form: {
      title: null,
      url: null,
      type: null
    },
    valid: null,
    showModal: false
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
    saveDocument () {
      this.$v.$touch()
      if (this.$v.$invalid) {
        return
      }

      this.showModal = false
      this.$emit('close', this.form.title, this.form.url, this.form.type)
      this.form.title = null
      this.form.url = null
      this.form.type = null
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

  /* Documents */
  tbody .md-table-row td {
    border-top: 0px;
  }

  .sd-add-button {
    float: right;
    margin: 16px;
  }

  /* Accordion */
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