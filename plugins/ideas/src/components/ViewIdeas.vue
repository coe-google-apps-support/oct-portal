<template>
  <div>
    <transition name="fade">
      <div v-if="ideas && ideas.length" class="md-layout md-alignment-top-center">
        <initiative v-for="idea in ideas"
          :key="idea.id" 
          :initiative="idea" 
          class="md-layout-item md-size-20 md-medium-size-30 md-small-size-100"
          :onActionClick="openDialog">
        </initiative>
      </div>
    </transition>  
    <md-dialog :md-active.sync="showDialog">
      <InitiativeInfo :id="activeCardInfo"></InitiativeInfo>
    </md-dialog>
  </div>    
</template>

<script>
import Initiative from '@/components/initiative'
import InitiativeInfo from '@/components/ViewInitiative'

export default {
  name: 'ViewIdeas',
  data: () => ({
    ideas: [],
    errors: [],
    showDialog: false,
    activeCardInfo: null
  }),
  components: {
    Initiative,
    InitiativeInfo
  },
  methods: {
    openDialog (initiative) {
      console.log('Opening: ' + initiative.id)
      this.activeCardInfo = initiative.id
      this.showDialog = true
    }
  },
  created () {
    this.ideas.splice(0, this.ideas.length)
    this.services.ideas.getIdeas().then((response) => {
      console.log('received ideas!!')
      this.ideas = response.data
    }, (e) => {
      this.errors.push(e)
    })
  }
}
</script>