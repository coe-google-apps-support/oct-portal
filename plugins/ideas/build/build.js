'use strict'
require('./check-versions')()

switch (process.env.npm_config_env) {
  case 'int':
  case 'integration':
    process.env.NODE_ENV = 'integration';
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

    // copy coe-ideas.php to dist
    ncp(path.resolve(__dirname, '../php'), config.build.assetsRoot, { }, function() {});

    var filename = path.join(config.build.assetsRoot, "CoeIdeasWebpackBuiltFiles.php");
    fs.readFile(filename, 'utf8', function(fsErr, fsData) {
        if (fsErr) {
          return console.log(chalk.red(fsErr));
        }
        var result = fsData.replace(/\\/g, '/');
      
        fs.writeFile(filename, result, 'utf8', function (fsErr2) {
            if (fsErr2) return console.log(chalk.red(fsErr2));
            console.log(chalk.cyan('  Build complete.\n'))
            console.log(chalk.yellow(
              '  Tip: built files are meant to be served over an HTTP server.\n' +
              '  Opening index.html over file:// won\'t work.\n'
            ))
        });
    });
  })
})
