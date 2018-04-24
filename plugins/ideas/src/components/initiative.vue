<template>
  <md-card>
    <md-card-media-cover>
      <md-card-media md-ratio="16:9">
        <img :src="getImage()" alt="Skyscraper">
        <div class="oct-cover" :style="{ backgroundColor: getColor(initiative) }"></div>
      </md-card-media>

      <md-card-area>
        <md-card-header>
          <md-card-header-text class="title-container">
            <div class="filler"></div>
            <div class="md-title title">{{ initiative.title }}</div>            
          </md-card-header-text>
        </md-card-header>
      </md-card-area>
    </md-card-media-cover>

    <div class="card-secondary-info">
      <div class="description-container">
        <div class ="description-text">{{ initiative.description }}</div>
      </div>
      <div class="date-text md-subhead">{{ initiative.createdDate | formatDate }}</div>
    </div>
    
    <md-divider></md-divider>

    <md-card-actions>
      <md-button @click="openUrl" :style="{ color: getColor(initiative) }">View</md-button>
    </md-card-actions>
    <md-progress-bar v-if="initiative.isLoading" class="md-accent" md-mode="indeterminate"></md-progress-bar>
  </md-card>
</template>
<script>
import formatDate from '@/utils/format-date-since'

export default {
  name: 'Initiative',
  props: [
    'initiative'
  ],
  filters: {
    truncate (str) {
      const MAX_LENGTH = 300
      if (str.length < MAX_LENGTH) {
        return str
      } else {
        return str.slice(0, MAX_LENGTH) + '...'
      }
    },
    formatDate
  },
  methods: {
    openUrl () {
      window.open(this.initiative.url, '_blank')
    },
    getImage () {
      const root = process.env.STATIC_ASSETS
      const images = [
        `https://octava.blob.core.windows.net/asset-58a0370c-d255-486a-86b4-11d7b0ad205f/card1.png?sv=2015-07-08&sr=c&si=1003172f-f9fc-4210-a1d3-35a5a4d1ff54&sig=g4iz1r0enU%2B0em9zZAU7nlC0yJY%2B55R4Nujas8KVoOo%3D&st=2018-04-24T17%3A02%3A23Z&se=2118-04-24T17%3A02%3A23Z`,
        `https://octava.blob.core.windows.net/asset-7cfad72e-a233-4ca5-a676-0e36088c4d3a/card2.png?sv=2015-07-08&sr=c&si=da184734-7051-4a76-a6a5-3c45945ac52d&sig=AXzCtvMmUPRuYHK6S4%2BlOhPiyrP2JTKdt4sGIIDTCNg%3D&st=2018-04-24T17%3A08%3A58Z&se=2118-04-24T17%3A08%3A58Z`,
        `https://octava.blob.core.windows.net/asset-3913c032-e00a-47e5-9b53-8b225753e1c7/card3.png?sv=2015-07-08&sr=c&si=006c20b4-7ac0-4b77-b137-6e52ebf518bc&sig=Di2N09bE12sLHgXiaZ45XbgHdOHfhn4WOk3%2FMHMlU74%3D&st=2018-04-24T17%3A09%3A31Z&se=2118-04-24T17%3A09%3A31Z`,
        `https://octava.blob.core.windows.net/asset-7ebe5cb4-e11b-4dab-a349-14118e2bc0d2/card4.png?sv=2015-07-08&sr=c&si=e35ae9ff-1e39-446a-ba60-36a2ea5ea3b9&sig=RgxhcMl%2FGETPdCOukgDDe5Sf%2B8XTpLRPBOWK0b74Cvo%3D&st=2018-04-24T17%3A10%3A02Z&se=2118-04-24T17%3A10%3A02Z`,
        `https://octava.blob.core.windows.net/asset-05287925-9a01-4aa2-be72-49257819af70/card5.png?sv=2015-07-08&sr=c&si=845bab3e-0640-4e52-ab5e-d09caccd9995&sig=RfM%2FIMYf1vo%2Fz053QQ496yMvptUPcrTB0pXPs6jytDY%3D&st=2018-04-24T17%3A10%3A25Z&se=2118-04-24T17%3A10%3A25Z`,
        `https://octava.blob.core.windows.net/asset-932f1781-1fcc-4251-9c97-fbbc06adaf00/card6.png?sv=2015-07-08&sr=c&si=c11001ad-14aa-4ab4-a6dc-0937e8eb1e1c&sig=E%2BGFpC%2BkYdljuH9zGBZBBOMFkdfkQ3SvrGCriDPAuOU%3D&st=2018-04-24T17%3A10%3A46Z&se=2118-04-24T17%3A10%3A46Z`,
        `https://octava.blob.core.windows.net/asset-7b5b921c-b98f-458b-9fa0-5484ca1ccdc6/card7.png?sv=2015-07-08&sr=c&si=1f85a495-2630-49d6-ab92-d6770fa688ab&sig=fsZzZsNxfgOakFew4U7XqCItOZM2MlYATOSo1gbrKEA%3D&st=2018-04-24T17%3A11%3A21Z&se=2118-04-24T17%3A11%3A21Z`,
        `https://octava.blob.core.windows.net/asset-89d832a8-8fa6-48a1-b3a6-1b92d78554ec/card8.png?sv=2015-07-08&sr=c&si=aa33f0af-8d43-435b-8494-2cf404b08b49&sig=1AQK%2FUKYvva3WkV%2FAHQnyAMnw%2FuKBwvIXBnes%2Bbzf%2Fk%3D&st=2018-04-24T17%3A11%3A44Z&se=2118-04-24T17%3A11%3A44Z`,
        `https://octava.blob.core.windows.net/asset-febc5c09-e02c-4bff-9007-df100f016f03/card9.png?sv=2015-07-08&sr=c&si=23d87e10-8de2-4da9-a482-9d7461c1514a&sig=wXe86GtuBlSiaO4q2JUEdxVJY9n7VGH9heKcfBC90Ak%3D&st=2018-04-24T17%3A12%3A05Z&se=2118-04-24T17%3A12%3A05Z`,
        `https://octava.blob.core.windows.net/asset-5a0839ca-4d85-42ba-aed5-56a263c7891d/card10.png?sv=2015-07-08&sr=c&si=5340b171-27b8-4f0a-a9f9-18db150fc9d4&sig=JKKCfij2zR8G7uGIVxkffbajmJyefrxbigzx2IjtNsM%3D&st=2018-04-24T17%3A12%3A32Z&se=2118-04-24T17%3A12%3A32Z`,
        `https://octava.blob.core.windows.net/asset-48691775-3d3a-454e-b74e-a95ee69804e0/card11.png?sv=2015-07-08&sr=c&si=1608e0bd-c112-473c-bace-cdff757330d9&sig=sY3o8fzmaWRnJTrT2je8CjQxx6oH5YuEOQYmwtU7Q%2FY%3D&st=2018-04-24T17%3A13%3A37Z&se=2118-04-24T17%3A13%3A37Z`,
        `https://octava.blob.core.windows.net/asset-e50e937e-2aa9-4983-b09a-7265d13a50a1/card12.png?sv=2015-07-08&sr=c&si=2e357411-3aee-4114-931c-2ec81fff3a9f&sig=OSeLjgjbO9mWYf3DocNCVJHwQHqi6YKZIPd0mm5HsbU%3D&st=2018-04-24T17%3A14%3A01Z&se=2118-04-24T17%3A14%3A01Z`,
        `https://octava.blob.core.windows.net/asset-766a3816-e430-4063-bc3e-5cf806ed3dd8/card13.png?sv=2015-07-08&sr=c&si=800f4004-b9dc-4a74-8cc6-fbad98960c1d&sig=IEMWd2Q5h2q7P2z88FWFIANyIxFoTJTsBc2Ym43vT84%3D&st=2018-04-24T17%3A14%3A32Z&se=2118-04-24T17%3A14%3A32Z`,
        `https://octava.blob.core.windows.net/asset-83eb80a5-4820-441e-84c8-2f7f381d347f/card14.png?sv=2015-07-08&sr=c&si=25cd923d-e7ea-4490-82ff-823adc450aaa&sig=VEQuk4oX2J4ds84S84elzeaaretphqHOThvSQxtEc6E%3D&st=2018-04-24T17%3A14%3A49Z&se=2118-04-24T17%3A14%3A49Z`
      ]

      const randIndex = (this.initiative.title.charCodeAt(0) + this.initiative.title.charCodeAt(1) + this.initiative.id) % images.length
      return images[randIndex]
    },
    getColor (idea) {
      // const colors = [
      //   '#e57373',
      //   '#F06292',
      //   '#BA68C8',
      //   '#9575CD',
      //   '#7986CB',
      //   '#64B5F6',
      //   '#4FC3F7',
      //   '#4DD0E1',
      //   '#4DB6AC',
      //   '#81C784',
      //   '#AED581',
      //   '#DCE775',
      //   '#FFF176',
      //   '#FFD54F',
      //   '#FFB74D',
      //   '#FF8A65'
      // ]
      const colors = [
        '#4DB6AC'
      ]

      const randIndex = (idea.title.charCodeAt(0) + idea.title.charCodeAt(1) + idea.id) % colors.length
      return colors[randIndex]
    },
    openCard (idea) {
      this.$router.push(`/ViewIdeas/${idea.id}`)
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import '../mixins.scss';

.description-text {
  position: relative;
  @include multiLineEllipsis($lineHeight: 1.2em, $lineCount: 3, $bgColor: white);
}

.description-container {
  height: 3.6em;
}

.oct-cover {
  position: absolute;
  width: 100%;
  height: 100%;
  opacity: 0.8;
  top: 50%;
  right: 0;
  left: 0;
  transform: translateY(-50%);
  z-index: 1;
}

.md-card {
  margin: 12px;
  display: inline-block;
  vertical-align: top;
  background-color: #fafafa;
}

.title {
  color: #fefefe;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.title-container {
  flex-direction: column;
  display: flex;
}

.filler {
  flex-grow: 1;
}

.card-secondary-info {
  margin: 14px;
}

.card-secondary-info > * {
  margin-bottom: 8px;
  margin-top: 8px;
}

.date-text {
  text-align: right;
}

</style>
