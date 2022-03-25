const wdioConfig = require('./wdio.conf.js');

wdioConfig.config.maxInstances =  1,
wdioConfig.config.capabilities = [{
    // Edge Settings
    maxInstances: 1,
    browserName: 'MicrosoftEdge',     
    'ms:edgeOptions': {            
        args: [
            '--headless',
        ],            
        prefs:{  
            'download.default_directory': downloadDir
        }
    }
}];
wdioConfig.config.beforeSession = function (config, capabilities, specs) {
    if(capabilities.browserName === 'MicrosoftEdge' && specs.toString().includes('mobileNavigation.feature')){
        capabilities['ms:edgeOptions'].mobileEmulation = { deviceName: 'iPhone X' } ;
}}

exports.config = wdioConfig.config;