<template>
  <div>
    <div v-if="ideas && ideas.length">
      <initiative v-for="idea in ideas" :key="idea.id" :initiative="idea"></initiative>
    </div>
    
    <router-view></router-view>
  </div>    
</template>

<script>
import Initiative from '@/components/initiative'

export default {
  name: 'ViewIdeas',
  data: () => ({
    ideas: [],
    errors: []
  }),
  components: {
    Initiative
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