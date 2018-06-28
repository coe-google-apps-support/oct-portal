<template>
  <div>
    <div class="full-control md-scrollbar">
      <div class="list">
        <md-list md-expand-single>
          <md-list-item md-expand="true">
            <md-icon>description</md-icon>
            <span class="md-list-item-text">Supporting Documents</span>
            <div class="display-docs md-scrollbar" slot="md-expand">
              <md-table class="fill-width scroll-fix">
                <md-table-row v-for="(doc, index) in documents" v-bind:key="`document-${index}`">
                  <md-table-cell md-label="Title" md-sort-by="title">{{ doc.title }}</md-table-cell>
                  <md-table-cell md-label="URL" md-sort-by="url">{{ doc.url }}</md-table-cell>
                  <md-table-cell md-label="Type" md-sort-by="type">{{ doc.type | displayDocType }}</md-table-cell>
                </md-table-row>
              </md-table>
              <div v-if="documents.length === 0 && editPermissions === false" class="fill-width">
                <md-empty-state
                  md-icon="location_city"
                  md-label="There are no supporting documents!">
                </md-empty-state>
              </div>
              <div v-if="documents.length === 0 && editPermissions === true" class="fill-width">
                <md-empty-state
                  md-icon="location_city"
                  md-label="There are no supporting documents!"
                  md-description="Click the + button to get started.">
                </md-empty-state>
              </div>
              <md-button v-if="editPermissions === true" class="md-fab md-accent" @click="showModal = true">
                <md-icon>add</md-icon>
              </md-button>
            </div>
          </md-list-item>
        </md-list>
      </div>
    </div>
    <transition v-if="showModal" name="modal">
      <div class="modal-mask">
        <div class="modal-container">
          <div class="modal-header">
            <h3>New Supporting Document</h3> 
          </div>
          <div class="modal-body">
            <md-field class="form-control" :class="getValidationClass('title')">
              <label for="supdoc-title">What is the title?</label>
              <md-input name="title" id="supdoc-title" v-model="form.title" />
              <span class="md-error" v-if="!$v.form.title.required">Title is required</span>
              <span class="md-error" v-else-if="!$v.form.title.minlength">Invalid title</span>
            </md-field>
            <md-field class="form-control" :class="getValidationClass('url')">
              <label for="supdoc-url">Documents or links</label>
              <md-input name="url" id="supdoc-url" v-model="form.url" />
              <span class="md-error" v-if="!$v.form.url.required">URL is required</span>
              <span class="md-error" v-else-if="!$v.form.url.minlength">Invalid URL</span>
            </md-field>
            <md-field class="form-control" :class="getValidationClass('type')">
              <label for="supdoc-type">Type</label>
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
    'documents',
    'header'
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
    showModal: false,
    editPermissions: false
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
    checkPermissions (header) {
      if (header['can-edit']) {
        var x = header['can-edit']
        if (x === 'False') {
          this.editPermissions = false
        } else if (x === 'True') {
          this.editPermissions = true
        }
      }
      if (header === true) {
        this.editPermissions = true
      }
    },
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
    }
  },
  created () {
    this.checkPermissions(this.header)
  }
}
</script>

<style lang="scss">
  @media (min-width: 100px) {
    .md-empty-state-label {
      font-size: 11px;
    }
    .md-list-item-text {
      font-size: 12px;
    }
    .md-empty-state-description {
      font-size: 11px;
    }
    .md-list-item-content {
      padding: 0px;
    }
    .md-list-item-text {
      font-size: 9.9px;
    }
    .md-button.md-theme-default.md-fab {
      width: 40px;
      height: 40px;
    }
    .et_pb_button {
      font-size: 13px!important;
    }
    .md-field label {
      font-size: 12px;
    }
    .md-field.md-focused label {
      font-size: 10.5px;
    }
  }
  @media (min-width: 300px) {
    .md-empty-state-label {
      font-size: 14px;
    }
    .md-list-item-text {
      font-size: 12px;
    }
    .md-empty-state-description {
      font-size: 11px;
    }
    .md-field.md-focused label {
      font-size: 10.5px;
    }
    .md-button.md-theme-default.md-fab {
      width: 45px;
      height: 45px;
    }
  }

    @media (min-width: 400px) {
    .md-empty-state-label {
      font-size: 20px;
    }
    .md-list-item-text {
      font-size: 16px;
    }
    .md-field label {
      font-size: 16px;
    }
    .md-empty-state-description {
      font-size: 16px;
    }
    .md-field.md-focused label {
      font-size: 13px;
    }
    .et_pb_button {
      font-size: 20px!important;
    }
    .md-button.md-theme-default.md-fab {
      width: 50px;
      height: 50px;
    }
  }
  @import "~vue-material/dist/theme/engine";

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
    color: var(--primary-color);
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

  /* Accordion overrides */
  /* TODO remove !important rules */
  // Override md-list
  #app-ideas .full-control .md-list-item-expand {
    cursor: auto;
    user-select: inherit;
  }

  #app-ideas .full-control .md-ripple {
    cursor: pointer;
  }

  .display-docs {
    background-color: #fff;
    display: flex;
    flex-direction: column;
    align-items: flex-end;
  }

  .fill-width {
    width: 100%;
  }
  .scroll-fix {
    overflow-x: scroll;
    -webkit-overflow-scrolling: touch
  }
</style>