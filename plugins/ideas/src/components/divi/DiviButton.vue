<template>
	<body >

		<button class="app"
    v-bind:colour="colour">
    <span>{{message}}</span>
    </button>

	</body>
</template>

<script>

export default {
  name: 'Divi Button',
  props: {
    message: {
      type: String,
      default: 'Press'
    },
    colour: {
      type: String,
      default: 'default'
    }
  }

}
</script>

<style lang="scss" scoped>

.app {
	font-size: 20px;
	font-family: "Open Sans", Arial, sans-serif;
	font-weight: bold;
	background-color: transparent;
	padding: .3em 1em;
	border: 2px solid; 
	border-radius: 3px;
	text-align: center;
	transition: all 0.2s;
	cursor: pointer;
	color: inherit;
	line-height: 1.7em;
&[colour="white"] {
    color: white;
  }
  &[colour="blue"]{
    color: #005087;
  }
}

.app:hover {
border: 2px solid transparent;
background-color:rgba(0,0,0,0.07);
//padding: .3em 1em;

}
.app span{
	cursor: pointer;
	display: inline-block;
	position: relative;
  transition: 0.2s;
	
}
.app span:after {
  content: '>';
  position: absolute;
  opacity: 0;
	border: none;
  top: 0;
  right: 0px;
  transition: 0.2s;
}

.app:hover span {
  padding-right: 20px;
	
}

.app:hover span:after {
  opacity: 1;
  right: -20;
	
}

#center {
align-content:center;

}

</style>