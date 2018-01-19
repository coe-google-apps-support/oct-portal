<template>
  <div class="et_pb_section  et_pb_section_2 et_section_regular">



    <div class="wrapper et_pb_row et_pb_row_0 et_pb_row_4col">
      <div class="et_pb_column et_pb_column_1_4  et_pb_column_0 et_pb_css_mix_blend_mode_passthrough">


        <div class="et_pb_blurb et_pb_module et_pb_bg_layout_light et_pb_text_align_center  et_pb_blurb_0 et_pb_blurb_position_top">


          <div class="et_pb_blurb_content">
            <div class="et_pb_main_blurb_image">
              <a :href="url">
                <span class="et_pb_image_wrap">
                  <span class="et-pb-icon et-waypoint et_pb_animation_top et-pb-icon-circle et-pb-icon-circle-border et-animated"
                    style="color: #a8a8a8; background-color: #ffffff; border-color: #e0e0e0;">h</span>
                </span>
              </a>
            </div>
            <div class="et_pb_blurb_container">
              <h4 class="et_pb_module_header">
                <a :href="url">Submit</a>
              </h4>
              <div class="et_pb_blurb_description">
                <p>You just did this. Congrats!</p>
              </div>
              <!-- .et_pb_blurb_description -->
            </div>
          </div>
          <!-- .et_pb_blurb_content -->
        </div>
        <!-- .et_pb_blurb -->
      </div>
      <!-- .et_pb_column -->
      <div class="et_pb_column et_pb_column_1_4  et_pb_column_1 et_pb_css_mix_blend_mode_passthrough">


        <div class="et_pb_blurb et_pb_module et_pb_bg_layout_light et_pb_text_align_center  et_pb_blurb_1 et_pb_blurb_position_top">


          <div class="et_pb_blurb_content">
            <div class="et_pb_main_blurb_image">
              <a :href="url">
                <span class="et_pb_image_wrap">
                  <span class="et-pb-icon et-waypoint et_pb_animation_top et-pb-icon-circle et-pb-icon-circle-border et-animated"
                    style="color: #a8a8a8; background-color: #ffffff; border-color: #e0e0e0;">j</span>
                </span>
              </a>
            </div>
            <div class="et_pb_blurb_container">
              <h4 class="et_pb_module_header">
                <a :href="url">Review</a>
              </h4>
              <div class="et_pb_blurb_description">
                <p>Submitted requests will be reviewed to determine the appropriate resources to assign to the request.</p>
              </div>
              <!-- .et_pb_blurb_description -->
            </div>
          </div>
          <!-- .et_pb_blurb_content -->
        </div>
        <!-- .et_pb_blurb -->
      </div>
      <!-- .et_pb_column -->
      <div class="et_pb_column et_pb_column_1_4  et_pb_column_2 et_pb_css_mix_blend_mode_passthrough">


        <div class="et_pb_blurb et_pb_module et_pb_bg_layout_light et_pb_text_align_center  et_pb_blurb_2 et_pb_blurb_position_top">


          <div class="et_pb_blurb_content">
            <div class="et_pb_main_blurb_image">
              <a :href="url">
                <span class="et_pb_image_wrap">
                  <span class="et-pb-icon et-waypoint et_pb_animation_top et-pb-icon-circle et-pb-icon-circle-border et-animated"
                    style="color: #a8a8a8; background-color: #ffffff; border-color: #e0e0e0;">v</span>
                </span>
              </a>
            </div>
            <div class="et_pb_blurb_container">
              <h4 class="et_pb_module_header">
                <a :href="url">Collaborate</a>
              </h4>
              <div class="et_pb_blurb_description">
                <p>Next, we will coordinate with you to gather additional information to better understand the scope of the
                  technology initiative.</p>
              </div>
              <!-- .et_pb_blurb_description -->
            </div>
          </div>
          <!-- .et_pb_blurb_content -->
        </div>
        <!-- .et_pb_blurb -->
      </div>
      <!-- .et_pb_column -->
      <div class="et_pb_column et_pb_column_1_4  et_pb_column_3 et_pb_css_mix_blend_mode_passthrough et-last-child">


        <div class="et_pb_blurb et_pb_module et_pb_bg_layout_light et_pb_text_align_center  et_pb_blurb_3 et_pb_blurb_position_top">


          <div class="et_pb_blurb_content">
            <div class="et_pb_main_blurb_image">
              <a :href="url">
                <span class="et_pb_image_wrap">
                  <span class="et-pb-icon et-waypoint et_pb_animation_top et-pb-icon-circle et-pb-icon-circle-border et-animated"
                    style="color: #a8a8a8; background-color: #ffffff; border-color: #e0e0e0;"></span>
                </span>
              </a>
            </div>
            <div class="et_pb_blurb_container">
              <h4 class="et_pb_module_header">
                <a :href="url">Deliver</a>
              </h4>
              <div class="et_pb_blurb_description">
                <p>Lastly, we will work with the business on execution and delivery of the request.</p>
              </div>
              <!-- .et_pb_blurb_description -->
            </div>
          </div>
          <!-- .et_pb_blurb_content -->
        </div>
        <!-- .et_pb_blurb -->
      </div>
      <!-- .et_pb_column -->


    </div>
    <!-- .et_pb_row -->

  </div>
</template>
<script>
export default {
  name: 'StolenFromDivi',
  props: ['url']
}
</script>
<style lang="scss" scoped>
  .et_pb_blurb_description {
    width: 160px;
  }
  .wrapper {
    display: flex;
    justify-content: space-between;
    padding: 10px;
  }
</style>
