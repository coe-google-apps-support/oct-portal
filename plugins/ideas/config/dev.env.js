'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  IDEAS_API: '"http://localhost/wordpress/api/initiatives"',
  STATIC_ASSETS: '"/static"'
})
