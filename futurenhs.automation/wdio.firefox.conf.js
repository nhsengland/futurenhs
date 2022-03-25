const wdioConfig = require('./wdio.conf.js');

wdioConfig.config.maxInstances =  1,
wdioConfig.config.capabilities = [{
    // Firefox Settings
    maxInstances: 1,
    browserName: 'firefox',
    acceptInsecureCerts : true,
    'moz:firefoxOptions':{
        log: {"level": "fatal"},
        args : [
            '--headless',
        ],
        prefs: {
            'moz:debuggerAddress': true,
            "pdfjs.disabled" : true,
            "browser.download.dir" : downloadDir,
            "browser.download.folderList" : 2,
            "browser.helperApps.alwaysAsk.force" : false,
            "browser.download.manager.showWhenStarting" : false,
            "browser.helperApps.neverAsk.saveToDisk" : 'application/pdf; application/msword;',
            "browser.download.manager.useWindow" : false,
            "browser.download.manager.focusWhenStarting" : false,
            "browser.download.manager.showAlertOnComplete" : false,
            "browser.download.manager.closeWhenDone" : true
        },
    }
}];

exports.config = wdioConfig.config;