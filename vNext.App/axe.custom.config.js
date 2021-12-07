/**
 * Config for generating aXe accessibility reports
 */
module.exports = {
    shouldIgnoreIssue: (issue) => {

        let shouldIgnore = false;

        /**
         * aXe returns a false positive on olour contrast for elements with a pseudo-element which has a background colour
         */
        if(issue.id === 'color-contrast' && issue.nodes.filter((node) => node.failureSummary === 'Fix any of the following:\n  Element\'s background color could not be determined due to a pseudo element').length === issue.nodes.length){

            shouldIgnore = true;

        }

        return shouldIgnore;

    },
    report: {
        name: 'FNHS front end application',
        outputPath: 'test-reports/axe'
    }
}