'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  IDEAS_API: '"http://localhost/plugins/initiatives/api"',
  STATIC_ASSETS: '"/static"'
})
