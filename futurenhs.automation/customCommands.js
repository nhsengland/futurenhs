{
    var exec = require('child-process-promise').exec;
    let obj = {
        "core": `wdio wdio.headless.conf.js --suite=smokeTest --cucumberOpts.tagExpression="@Core and not @Pending and not @NotLocal"`,
        "local": `wdio wdio.headless.conf.js --baseUrl=http://localhost:5000/ --suite=fullRegression --cucumberOpts.tagExpression="not @Pending and not @NotInLocal"`,
        "devTest": `wdio wdio.headless.conf.js --baseUrl=https://collaborate-dev.future.nhs.uk/ --suite=fullRegression`,
        "devTestAxe": `npm run scenario wdio.headless.conf.js Url=https://collaborate-dev.future.nhs.uk/ FNHS:FED02`,
        "devTestLighthouse": `run scenario wdio.headless.conf.js Url=https://collaborate-dev.future.nhs.uk/ FNHS:FED01`,
        "uat": `wdio wdio.headless.conf.js --baseUrl=https://collaborate-uat.future.nhs.uk/ --suite=fullRegression`,
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