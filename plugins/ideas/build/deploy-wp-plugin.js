'use strict'

//const webpackConfig = require('./build')

const rm = require('rimraf')
const path = require('path')
const config = require('../config')
const ncp = require('ncp').ncp;
const chalk = require('chalk')
const fs = require('file-system')

var dest = path.join(config.local.wordpressRoot, config.local.pluginsSubDirectory, config.build.wordpressPluginName);

console.log(chalk.cyan('  deploying to local wordpress...\n'));

rm(dest, err => {
    ncp(config.build.assetsRoot, dest, {
        filter: function(fn) {
            return !fn.endsWith(".html");
        }
    }, function() {
        // replace all backlashes with forward slashes (if any - i.e. if building on windows)
        var filename = path.join(dest, "CoeIdeasWebpackBuiltFiles.php");
        fs.readFile(filename, 'utf8', function(fsErr, fsData) {
            if (fsErr) {
                return console.log(chalk.red(fsErr));
              }
              var result = fsData.replace(/\\/g, '/');
            
              fs.writeFile(filename, result, 'utf8', function (fsErr2) {
                 if (fsErr2) return console.log(chalk.red(fsErr2));
              });
        });
    });

    
});

