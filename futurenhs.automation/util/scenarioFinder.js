const fs = require('fs');
const featureDirectory = './features/'
const child_process = require('child_process');
const files = fs.readdirSync(featureDirectory)
var config = 'wdio.conf.js'
var Url = ''

/**
 * function to search through each Scenario Title, and find the desired based on the input value from the command
 * @param {*} string - input string value to match against specific Scenario title
 * @returns - full string path and line number of desired scenario
 */
var find = function(string){
    var result
    var regex = `Scenario( Outline)?: (.*(${string})( - )([ a-zA-Z]*))`
    files.forEach(file => {
        var contents = fs.readFileSync(featureDirectory + file).toString()
        contents.split(/\r?\n/).forEach((line, lineNumber)=>{
            var x = line.match(regex)
            if (x != null){
                result = `${file}:${lineNumber + 1}`;
                return;
            }
        })
    })
    return result
}

/**
 * Custom config for additional processes to be added into the exec command
 */
if (process.argv !== undefined) {
    process.argv.forEach(arg => {
        if (arg.indexOf('.conf.') !== -1) {
            var configIndex = process.argv.findIndex(arg => arg.includes('.conf.'));
            config = process.argv[configIndex]
        }
        if (arg.indexOf('Url=') !== -1) {
            Url = `--baseUrl=${arg.replace('Url=', '')}`
        }
    });
}

child_process.exec(`wdio ${config} ${Url} --spec=./features/${find(process.argv.slice(-1))}`, function (error,stdout,stderr){console.log(stdout + stderr)})