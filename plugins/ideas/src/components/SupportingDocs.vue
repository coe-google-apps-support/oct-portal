<template><!-- template for the modal component -->
  <transition name="modal">
    <div class="modal-mask">
      <div class="modal-container">
        <div class="modal-header">
          <h3>New Supporting Document</h3>
        </div>
        <div class="modal-body">
          <div v-if='valid == false' class="error-message">
            One of the fields were left empty!
          </div> 
          <label class="form-label">
            Title
            <input id="title" class="form-control" required v-model='form.title' placeholder="What would you like to name your supporting document?">
          </label>
          <label class="form-label">
            URL
            <input id="url" type="url" class="form-control" required v-model='form.url' placeholder="Please enter the full url (i.e. http://)">
          </label>
          <label class="form-label">
            Type
            <select class='form-control' required v-model="form.type">
              <option disabled value="">Please select a type</option>
              <option>Business Cases</option>
              <option>Technology Investment Form</option>
              <option>Other</option>
            </select>
          </label>
        </div>
        <div class="modal-footer text-right">
          <md-button v-if='sending == false' v-on:click="$emit('close')" class="md-raised modal-default-button">
            Cancel
          </md-button>
          <md-button v-on:click="savePost" class="md-raised md-primary modal-default-button">
            Save
          </md-button>
        </div>
        <md-progress-bar md-mode="indeterminate" class="md-primary" v-if="sending" />
      </div>
    </div>
  </transition>
</template>

<script>
export default {
  name: 'SupportingDocs',
  props: [
    'id'
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
  methods: {
    formValidation () {
      // TODO url input field validation
      if (this.form.title === null || this.form.url === null || this.form.type === null) {
        this.valid = false
      } else {
        this.valid = true
      }
    },
    savePost () {
      this.formValidation()
      if (this.valid === true) {
        this.sending = true
        this.services.ideas.createSupportingDoc(
          this.id,
          this.form.title,
          this.form.url,
          this.form.type.replace(/\s/g, '')     // removes spaces to meet backend expectations
        ).then(x => {
          this.sending = false
          this.$emit('close')
          location.reload()
        }).catch((err, y) => {
          this.sending = false
          console.debug(err)
          console.debug(y)
        })
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
  .modal-mask {
    position: fixed;
    z-index: 9998;
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
    margin-bottom: 1em;
  }

  .form-label > .form-control {
    margin-top: 0.5em;
  }

  .form-control {
    display: block;
    width: 80%;
    padding: 0.5em 1em;
    line-height: 1.5;
    border: 1px solid #ddd;
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