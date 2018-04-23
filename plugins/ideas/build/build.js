'use strict'
require('./check-versions')()

process.env.NODE_ENV = 'production'
switch (process.env.npm_config_env) {
  case 'int':
  case 'integration':
    process.env.NODE_ENV = 'integration';
    break;
  case 'local':
  process.env.NODE_ENV = 'local';
  break;
  default:
    process.env.NODE_ENV = 'production'
}

const ora = require('ora')
const rm = require('rimraf')
const path = require('path')
const chalk = require('chalk')
const webpack = require('webpack')
const config = require('../config')
const webpackConfig = require('./webpack.prod.conf')
const ncp = require('ncp').ncp;
const fs = require('file-system')

const spinner = ora('building for ' + process.env.NODE_ENV + '...')
spinner.start()

rm(path.join(config.build.assetsRoot, config.build.assetsSubDirectory), err => {
  if (err) throw err
  webpack(webpackConfig, (err, stats) => {
    spinner.stop()
    if (err) throw err
    process.stdout.write(stats.toString({
      colors: true,
      modules: false,
      children: false, // If you are using ts-loader, setting this to true will make TypeScript errors show up during build.
      chunks: false,
      chunkModules: false
    }) + '\n\n')

    if (stats.hasErrors()) {
      console.log(chalk.red('  Build failed with errors.\n'))
      process.exit(1)
    }
  })
})
