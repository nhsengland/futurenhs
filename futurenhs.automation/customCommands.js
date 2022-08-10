{
    var exec = require('child-process-promise').exec;
    require('dotenv').config()
    let obj = {
        "core": `wdio wdio.headless.conf.js --suite=smokeTest --cucumberOpts.tagExpression="@Core and not @Pending and not @NotLocal"`,
        "local": `wdio wdio.headless.conf.js --baseUrl=${process.env['LOCAL_ENV']} --suite=fullRegression --cucumberOpts.tagExpression="not @Pending and not @NotInLocal"`,
        "devTest": `wdio wdio.headless.conf.js --baseUrl=${process.env['DEVTEST_ENV']} --suite=fullRegression`,
        "devTestAxe": `npm run scenario wdio.headless.conf.js Url=${process.env['DEVTEST_ENV']} FNHS:FED02`,
        "devTestLighthouse": `run scenario wdio.headless.conf.js Url=${process.env['DEVTEST_ENV']} FNHS:FED01`,
        "uat": `wdio wdio.headless.conf.js --baseUrl=${process.env['DEVTEST_ENV']} --suite=fullRegression`,
        "firefox": `wdio wdio.firefox.conf.js --suite=smokeTest`,
        "msedge": `wdio wdio.msedge.conf.js --suite=smokeTest`,
        "visualRegression": `wdio --spec=./features/visualRegression.feature`,
        "AllureReport": `allure generate allureResults --clean -o allureReport`,
    }
    var promise = exec(obj[process.argv[2]]);

    var childProcess = promise.childProcess;
    
    childProcess.stdout.on('data', function(data) {
        console.log(data);
    });
    childProcess.stderr.on('data', function(data) {
        console.log(data);
    });

    promise.then(function () {
        console.log('Automation Test Run Has Successfully Completed.');
    })
    .catch(function (err) {
        console.error('Automation Test Run has failed proceed to review errors above.');
        process.exit(1)
    });
}