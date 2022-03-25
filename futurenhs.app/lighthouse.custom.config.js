const path = require('path')

module.exports = {
    report: {
        output: 'html',
        outputPath: path.join(__dirname, 'test-reports/lighthouse')
    }
}