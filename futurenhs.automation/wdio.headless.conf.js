const wdioConfig = require('./wdio.conf.js');

wdioConfig.config.debug = false,
wdioConfig.config.execArgv = [],
wdioConfig.config.maxInstances =  1,
wdioConfig.config.capabilities = [{
    maxInstances: 1,
    browserName: 'chrome',
        'goog:chromeOptions': {
            prefs: {
                'download.default_directory': downloadDir
            },
            args: [
                '--no-sandbox',
                '--headless',
                '--disable-gpu',
                '--disable-dev-smh-usage'
            ]
        }
}];
wdioConfig.config.logLevel = 'error'

exports.config = wdioConfig.config;