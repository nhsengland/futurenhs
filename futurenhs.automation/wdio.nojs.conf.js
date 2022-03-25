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
                '--user-data-dir=C:/Source/FutureNHSDev/futurenhs.automation/profiles/noJSChromeProfile',
                // 'profile-directory=Profile 1'
            ]
        }
}];
wdioConfig.config.logLevel = 'error'

exports.config = wdioConfig.config;