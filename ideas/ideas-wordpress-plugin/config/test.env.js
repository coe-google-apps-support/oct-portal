'use strict'
const merge = require('webpack-merge')
const devEnv = require('./dev.env')

module.exports = merge(devEnv, {
  NODE_ENV: '"testing"',
  IDEAS_API: '"https://octportal.edmonton.ca:18197"'
})
