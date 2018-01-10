<template>
  <div>
    <md-table v-if="ideas && ideas.length">
      <md-table-row>
        <md-table-head>Title</md-table-head>
        <md-table-head>Stakeholders</md-table-head>
      </md-table-row>

      <md-table-row v-for="idea in ideas" :key="idea.id">
        <md-table-cell>{{ idea.title }}</md-table-cell>
        <md-table-cell>
          <ul v-if="idea.stakeholders && idea.stakeholders.length">
            <li v-for="s in idea.stakeholders" :key="s.id">{{ s.userName }}</li>
          </ul>
        </md-table-cell>
      </md-table-row>

    </md-table>
  </div>    
</template>

<script>
import { HTTP } from '../HttpCommon'

export default {
  name: 'ViewIdeas',
  data: () => ({
    ideas: [],
    errors: []
  }),
  // Fetches ideas when the component is created.
  created () {
    // clear out the ideas
    this.ideas.splice(0, this.ideas.length)
    HTTP.get('').then(response => {
      // JSON responses are automatically parsed.
      this.ideas = response.data
    })
    .catch(e => {
      this.errors.push(e)
    })

    // async / await version (created() becomes async created())
    //
    // try {
    //   const response = await HTTP.get(''
    //   this.ideas = response.data
    // } catch (e) {
    //   this.errors.push(e)
    // }
  },
  methods: {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>
